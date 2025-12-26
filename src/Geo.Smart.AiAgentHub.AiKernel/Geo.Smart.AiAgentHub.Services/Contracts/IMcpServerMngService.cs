namespace Geo.Smart.AiAgentHub.Services.Contracts;

using Geo.Smart.AiAgentHub.AiKernel.Vm;
using Geo.Smart.AiAgentHub.Entities;
using Geo.Smart.AiAgentHub.Entities.Vms.AiAgent;

/// <summary>
/// MCP Server 管理服務介面
/// </summary>
public interface IMcpServerMngService
{
    /// <summary>
    /// 取得 MCP Server 資料列表
    /// </summary>
    /// <param name="param">查詢條件</param>
    /// <returns>分頁的 MCP Server 資料列表</returns>
    Task<PaginationResult<McpServerListVm>> Query(QueryBase param);

    /// <summary>
    /// 取得 MCP Server 詳細資料
    /// </summary>
    /// <param name="mcpServerId">MCP Server Id</param>
    /// <returns>MCP Server 詳細資料</returns>
    Task<Result<McpServerDetailVm>> Detail(Guid mcpServerId);

    /// <summary>
    /// 新增 MCP Server 資料
    /// </summary>
    /// <param name="vm">MCP Server 設定 ViewModel</param>
    /// <param name="userId">擁有者 UserId</param>
    /// <returns>新增結果</returns>
    Task<Result<string>> Create(McpServerVm vm, string userId);

    /// <summary>
    /// 編輯 MCP Server 資料
    /// </summary>
    /// <param name="vm">MCP Server 更新 ViewModel</param>
    /// <returns>更新結果</returns>
    Task<Result<string>> Update(McpServerUpdateVm vm);

    /// <summary>
    /// 刪除 MCP Server 資料
    /// </summary>
    /// <param name="mcpServerId">要刪除的 MCP Server Id</param>
    /// <param name="userInfo">使用者資訊</param>
    /// <returns>刪除結果</returns>
    Task<Result<string>> Delete(Guid mcpServerId, UserInfo userInfo);
}