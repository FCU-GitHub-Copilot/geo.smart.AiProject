using Microsoft.AspNetCore.Mvc;

namespace Geo.Smart.AiAgentHub.WebApi.Common;
/// <summary>
/// 已登入的 Controller，繼承自 ControllerBase
/// </summary>
public class SmartController : ControllerBase
{
    /// <summary>
    /// 使用者資訊
    /// </summary>
    protected UserInfo UserInfo => UserHelper.GetUserInfo(User);
}