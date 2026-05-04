using Geo.Smart.AiAgentHub.AiKernel;
using Geo.Smart.AiAgentHub.AiKernel.Vm;
using Geo.Smart.AiAgentHub.Entities.Vms.AiAgent;
using Geo.Smart.AiAgentHub.Infras;
using Geo.Smart.AiAgentHub.Services.Common;
using Geo.Smart.AiAgentHub.Services.Contracts;
using Geo.Smart.AiAgentHub.Services.Extension;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace Geo.Smart.AiAgentHub.Services;

/// <summary>
/// MCP Server 管理服務
/// </summary>
/// <param name="dbModel">資料庫內容物件</param>
/// <param name="_logger">log 物件</param>
public class McpServerMngService(GdbContext dbModel,
    ILogger<CommonService> _logger
    ) : BaseService(dbModel, _logger), IMcpServerMngService
{
    /// <summary>
    /// 取得 MCP Server 資料列表
    /// </summary>
    /// <param name="param">查詢條件</param>
    /// <returns>分頁的 MCP Server 資料列表</returns>
    public async Task<PaginationResult<McpServerListVm>> Query(QueryBase param)
    {
        try
        {
            var query = DbModel.McpServers.AsNoTracking()
                .Where(x => x.IsEnabled)
                .WhereIf(!string.IsNullOrEmpty(param.Keyword), x =>
                    x.Name.Contains(param.Keyword!) ||
                    (x.SseUrl != null && x.SseUrl.Contains(param.Keyword!)) ||
                    (x.StdioCommand != null && x.StdioCommand.Contains(param.Keyword!))
                )
                .Select(x => new McpServerListVm
                {
                    McpServerId = x.McpServerId,
                    Name = x.Name,
                    McpServerType = x.McpServerType,
                    SseUrl = x.SseUrl,
                    StdioCommand = x.StdioCommand
                });

            return await ResultHelper.PaginationSuccessAsync(query, param);
        }
        catch (Exception ex)
        {
            LogError(ex, nameof(Query));
            return ResultHelper.PaginationFailure<McpServerListVm>(ex.Message);
        }
    }

    /// <summary>
    /// 取得 MCP Server 詳細資料
    /// </summary>
    /// <param name="mcpServerId">MCP Server Id</param>
    /// <returns>MCP Server 詳細資料</returns>
    public async Task<Result<McpServerDetailVm>> Detail(Guid mcpServerId)
    {
        try
        {
            var server = await DbModel.McpServers.AsNoTracking()
                .Include(x => x.ApplicationUser)
                .Where(x => x.McpServerId == mcpServerId && x.IsEnabled)
                .FirstOrDefaultAsync();
            if (server == null)
            {
                return ResultHelper.Failure<McpServerDetailVm>(ConstantData.Error.NoData);
            }

            var vm = new McpServerDetailVm
            {
                McpServerId = server.McpServerId,
                Name = server.Name,
                McpServerType = server.McpServerType,
                SseUrl = server.SseUrl,
                StdioCommand = server.StdioCommand,
                StdioArgs = !string.IsNullOrEmpty(server.StdioArgs)
                    ? JsonSerializer.Deserialize<List<string>>(server.StdioArgs) : [],
                StdioEnv = !string.IsNullOrEmpty(server.StdioEnv)
                    ? JsonSerializer.Deserialize<Dictionary<string, string?>>(server.StdioEnv) : [],
                Tools = JsonSerializer.Deserialize<List<string>>(server.Tools),
                UserName = server.ApplicationUser.FullName
            };
            return ResultHelper.Success(vm);
        }
        catch (Exception ex)
        {
            LogError(ex, nameof(Detail));
            return ResultHelper.Failure<McpServerDetailVm>(ex.Message);
        }
    }

    /// <summary>
    /// 新增 MCP Server 資料
    /// </summary>
    /// <param name="vm">MCP Server 設定 ViewModel</param>
    /// <param name="userId">擁有者 UserId</param>
    /// <returns>新增結果</returns>
    public async Task<Result<string>> Create(McpServerVm vm, string userId)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(vm.Name))
            {
                return ResultHelper.Failure<string>("MCP Server 名稱為必填");
            }

            if (!Regex.IsMatch(vm.Name, @"^[a-zA-Z0-9_]+$"))
            {
                return ResultHelper.Failure<string>("MCP Server 名稱只能包含英文字母、數字及底線");
            }
            var exists = await DbModel.McpServers.AsNoTracking()
                .AnyAsync(x => x.Name == vm.Name && x.IsEnabled);
            if (exists)
            {
                return ResultHelper.Failure<string>("MCP Server 名稱已存在");
            }
            if (vm.McpServerType == McpServerType.Stdio)
            {
                return ResultHelper.Failure<string>("目前不支援 stdio");
            }
            var entity = new McpServer
            {
                McpServerId = Guid.NewGuid(),
                Name = vm.Name,
                McpServerType = vm.McpServerType,
                SseUrl = vm.SseUrl,
                StdioCommand = vm.StdioCommand,
                StdioArgs = vm.StdioArgs != null ? JsonSerializer.Serialize(vm.StdioArgs) : null,
                StdioEnv = vm.StdioEnv != null ? JsonSerializer.Serialize(vm.StdioEnv) : null,
                UserId = userId,
                Tools = await GetMcpTools(vm),
                IsEnabled = true
            };

            DbModel.McpServers.Add(entity);
            await DbModel.SaveChangesAsync();

            return ResultHelper.Success(entity.McpServerId.ToString());
        }
        catch (Exception ex)
        {
            LogError(ex, nameof(Create));
            return ResultHelper.Failure<string>(ex.Message);
        }
    }

    private static async Task<string> GetMcpTools(McpServerVm vm)
    {
        var tools = await AiAgentBuilder.GetMcpClientToolsAsync(vm);
        return JsonSerializer.Serialize(tools.Select(x => x.Name));
    }

    /// <summary>
    /// 編輯 MCP Server 資料
    /// </summary>
    /// <param name="vm">MCP Server 更新 ViewModel</param>
    /// <returns>更新結果</returns>
    public async Task<Result<string>> Update(McpServerUpdateVm vm)
    {
        try
        {
            var requiredFields = new Dictionary<string, string>
            {
                { nameof(vm.McpServerId), vm.McpServerId == Guid.Empty ? string.Empty : vm.McpServerId.ToString() },
                { nameof(vm.Name), vm.Name }
            };
            var emptyField = requiredFields.FirstOrDefault(f => string.IsNullOrWhiteSpace(f.Value));
            if (!string.IsNullOrEmpty(emptyField.Key))
            {
                return ResultHelper.Failure<string>($"{emptyField.Key}為必填");
            }
            if (!Regex.IsMatch(vm.Name, @"^[a-zA-Z0-9_]+$"))
            {
                return ResultHelper.Failure<string>("MCP Server 名稱只能包含英文字母、數字及底線");
            }

            if (vm.McpServerType == McpServerType.Stdio)
            {
                return ResultHelper.Failure<string>("目前不支援 stdio");
            }
            var exist = await DbModel.McpServers
                .AsNoTracking()
                .AnyAsync(x => x.McpServerId != vm.McpServerId && x.Name == vm.Name && x.IsEnabled);
            if (exist)
            {
                return ResultHelper.Failure<string>("MCP Server 名稱已經被使用，請填寫其他名稱");
            }

            var entity = await DbModel.McpServers
                .FirstOrDefaultAsync(x => x.McpServerId == vm.McpServerId && x.IsEnabled);
            if (entity == null)
            {
                return ResultHelper.Failure<string>(ConstantData.Error.NoData);
            }

            entity.Name = vm.Name;
            entity.McpServerType = vm.McpServerType;
            entity.SseUrl = vm.SseUrl;
            entity.StdioCommand = vm.StdioCommand;
            entity.StdioArgs = vm.StdioArgs != null ? JsonSerializer.Serialize(vm.StdioArgs) : null;
            entity.StdioEnv = vm.StdioEnv != null ? JsonSerializer.Serialize(vm.StdioEnv) : null;
            entity.Tools = await GetMcpTools(vm);

            await DbModel.SaveChangesAsync();
            return ResultHelper.Success(entity.McpServerId.ToString());
        }
        catch (Exception ex)
        {
            LogError(ex, nameof(Update));
            return ResultHelper.Failure<string>(ex.Message);
        }
    }

    /// <summary>
    /// 刪除 MCP Server 資料
    /// </summary>
    /// <param name="mcpServerId">要刪除的 MCP Server Id</param>
    /// <param name="userInfo">使用者資訊</param>
    /// <returns>刪除結果</returns>
    public async Task<Result<string>> Delete(Guid mcpServerId, UserInfo userInfo)
    {
        try
        {
            var entity = await DbModel.McpServers
                .FirstOrDefaultAsync(x => x.McpServerId == mcpServerId && x.IsEnabled);

            if (entity == null)
            {
                return ResultHelper.Failure<string>(ConstantData.Error.NoData);
            }
            if (!DefaultCfgs.IsAdmin(userInfo)
                && userInfo.UserId != entity.UserId)
            {
                return ResultHelper.Failure<string>(ConstantData.Error.NoAuthority);
            }
            entity.IsEnabled = false;
            await DbModel.SaveChangesAsync();
            return ResultHelper.Success(entity.McpServerId.ToString());
        }
        catch (Exception ex)
        {
            LogError(ex, nameof(Delete));
            return ResultHelper.Failure<string>(ex.Message);
        }
    }
}