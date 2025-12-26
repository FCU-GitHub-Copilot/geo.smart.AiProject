using Geo.Smart.AiAgentHub.Entities.Vms.AiAgent;
using Geo.Smart.AiAgentHub.WebApi.Common;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Geo.Smart.AiAgentHub.WebApi.Controllers;

/// <summary>
/// LLM 管理
/// </summary>
/// <param name="_service">LLM 管理服務</param>
[ApiController]
[Route("[controller]/[action]")]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
public class LlmMngController(ILlmMngService _service) : SmartController
{
    /// <summary>
    /// 取得 LLM 資料列表
    /// </summary>
    /// <param name="param">查詢條件</param>
    /// <returns>分頁的 LLM 資料列表</returns>
    [HttpGet]
    public async Task<ActionResult<PaginationResult<LlmListVm>>> Query([FromQuery] QueryBase param)
    {
        return Ok(await _service.Query(param));
    }

    /// <summary>
    /// 取得 LLM 詳細資料
    /// </summary>
    /// <param name="llmId">LLM Id</param>
    /// <returns>LLM 詳細資料</returns>
    [HttpGet("{llmId}")]
    public async Task<ActionResult<Result<LlmDetailVm>>> Detail(Guid llmId)
    {
        return Ok(await _service.Detail(llmId));
    }

    /// <summary>
    /// 新增 LLM 設定資料
    /// </summary>
    /// <param name="vm">LLM 設定 ViewModel</param>
    /// <returns>新增結果</returns>
    [HttpPost]
    public async Task<ActionResult<Result<string>>> Create([FromBody] LlmCreateVm vm)
    {
        return Ok(await _service.Create(vm, UserInfo.UserId));
    }

    /// <summary>
    /// 編輯 LLM 資料
    /// </summary>
    /// <param name="vm">LLM 更新 ViewModel</param>
    /// <returns>更新結果</returns>
    [HttpPost]
    public async Task<ActionResult<Result<string>>> Update([FromBody] LlmUpdateVm vm)
    {
        return Ok(await _service.Update(vm));
    }

    /// <summary>
    /// 刪除 LLM 資料
    /// </summary>
    /// <param name="llmId">要刪除的 LLM Id</param>
    /// <returns>刪除結果</returns>
    [HttpPost("{llmId}")]
    public async Task<ActionResult<Result<string>>> Delete(Guid llmId)
    {
        return Ok(await _service.Delete(llmId, UserInfo));
    }
}