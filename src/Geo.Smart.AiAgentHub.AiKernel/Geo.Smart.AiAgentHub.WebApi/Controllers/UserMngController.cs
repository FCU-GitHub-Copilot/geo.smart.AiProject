using Geo.Smart.AiAgentHub.Entities.Vms.UserFootprint;
using Geo.Smart.AiAgentHub.Entities.Vms.UserLog;
using Geo.Smart.AiAgentHub.Infras;
using Geo.Smart.AiAgentHub.Infras.Enums;
using Geo.Smart.AiAgentHub.WebApi.Common;
using Geo.Smart.AiAgentHub.WebApi.Filters;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Geo.Smart.AiAgentHub.WebApi.Controllers;

/// <summary>
/// 使用者管理 API Controller
/// </summary>
[ApiController]
[Route("[controller]/[action]")]
[Authorize(Roles = $"{ConstantData.Roles.系統管理者}", AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
[ApiExplorerSettings(IgnoreApi = true)]
public class UserMngController(IUserMngService _service,
    IUserFootprintService _footprintService,
    IUserLogService _userLogService) : SmartController
{
    /// <summary>
    ///系統管理 - 帳號管理 - 使用者清單
    /// </summary>
    /// <param name="param"></param>
    /// <returns></returns>
    [HttpGet]
    public async Task<ActionResult<PaginationResult<UserListVm>>> Query([FromQuery] QueryBase param)
    {
        return Ok(await _service.Query(param));
    }

    /// <summary>
    /// 系統管理 - 帳號管理 - 取得目前登入者個人資料
    /// </summary>
    /// <param name="userId"></param>
    /// <returns></returns>
    [HttpGet("{userId}")]
    public async Task<ActionResult<Result<UserDetailVm>>> Detail(string userId)
    {
        return Ok(await _service.Detail(userId));
    }

    /// <summary>
    /// 系統管理 - 帳號管理 - 新增使用者
    /// </summary>
    /// <param name="newUser"></param>
    /// <returns></returns>
    [HttpPost]
    [UserHistory(HistoryType = UserHistoryType.新增帳號)]
    public async Task<ActionResult<Result<string>>> Create([FromBody] UserCreateVm newUser)
    {
        return Ok(await _service.Create(newUser));
    }

    /// <summary>
    ///系統管理 - 帳號管理 - 編輯使用者資料
    /// </summary>
    /// <param name="user"></param>
    /// <returns></returns>
    [HttpPost]
    [UserHistory(HistoryType = UserHistoryType.編輯帳號)]
    public async Task<ActionResult<Result<string>>> Update([FromBody] UserUpdateVm user)
    {
        return Ok(await _service.Update(user));
    }

    /// <summary>
    ///系統管理 - 帳號管理 - 刪除使用者
    /// </summary>
    /// <param name="userId">要刪除的使用者 Id</param>
    /// <returns>刪除結果</returns>
    [HttpPost("{userId}")]
    [UserHistory(HistoryType = UserHistoryType.刪除帳號)]
    public async Task<ActionResult<Result<string>>> Delete(string userId)
    {
        var result = await _service.Delete(userId);
        return Ok(result);
    }

    /// <summary>
    ///系統管理 - 帳號管理 - 角色清單
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    public async Task<ActionResult<Result<List<CodeName>>>> Role()
    {
        return Ok(await _service.Role());
    }

    /// <summary>
    /// 系統管理 - 帳號管理 - 使用紀錄
    /// </summary>
    /// <param name="param">查詢條件（含日期區間、分頁、排序等）</param>
    /// <returns></returns>
    [HttpGet]
    public async Task<ActionResult<PaginationResult<FullUserFootprintVm>>> UserFootprint([FromQuery] UserFootprintQueryVm param)
    {
        return Ok(await _footprintService.Query(param));
    }

    /// <summary>
    /// 系統管理 - 帳號管理 - 登入記錄查詢
    /// </summary>
    /// <param name="param"></param>
    /// <returns></returns>
    [HttpGet]
    public async Task<ActionResult<PaginationResult<UserLogVm>>> UserLog([FromQuery] UserLogQueryVm param)
    {
        return Ok(await _userLogService.Query(param));
    }
}