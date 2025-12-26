using Geo.Smart.AiAgentHub.WebApi.Common;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Geo.Smart.AiAgentHub.WebApi.Controllers;
/// <summary>
/// 提供使用者頁面軌跡相關操作的API控制器
/// </summary>
[ApiController]
[Route("[controller]/[action]")]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
public class FootprintController(IFootprintService _service) : SmartController
{
    /// <summary>
    /// 紀錄前端使用者頁面軌跡
    /// </summary>
    /// <param name="vm">使用者頁面軌跡的檢視模型</param>
    /// <returns>操作結果，包含成功或失敗的訊息</returns>
    [HttpPost]
    public ActionResult<Result<string>> Frontend(UserFootprintVm vm)
    {
        return Ok(_service.Frontend(vm, UserInfo.UserId));
    }
}