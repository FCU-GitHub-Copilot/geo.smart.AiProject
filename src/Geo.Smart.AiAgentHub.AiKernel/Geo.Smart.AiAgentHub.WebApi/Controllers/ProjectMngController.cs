using Geo.Smart.AiAgentHub.AiKernel.Models.Vms;
using Geo.Smart.AiAgentHub.Entities.Vms.AiAgent;
using Geo.Smart.AiAgentHub.WebApi.Common;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Geo.Smart.AiAgentHub.WebApi.Controllers;

/// <summary>
/// AI 專案管理
/// </summary>
/// <param name="_service">AI 專案管理服務</param>
[ApiController]
[Route("[controller]/[action]")]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
public class ProjectMngController(IProjectMngService _service) : SmartController
{
    /// <summary>
    /// 取得 AI 專案資料列表
    /// </summary>
    /// <param name="param">查詢條件</param>
    /// <returns>分頁的 AI 專案資料列表</returns>
    [HttpGet]
    public async Task<ActionResult<PaginationResult<AiProjectListVm>>> Query([FromQuery] QueryBase param)
    {
        return Ok(await _service.Query(param));
    }

    /// <summary>
    /// 取得 AI 專案詳細資料
    /// </summary>
    /// <param name="projectId">專案 ID</param>
    /// <returns>AI 專案詳細資料</returns>
    [HttpGet("{projectId}")]
    public async Task<ActionResult<Result<AiProjectDetailVm>>> Detail(Guid projectId)
    {
        return Ok(await _service.Detail(projectId));
    }

    /// <summary>
    /// 新增 AI 專案資料
    /// </summary>
    /// <param name="vm">AI 專案建立 ViewModel</param>
    /// <returns>新增結果</returns>
    [HttpPost]
    public async Task<ActionResult<Result<string>>> Create([FromBody] AiProjectCreateVm vm)
    {
        return Ok(await _service.Create(vm, UserInfo.UserId));
    }

    /// <summary>
    /// 編輯 AI 專案資料
    /// </summary>
    /// <param name="vm">AI 專案更新 ViewModel</param>
    /// <returns>更新結果</returns>
    [HttpPost]
    public async Task<ActionResult<Result<string>>> Update([FromBody] AiProjectUpdateVm vm)
    {
        return Ok(await _service.Update(vm));
    }

    /// <summary>
    /// 刪除 AI 專案資料
    /// </summary>
    /// <param name="projectId">要刪除的專案 ID</param>
    /// <returns>刪除結果</returns>
    [HttpPost("{projectId}")]
    public async Task<ActionResult<Result<string>>> Delete(Guid projectId)
    {
        return Ok(await _service.Delete(projectId, UserInfo));
    }

    /// <summary>
    /// 下載 AI 專案設定檔 JSON
    /// </summary>
    /// <param name="projectId"></param>
    /// <returns></returns>
    [HttpGet("{projectId}")]
    public async Task<IActionResult> DownloadSetting(Guid projectId)
    {
        var result = await _service.DownloadSetting(projectId, UserInfo);
        if (!result.Success)
        {
            return BadRequest(result);
        }
        byte[] bs = System.Text.Encoding.UTF8.GetBytes(result.Data);
        return File(bs, "application/json", result.ID.ToString());
    }

    /// <summary>
    /// 取得可以選取的 LLM 清單
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    public async Task<ActionResult<Result<List<ProjectLlmVm>>>> Llms()
    {
        return Ok(await _service.GetLlms());
    }

    /// <summary>
    /// 取得可以選取的 MCP Server 清單
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    public async Task<ActionResult<Result<List<ProjectMcpVm>>>> McpServers()
    {
        return Ok(await _service.GetMcpServers());
    }

    /// <summary>
    /// 【從 ChatRoom 搬過來】取得專案設定的 LLM 與工具清單，專案 ID 必要
    /// </summary>
    /// <param name="projectId">專案 ID</param>
    /// <returns></returns>
    [HttpGet]
    public async Task<ActionResult<Result<ModelToolsVm>>> ModelTools(Guid projectId)
    {
        return Ok(await _service.ModelTools(projectId));
    }
}