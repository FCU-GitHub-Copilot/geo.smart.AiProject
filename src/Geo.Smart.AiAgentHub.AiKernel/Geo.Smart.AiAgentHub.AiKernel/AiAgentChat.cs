using Geo.Smart.AiAgentHub.AiKernel.Vm;
using Geo.Smart.AiAgentHub.Infras.Enums;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.AzureOpenAI;
using Microsoft.SemanticKernel.Connectors.Google;
using Microsoft.SemanticKernel.Connectors.Ollama;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using OllamaSharp.Models.Chat;
using System.Text.Json;

namespace Geo.Smart.AiAgentHub.AiKernel;

/// <summary>
/// AI Agent 聊天服務類別，負責處理聊天請求並回傳聊天訊息內容
/// </summary>
public static class AiAgentChat
{
    /// <summary>
    /// 依據聊天請求、聊天歷史、LLM 設定及 MCP 伺服器設定，取得聊天訊息內容
    /// </summary>
    /// <param name="askVm">使用者提問內容</param>
    /// <param name="request">聊天完成的請求參數</param>
    /// <param name="cancellationToken">取消作業的標記</param>
    /// <returns>聊天訊息內容</returns>
    public static async Task<ChatMessageContent> Ask(
        AskVm askVm,
        ChatCompletionRequest request,
        CancellationToken cancellationToken)
    {
        var llmInfo = request.ProjectSetting.LlmInfos
            .First(x => x.ServiceId == askVm.ServiceId);

        var kernel = AiAgentBuilder.GetKernel(llmInfo);

        await AiAgentBuilder.SetMcpTools(
            request.ProjectSetting.McpServers,
            kernel,
            askVm.ToolSelected
        );

        var chatCompletionService = kernel.GetRequiredService<IChatCompletionService>();
        var promptExecutionSettings = GetPromptExecutionSettings(
            llmInfo, request.ProjectSetting);

        var question = GetPromptWithConnectionId(askVm.Message, askVm.ConnectionId);

        if (askVm.Images != null && askVm.Images.Count > 0)
        {
            var items = new ChatMessageContentItemCollection
            {
                new TextContent(question)
            };
            foreach (var image in askVm.Images)
            {
                items.Add(new ImageContent(new Uri(image)));
            }
            request.ChatHistory.Add(new ChatMessageContent(
                AuthorRole.User,
                items: items
            ));
        }
        else
        {
            request.ChatHistory.AddUserMessage(question);
        }

        var result = await chatCompletionService.GetChatMessageContentAsync(
            request.ChatHistory,
            executionSettings: promptExecutionSettings,
            kernel: kernel,
            cancellationToken: cancellationToken
        );
        request.ChatHistory.AddAssistantMessage(result.ToString());
        return result;
    }

    /// <summary>
    /// 取得各 LLM 模型的 PromptExecutionSettings
    /// </summary>
    /// <param name="llmInfo">LLM 服務設定資訊</param>
    /// <param name="projectSetting">AI 專案設定檔</param>
    /// <returns></returns>
    private static PromptExecutionSettings GetPromptExecutionSettings(
        LlmSetupVm llmInfo,
        ProjectSettingVm projectSetting)
    {
        return llmInfo.LlmSourceType switch
        {
            LlmSourceType.OpenAi =>
                CreateOpenAIPromptExecutionSettings(llmInfo, projectSetting),
            LlmSourceType.AzureOpenAi =>
                GetAzureOpenAIPromptExecutionSettings(llmInfo, projectSetting),
            LlmSourceType.Ollama =>
                GetOllamaPromptExecutionSettings(llmInfo, projectSetting),
            LlmSourceType.Gemini =>
                GetGeminiPromptExecutionSettings(llmInfo, projectSetting),

            _ => new PromptExecutionSettings
            {
                ServiceId = llmInfo.ServiceId,
            },
        };
    }

    /// <summary>
    /// 取得 Gemini 的 PromptExecutionSettings
    /// </summary>
    /// <param name="llmInfo">LLM 服務設定資訊</param>
    /// <param name="projectSetting">AI 專案設定檔</param>
    /// <returns></returns>
    private static GeminiPromptExecutionSettings GetGeminiPromptExecutionSettings(
        LlmSetupVm llmInfo,
        ProjectSettingVm projectSetting)
    {
        var pes = new GeminiPromptExecutionSettings
        {
            ServiceId = llmInfo.ServiceId,
            ToolCallBehavior = GeminiToolCallBehavior.AutoInvokeKernelFunctions,
        };
        if (projectSetting.Temperature.HasValue)
        {
            pes.Temperature = projectSetting.Temperature.Value;
        }
        if (projectSetting.TopP.HasValue)
        {
            pes.TopP = projectSetting.TopP.Value;
        }
        if (projectSetting.TopK.HasValue)
        {
            pes.TopK = projectSetting.TopK.Value;
        }
        if (projectSetting.MaxTokens.HasValue)
        {
            pes.MaxTokens = projectSetting.MaxTokens.Value;
        }
        return pes;
    }

    /// <summary>
    /// 取得 Ollama 的 PromptExecutionSettings
    /// </summary>
    /// <param name="llmInfo">LLM 服務設定資訊</param>
    /// <param name="projectSetting">AI 專案設定檔</param>
    /// <returns></returns>
    private static OllamaPromptExecutionSettings GetOllamaPromptExecutionSettings(
        LlmSetupVm llmInfo,
        ProjectSettingVm projectSetting)
    {
        var pes = new OllamaPromptExecutionSettings
        {
            ServiceId = llmInfo.ServiceId,
            FunctionChoiceBehavior = FunctionChoiceBehavior.Auto(options: new()
            {
                RetainArgumentTypes = true,
            }),
        };
        if (projectSetting.Temperature.HasValue)
        {
            pes.Temperature = (float)projectSetting.Temperature.Value;
        }
        if (projectSetting.TopP.HasValue)
        {
            pes.TopP = (float)projectSetting.TopP.Value;
        }
        if (projectSetting.TopK.HasValue)
        {
            pes.TopK = projectSetting.TopK.Value;
        }
        return pes;
    }

    /// <summary>
    /// 取得 Azure AI 的 PromptExecutionSettings
    /// </summary>
    /// <param name="llmInfo">LLM 服務設定資訊</param>
    /// <param name="projectSetting">AI 專案設定檔</param>
    /// <returns></returns>
    private static AzureOpenAIPromptExecutionSettings GetAzureOpenAIPromptExecutionSettings(
        LlmSetupVm llmInfo,
        ProjectSettingVm projectSetting)
    {
        var pes = new AzureOpenAIPromptExecutionSettings
        {
            ServiceId = llmInfo.ServiceId,
            FunctionChoiceBehavior = FunctionChoiceBehavior.Auto(),
        };
        if (projectSetting.Temperature.HasValue)
        {
            pes.Temperature = projectSetting.Temperature.Value;
        }
        if (projectSetting.TopP.HasValue)
        {
            pes.TopP = projectSetting.TopP.Value;
        }
        if (projectSetting.MaxTokens.HasValue)
        {
            pes.MaxTokens = projectSetting.MaxTokens.Value;
        }
        return pes;
    }

    /// <summary>
    /// 取得 OpenAi 的 PromptExecutionSettings
    /// </summary>
    /// <param name="llmInfo">LLM 服務設定資訊</param>
    /// <param name="projectSetting">AI 專案設定檔</param>
    /// <returns></returns>
    private static OpenAIPromptExecutionSettings CreateOpenAIPromptExecutionSettings(
        LlmSetupVm llmInfo,
        ProjectSettingVm projectSetting)
    {
        var pes = new OpenAIPromptExecutionSettings
        {
            ServiceId = llmInfo.ServiceId,
            FunctionChoiceBehavior = FunctionChoiceBehavior.Auto(),
        };
        if (projectSetting.Temperature.HasValue)
        {
            pes.Temperature = projectSetting.Temperature.Value;
        }
        if (projectSetting.TopP.HasValue)
        {
            pes.TopP = projectSetting.TopP.Value;
        }
        if (projectSetting.MaxTokens.HasValue)
        {
            pes.MaxTokens = projectSetting.MaxTokens.Value;
        }
        return pes;
    }

    /// <summary>
    /// 取得帶有連線 ID 的提示內容
    /// </summary>
    /// <param name="message">使用者輸入的訊息</param>
    /// <param name="connectionId">SingleR 的連線 ID</param>
    /// <returns>處理後的提示內容</returns>
    private static string GetPromptWithConnectionId(string message, string connectionId)
    {
        if (string.IsNullOrEmpty(connectionId))
        {
            return message;
        }
        return @$"{message}

```
ClientMapTool 的連線 ID (connectionId) 為：`{connectionId}`
```
";
    }

    /// <summary>
    /// 解析 AI 回覆內容的 Metadata 並建立 ViewModel
    /// </summary>
    /// <param name="result">AI 回覆內容</param>
    /// <returns>AI 回覆紀錄 ViewModel</returns>
    public static ChatCompletionLogVm GetMetadataToLog(ChatMessageContent result)
    {
        var logVm = new ChatCompletionLogVm();

        if (result.Metadata == null)
        {
            return logVm;
        }

        // 取得 LogId
        logVm.LogId = result.Metadata.TryGetValue("Id", out var idObj) && idObj != null
            ? idObj.ToString()
            : Guid.NewGuid().ToString();

        // 取得 CreatedAt
        logVm.CreatedDate = DateTime.Now;

        // 取得 Token 使用數
        SetTokenUsage(result.Metadata, logVm);

        // 取得完整 Metadata JSON
        logVm.Metadata = JsonSerializer.Serialize(result.Metadata);

        return logVm;
    }

    /// <summary>
    /// 取得回覆的建立時間，並統一轉換為 +8 時區
    /// 先不使用
    /// </summary>
    /// <param name="result">AI 回覆內容</param>
    /// <returns></returns>
    public static DateTimeOffset GetCreatedDate(ChatMessageContent result)
    {
        // 取得原始建立時間（預設為 UTC）
        DateTimeOffset createdDate = DateTimeOffset.UtcNow;
        if (result.Metadata != null && result.Metadata.TryGetValue("CreatedAt", out var createdAtObj))
        {
            if (createdAtObj is DateTimeOffset dto)
            {
                createdDate = dto;
            }
            else if (createdAtObj is DateTime dt)
            {
                createdDate = dt.Kind == DateTimeKind.Unspecified
                    ? new DateTimeOffset(DateTime.SpecifyKind(dt, DateTimeKind.Utc))
                    : new DateTimeOffset(dt);
            }
        }
        else if (result.InnerContent is ChatDoneResponseStream chatResponse && chatResponse.CreatedAt.HasValue)
        {
            createdDate = chatResponse.CreatedAt.Value;
        }

        // 統一轉換為 +8 時區
        return createdDate.ToOffset(TimeSpan.FromHours(8));
    }

    /// <summary>
    /// 設定 Token 使用數
    /// </summary>
    /// <param name="metadata">Metadata 字典</param>
    /// <param name="logVm">Log ViewModel</param>
    private static void SetTokenUsage(IReadOnlyDictionary<string, object?> metadata, ChatCompletionLogVm logVm)
    {
        if (!metadata.TryGetValue("Usage", out var usageObject) || usageObject == null)
        {
            return;
        }

        switch (usageObject)
        {
            case OpenAI.Chat.ChatTokenUsage chatTokenUsage:
                logVm.PromptToken = chatTokenUsage.InputTokenCount;
                logVm.CompletionToken = chatTokenUsage.OutputTokenCount;
                logVm.TotalToken = chatTokenUsage.TotalTokenCount;
                break;

            case Microsoft.Extensions.AI.UsageDetails usageDetails:
                logVm.PromptToken = usageDetails.InputTokenCount;
                logVm.CompletionToken = usageDetails.OutputTokenCount;
                logVm.TotalToken = usageDetails.TotalTokenCount;
                break;
        }
    }
}