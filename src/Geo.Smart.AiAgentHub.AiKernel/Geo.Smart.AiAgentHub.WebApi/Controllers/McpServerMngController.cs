using Geo.Smart.AiAgentHub.AiKernel.Vm;
using Geo.Smart.AiAgentHub.Entities.Vms.AiAgent;
using Geo.Smart.AiAgentHub.WebApi.Common;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Geo.Smart.AiAgentHub.WebApi.Controllers;

/// <summary>
/// MCP Server 管理
/// </summary>
/// <param name="_service">MCP Server 管理服務</param>
[ApiController]
[Route("[controller]/[action]")]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
public class McpServerMngController(IMcpServerMngService _service)
    : SmartController
{
    /// <summary>
    /// 取得 MCP Server 資料列表
    /// </summary>
    /// <param name="param">查詢條件</param>
    /// <returns>分頁的 MCP Server 資料列表</returns>
    [HttpGet]
    public async Task<ActionResult<PaginationResult<McpServerListVm>>> Query([FromQuery] QueryBase param)
    {
        return Ok(await _service.Query(param));
    }

    /// <summary>
    /// 取得 MCP Server 詳細資料
    /// </summary>
    /// <param name="mcpServerId">MCP Server Id</param>
    /// <returns>MCP Server 詳細資料</returns>
    [HttpGet("{mcpServerId}")]
    public async Task<ActionResult<Result<McpServerDetailVm>>> Detail(Guid mcpServerId)
    {
        return Ok(await _service.Detail(mcpServerId));
    }

    /// <summary>
    /// 新增 MCP Server 資料
    /// </summary>
    /// <param name="vm">MCP Server 設定 ViewModel</param>
    /// <returns>新增結果</returns>
    [HttpPost]
    public async Task<ActionResult<Result<string>>> Create([FromBody] McpServerVm vm)
    {
        return Ok(await _service.Create(vm, UserInfo.UserId));
    }

    /// <summary>
    /// 編輯 MCP Server 資料
    /// </summary>
    /// <param name="vm">MCP Server 更新 ViewModel</param>
    /// <returns>更新結果</returns>
    [HttpPost]
    public async Task<ActionResult<Result<string>>> Update([FromBody] McpServerUpdateVm vm)
    {
        return Ok(await _service.Update(vm));
    }

    /// <summary>
    /// 刪除 MCP Server 資料
    /// </summary>
    /// <param name="mcpServerId">要刪除的 MCP Server Id</param>
    /// <returns>刪除結果</returns>
    [HttpPost("{mcpServerId}")]
    public async Task<ActionResult<Result<string>>> Delete(Guid mcpServerId)
    {
        return Ok(await _service.Delete(mcpServerId, UserInfo));
    }
}