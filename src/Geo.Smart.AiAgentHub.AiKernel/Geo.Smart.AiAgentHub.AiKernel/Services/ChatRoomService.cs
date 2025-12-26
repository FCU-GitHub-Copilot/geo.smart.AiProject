using Geo.Smart.AiAgentHub.AiKernel.Models;
using Geo.Smart.AiAgentHub.AiKernel.Models.Vms;
using Geo.Smart.AiAgentHub.AiKernel.Services.Contracts;
using Geo.Smart.AiAgentHub.AiKernel.Vm;
using Geo.Smart.AiAgentHub.Infras;
using Geo.Smart.CommonCore.Helpers;
using Geo.Smart.CommonCore.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.SemanticKernel.ChatCompletion;
using System.Text.Json;

namespace Geo.Smart.AiAgentHub.AiKernel.Services;

/// <summary>
/// 聊天室服務
/// </summary>
public class ChatRoomService(AiHubContext _dbModel,
    IOptions<ProjectSettingVm> projectOptions,
    ILogger<ChatRoomService> _logger)
    : IChatRoomService
{
    /// <summary>
    /// 取得指定使用者的聊天室清單
    /// </summary>
    /// <param name="param">分頁參數</param>
    /// <param name="userId">使用者識別碼</param>
    /// <returns>包含聊天室清單的分頁結果物件</returns>
    public async Task<PaginationResult<ChatRoomVm>> Query(QueryBase param, string userId)
    {
        try
        {
            var query = _dbModel.ChatRooms.AsNoTracking()
                .Where(x => x.IsEnabled && x.CreatedBy == userId)
                .WhereIf(!string.IsNullOrWhiteSpace(param.Keyword), x =>
                    x.Name.Contains(param.Keyword)
                )
                .Select(x => new ChatRoomVm
                {
                    RoomId = x.RoomId,
                    Name = x.Name,
                    CreatedDate = x.CreatedDate,
                    MessagesCount = x.ChatMessages.Count,
                });
            return await ResultHelper.PaginationSuccessAsync(query, param);
        }
        catch (Exception e)
        {
            LogError(e, nameof(Query));
            return ResultHelper.PaginationFailure<ChatRoomVm>(e.Message);
        }
    }

    /// <summary>
    /// 取得指定聊天室詳細資訊
    /// </summary>
    /// <param name="roomId">聊天室 ID</param>
    /// <param name="userId">使用者 ID</param>
    /// <returns>包含聊天室詳細資訊的結果物件</returns>
    public async Task<Result<ChatRoomDetailVm>> Datail(Guid roomId, string userId)
    {
        try
        {
            var chatRoom = await _dbModel.ChatRooms
                .Include(x => x.ChatMessages)
                .AsNoTracking()
                .FirstOrDefaultAsync(x =>
                    x.IsEnabled && x.CreatedBy == userId && x.RoomId == roomId
                );

            if (chatRoom == null)
            {
                return ResultHelper.Failure<ChatRoomDetailVm>(ConstantData.Error.NoData);
            }

            var vm = new ChatRoomDetailVm
            {
                RoomId = chatRoom.RoomId,
                Name = chatRoom.Name,
                CreatedDate = chatRoom.CreatedDate,
                MessagesCount = chatRoom.ChatMessages.Count,
                LlmServiceId = chatRoom.LlmServiceId,
                ToolSelected = string.IsNullOrWhiteSpace(chatRoom.ToolSelected)
                    ? []
                    : JsonSerializer.Deserialize<Dictionary<string, List<string>>>(chatRoom.ToolSelected)!,
                ChatMessages = [.. chatRoom.ChatMessages
                    .Where(cm => cm.IsEnabled)
                    .OrderBy(cm => cm.SentAt)
                    .Select(cm => new ChatMessageVm
                    {
                        MessageId = cm.MessageId,
                        Role = cm.Role,
                        Content = cm.Content,
                        SentAt = cm.SentAt
                    })]
            };

            return ResultHelper.Success(vm);
        }
        catch (Exception e)
        {
            LogError(e, nameof(Datail));
            return ResultHelper.Failure<ChatRoomDetailVm>(ConstantData.Error.Exception);
        }
    }

    /// <summary>
    /// 使用者聊天室提問
    /// </summary>
    /// <param name="askVm">提問內容</param>
    /// <param name="userId">使用者 ID</param>
    /// <param name="cancellationToken">用於中斷請求的 CancellationToken</param>
    /// <returns>包含 AI 回覆內容的結果物件</returns>
    public async Task<Result<string>> Ask(ClientAskVm askVm, string userId,
        CancellationToken cancellationToken)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(askVm.Message))
            {
                return ResultHelper.Failure<string>("請輸入提問內容");
            }

            var projectSetting = projectOptions.Value;
            if (projectSetting.LlmInfos.FirstOrDefault(x => x.ServiceId == askVm.ServiceId) == null)
            {
                return ResultHelper.Failure<string>("LLM 模型設定錯誤");
            }
            try
            {
                var chatroom = await GetOrCreateChatRoom(askVm, userId,
                    projectSetting.SystemPrompt,
                    cancellationToken);
                if (chatroom == null)
                {
                    return ResultHelper.Failure<string>(ConstantData.Error.NoData);
                }
                var chatHistory = JsonSerializer.Deserialize<ChatHistory>(chatroom.History);
                await SaveUserChatMessage(askVm, chatroom, cancellationToken);

                var chatRequest = new ChatCompletionRequest
                {
                    ProjectSetting = projectSetting,
                    ChatHistory = chatHistory!,
                };

                // 呼叫 AiAgentChat 取得回覆
                var askResult = await AiAgentChat.Ask(askVm, chatRequest, cancellationToken);

                var resultMessage = askResult.ToString();

                var logVm = AiAgentChat.GetMetadataToLog(askResult);
                AddChatAssistantMessage(chatroom.RoomId, askVm, resultMessage, logVm);
                AddChatCompletionLog(logVm);

                chatroom.History = JsonSerializer.Serialize(chatHistory);
                await _dbModel.SaveChangesAsync(cancellationToken);

                var result = ResultHelper.Success(resultMessage);
                if (!askVm.RoomId.HasValue)
                {
                    result.ID = chatroom.RoomId;
                }
                return result;
            }
            catch (OperationCanceledException oce)
            {
                LogError(oce, nameof(Ask));
                return ResultHelper.Failure<string>("操作已取消");
            }
            catch (Exception ex)
            {
                LogError(ex, nameof(Ask));
                return ResultHelper.Failure<string>(ConstantData.Error.Exception);
            }
        }
        catch (Exception e)
        {
            LogError(e, nameof(Ask));
            return ResultHelper.Failure<string>(ConstantData.Error.Exception);
        }
    }

    /// <summary>
    /// 儲存使用者提問訊息
    /// </summary>
    /// <param name="askVm">提問內容</param>
    /// <param name="chatroom">聊天室實體</param>
    /// <param name="cancellationToken">用於中斷請求的 CancellationToken</param>
    private async Task SaveUserChatMessage(AskVm askVm, ChatRoom chatroom, CancellationToken cancellationToken)
    {
        _dbModel.ChatMessages.Add(new ChatMessage
        {
            RoomId = chatroom.RoomId,
            Role = AuthorRole.User.ToString(),
            Content = askVm.Message,
            LlmServiceId = askVm.ServiceId,
            ToolSelected = JsonSerializer.Serialize(askVm.ToolSelected),
        });
        await _dbModel.SaveChangesAsync(cancellationToken);
    }

    /// <summary>
    /// 新增 AI 回覆紀錄
    /// </summary>
    /// <param name="logVm">AI 回覆紀錄 ViewModel</param>
    private void AddChatCompletionLog(ChatCompletionLogVm logVm)
    {
        var log = new ChatCompletionLog
        {
            LogId = logVm.LogId,
            Metadata = logVm.Metadata,
            CreatedDate = logVm.CreatedDate,
            PromptToken = logVm.PromptToken,
            CompletionToken = logVm.CompletionToken,
            TotalToken = logVm.TotalToken
        };
        _dbModel.ChatCompletionLogs.Add(log);
    }

    /// <summary>
    /// 新增 AI 助手訊息
    /// </summary>
    /// <param name="roomId">聊天室 ID</param>
    /// <param name="askVm">提問請求</param>
    /// <param name="message">AI 回覆訊息內容</param>
    /// <param name="logVm">AI 回覆紀錄 ViewModel</param>
    private void AddChatAssistantMessage(Guid roomId, AskVm askVm,
         string message, ChatCompletionLogVm logVm)
    {
        var outputMessage = new ChatMessage
        {
            RoomId = roomId,
            Role = AuthorRole.Assistant.ToString(),
            Content = message,
            SentAt = logVm.CreatedDate.DateTime,
            LlmServiceId = askVm.ServiceId,
            LogId = logVm.LogId,
            Tokens = logVm.TotalToken,
            ToolSelected = JsonSerializer.Serialize(askVm.ToolSelected),
        };
        _dbModel.ChatMessages.Add(outputMessage);
    }

    /// <summary>
    /// 建立聊天室，若無指定聊天室則自動建立
    /// </summary>
    /// <param name="askVm">使用者提問</param>
    /// <param name="userId">使用者 ID</param>
    /// <param name="systemPrompt">系統提示詞</param>
    /// <param name="cancellationToken">用於中斷請求的 CancellationToken</param>
    /// <returns>聊天室實體</returns>
    private async Task<ChatRoom?> GetOrCreateChatRoom(AskVm askVm, string userId,
        string systemPrompt,
        CancellationToken cancellationToken)
    {
        ChatRoom? chatRoom;
        if (askVm.RoomId != null)
        {
            chatRoom = await _dbModel.ChatRooms
                .Include(x => x.ChatMessages)
                .Where(x => x.IsEnabled && x.CreatedBy == userId
                    && x.RoomId == askVm.RoomId)
                .FirstOrDefaultAsync(cancellationToken: cancellationToken);
            if (chatRoom != null)
            {
                chatRoom.LlmServiceId = askVm.ServiceId;
                chatRoom.ToolSelected = JsonSerializer.Serialize(askVm.ToolSelected);
                return chatRoom;
            }
        }
        else
        {
            chatRoom = new ChatRoom
            {
                RoomId = Guid.NewGuid(),
                // 只取 message 前十個字，若長度不足則全部取用
                Name = (askVm.Message.Length > 10) ? askVm.Message[..10] : askVm.Message,
                LlmServiceId = askVm.ServiceId,
                ToolSelected = JsonSerializer.Serialize(askVm.ToolSelected),
            };
            var chatHistory = new ChatHistory();
            chatHistory.AddSystemMessage(systemPrompt);
            chatRoom.History = JsonSerializer.Serialize(chatHistory);
            _dbModel.ChatRooms.Add(chatRoom);
        }

        await _dbModel.SaveChangesAsync(cancellationToken);
        return chatRoom;
    }

    /// <summary>
    /// 更新聊天室名稱
    /// </summary>
    /// <param name="idName">ID 與名稱</param>
    /// <param name="userId">使用者 ID</param>
    /// <returns>更新後的聊天室名稱結果物件</returns>
    public async Task<Result<string>> Rename(IdName idName, string userId)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(idName.Name))
            {
                return ResultHelper.Failure<string>("請輸入聊天室名稱");
            }
            var room = await _dbModel.ChatRooms
                .Where(x => x.IsEnabled && x.CreatedBy == userId
                    && x.RoomId == idName.Id)
                .FirstOrDefaultAsync();
            if (room == null)
            {
                return ResultHelper.Failure<string>(ConstantData.Error.NoData);
            }
            room.Name = idName.Name;
            await _dbModel.SaveChangesAsync();
            return ResultHelper.Success(idName.Name);
        }
        catch (Exception e)
        {
            LogError(e, nameof(Rename));
            return ResultHelper.Failure<string>(ConstantData.Error.Exception);
        }
    }

    /// <summary>
    /// 刪除聊天室（軟刪除）
    /// </summary>
    /// <param name="roomId">聊天室 ID</param>
    /// <param name="userId">使用者 ID</param>
    /// <returns>刪除結果物件</returns>
    public async Task<Result<string>> Delete(Guid roomId, string userId)
    {
        try
        {
            var room = await _dbModel.ChatRooms
                .Where(x => x.IsEnabled && x.CreatedBy == userId
                    && x.RoomId == roomId)
                .FirstOrDefaultAsync();
            if (room == null)
            {
                return ResultHelper.Failure<string>(ConstantData.Error.NoData);
            }
            room.IsEnabled = false;
            await _dbModel.SaveChangesAsync();
            return ResultHelper.Success(string.Empty);
        }
        catch (Exception e)
        {
            LogError(e, nameof(Rename));
            return ResultHelper.Failure<string>(ConstantData.Error.Exception);
        }
    }

    /// <summary>
    /// 取得專案設定的 LLM 與工具清單
    /// </summary>
    /// <returns></returns>
    public async Task<Result<ModelToolsVm>> ModelTools()
    {
        try
        {
            var projectSetting = projectOptions.Value;
            var vm = new ModelToolsVm
            {
                Llms = [.. projectSetting.LlmInfos.Select(x => new ModelToolsLlm
                {
                    ServiceId = x.ServiceId,
                    ModelId = x.ModelId,
                    LlmSourceType = x.LlmSourceType,
                })],
                McpServers = [.. projectSetting.McpServers.Select(x => new ModelToolsMcp
                {
                    Name = x.Name,
                    McpServerType = x.McpServerType,
                    Tools = x.Tools,
                })]
            };

            return ResultHelper.Success(vm);
        }
        catch (Exception e)
        {
            LogError(e, nameof(ModelTools));
            return ResultHelper.Failure<ModelToolsVm>(ConstantData.Error.Exception);
        }
    }

    /// <summary>
    /// 記錄錯誤日誌
    /// </summary>
    /// <param name="e">例外狀況物件</param>
    /// <param name="methodName">發生錯誤的方法名稱</param>
    protected void LogError(Exception e, string methodName)
    {
        _logger.LogError(e, "Error {MethodName}：{Message}", methodName, e.Message);
    }
}