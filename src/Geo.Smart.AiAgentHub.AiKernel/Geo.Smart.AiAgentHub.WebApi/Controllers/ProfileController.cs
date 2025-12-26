using Geo.Smart.AiAgentHub.Entities.Vms.Profile;
using Geo.Smart.AiAgentHub.Infras.Enums;
using Geo.Smart.AiAgentHub.WebApi.Common;
using Geo.Smart.AiAgentHub.WebApi.Filters;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Geo.Smart.AiAgentHub.WebApi.Controllers;

/// <summary>
/// 個人資料管理控制項
/// </summary>
[ApiController]
[Route("[controller]/[action]")]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
public class ProfileController(IProfileService _profileService) : SmartController
{
    /// <summary>
    /// 個人設定管理 - 個人資料設定 - 取得登入者的個人資料
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    public async Task<ActionResult<Result<UserVm>>> Me()
    {
        var result = await _profileService.Me(UserInfo.UserId);
        return Ok(result);
    }

    /// <summary>
    /// 個人設定管理 - 個人資料設定 - 編輯登入者的個人資料
    /// </summary>
    /// <param name="vm">個人資料編輯用 ViewModel</param>
    /// <returns>更新後的個人資料</returns>
    [HttpPost]
    [UserHistory(HistoryType = UserHistoryType.編輯帳號)]
    public async Task<ActionResult<Result<MeVm>>> UpdateMe([FromBody] UpdateVm vm)
    {
        var result = await _profileService.Update(UserInfo.UserId, vm);
        return Ok(result);
    }

    /// <summary>
    /// 個人設定管理 - 個人資料設定 - 變更登入者密碼
    /// </summary>
    /// <param name="vm">變更密碼內容</param>
    /// <returns>變更結果訊息</returns>
    [HttpPost]
    [UserHistory(HistoryType = UserHistoryType.變更密碼)]
    public async Task<ActionResult<Result<string>>> UpdateQoo([FromBody] ChangeQooVm vm)
    {
        var result = await _profileService.UpdateQoo(UserInfo.UserId, vm);
        return Ok(result);
    }

    /// <summary>
    /// 系統管理 - 帳號管理 - 取得所有啟用的組織清單
    /// </summary>
    /// <returns>組織清單</returns>
    [HttpGet]
    public async Task<ActionResult<Result<List<OrgIdName>>>> Org()
    {
        return Ok(await _profileService.Org());
    }

    /// <summary>
    /// 取得目前登入者的角色 RoleId
    /// </summary>
    /// <returns>角色 RoleId</returns>
    [HttpGet]
    public async Task<ActionResult<Result<string>>> UserRoleId()
    {
        var result = await _profileService.GetUserRoleId(UserInfo.UserId);
        return Ok(result);
    }

    /// <summary>
    /// 強制更新密碼
    /// </summary>
    /// <param name="vm"></param>
    /// <returns></returns>
    [HttpPost()]
    [AllowAnonymous]
    [UserHistory(HistoryType = UserHistoryType.變更密碼)]
    [ApiExplorerSettings(IgnoreApi = true)]
    public async Task<ActionResult<Result<string>>> EnforceResetPassword(EnforceResetVm vm)
    {
        return Ok(await _profileService.EnforceResetPassword(vm));
    }

    /// <summary>
    /// 請求忘記密碼
    /// </summary>
    /// <param name="vm"></param>
    /// <returns></returns>
    [HttpPost()]
    [AllowAnonymous]
    [ApiExplorerSettings(IgnoreApi = true)]
    public async Task<ActionResult<Result<string>>> ForgotQooRequest(ForgotQooVm vm)
    {
        return Ok(await _profileService.ForgotQoo(vm));
    }

    /// <summary>
    /// 忘記密碼 - 修改密碼
    /// </summary>
    /// <param name="vm"></param>
    /// <returns></returns>
    [HttpPost]
    [AllowAnonymous]
    [ApiExplorerSettings(IgnoreApi = true)]
    public async Task<ActionResult<Result<string>>> ForgotQooUpdate(ForgotUpdateQooVm vm)
    {
        return Ok(await _profileService.ForgotQooUpdate(vm));
    }
}