using Geo.Smart.AiAgentHub.AiKernel.Models.Vms;
using Geo.Smart.AiAgentHub.AiKernel.Services.Contracts;
using Geo.Smart.CommonCore.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Geo.Smart.AiAgentHub.AiKernel.Controllers;

/// <summary>
/// 聊天室專用
/// </summary>
/// <param name="_service"></param>
[ApiController]
[Route("[controller]/[action]")]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
public class ChatRoomController(IChatRoomService _service) : ControllerBase
{
    /// <summary>
    /// 取得聊天室清單
    /// </summary>
    /// <param name="param">分頁參數</param>
    /// <returns></returns>
    [HttpGet]
    public async Task<ActionResult<PaginationResult<ChatRoomVm>>> Query(
        [FromQuery] QueryBase param)
    {
        return Ok(await _service.Query(param, GetLoginUserId()));
    }

    private string GetLoginUserId()
    {
        return User.Identity!.Name!;
    }

    /// <summary>
    /// 取得聊天室內容
    /// </summary>
    /// <param name="roomId">聊天室 ID</param>
    /// <returns></returns>
    [HttpGet("{roomId}")]
    public async Task<ActionResult<Result<ChatRoomDetailVm>>> Detail(
        Guid roomId)
    {
        return Ok(await _service.Datail(roomId, GetLoginUserId()));
    }

    /// <summary>
    /// 使用者聊天室提問
    /// </summary>
    /// <param name="askVm">提問內容</param>
    /// <param name="cancellationToken">用於中斷請求的 CancellationToken</param>
    /// <returns></returns>
    [HttpPost]
    public async Task<ActionResult<Result<string>>> Ask(ClientAskVm askVm,
        CancellationToken cancellationToken)
    {
        try
        {
            var result = await _service.Ask(askVm, GetLoginUserId(), cancellationToken);
            return Ok(result);
        }
        catch (OperationCanceledException)
        {
            return StatusCode(499, "請求已被客戶端取消");
        }
    }

    /// <summary>
    /// 更新聊天室名稱，Code 可忽略
    /// </summary>
    /// <param name="idName">ID 與 名稱</param>
    /// <returns></returns>
    [HttpPost]
    public async Task<ActionResult<Result<string>>> Rename(IdName idName)
    {
        return Ok(await _service.Rename(idName, GetLoginUserId()));
    }

    /// <summary>
    /// 刪除聊天室
    /// </summary>
    /// <param name="roomId">聊天室 ID</param>
    /// <returns></returns>
    [HttpPost("{roomId}")]
    public async Task<ActionResult<Result<string>>> Delete(Guid roomId)
    {
        return Ok(await _service.Delete(roomId, GetLoginUserId()));
    }

    /// <summary>
    /// 取得專案設定的 LLM 與工具清單
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    public async Task<ActionResult<Result<ModelToolsVm>>> ModelTools()
    {
        return Ok(await _service.ModelTools());
    }
}