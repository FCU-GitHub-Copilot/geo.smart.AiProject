using Geo.Smart.AiAgentHub.WebApi.Common;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using static Geo.Smart.AiAgentHub.Infras.ConstantData;

namespace Geo.Smart.AiAgentHub.WebApi.Controllers;

/// <summary>
/// 共通性資料控制項
/// </summary>
[ApiController]
[Route("[controller]/[action]")]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
public class CommonController(ICommonService _service,
    ILdapService _ldapService
    ) : SmartController
{
    /// <summary>
    /// 取得所有的列舉對應
    /// </summary>
    /// <returns> </returns>
    [HttpGet]
    [AllowAnonymous]
    public ActionResult<Dictionary<string, List<KeyName>>> Mappings()
    {
        return Ok(_service.GetAllMapping());
    }

    /// <summary>
    /// 取得組織 dropdown 清單
    /// </summary>
    /// <returns> </returns>
    [ApiExplorerSettings(IgnoreApi = true)]
    [HttpGet]
    public ActionResult<List<CodeName>> GetOrgs()
    {
        return Ok(_service.GetOrgs());
    }

    /// <summary>
    /// 取得角色選單
    /// </summary>
    /// <param name="isAll">true=全撈,false=只撈機關角色</param>
    /// <returns></returns>
    [ApiExplorerSettings(IgnoreApi = true)]
    [HttpGet("{isAll}")]
    public ActionResult<List<CodeName>> GetRole(bool isAll)
    {
        return Ok(_service.GetRole(isAll));
    }

#if DEBUG

    /// <summary>
    /// 【Test】Text Encrypt 文字加密
    /// </summary>
    /// <param name="t"></param>
    /// <returns></returns>
    [HttpPost]
    [ApiExplorerSettings(IgnoreApi = true)]
    public ActionResult<string?> Txtenc([FromForm] string t)
    {
        return Ok(SimpAesHelper.Encrypt(t));
    }

    /// <summary>
    /// 【Test】測試錯誤攔截
    /// </summary>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    [HttpGet]
    [AllowAnonymous]
    [ApiExplorerSettings(IgnoreApi = true)]
    public ActionResult<string> Elmah()
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// ✅LDAP Sync
    /// </summary>
    /// <returns></returns>
    [HttpPost]
    [ApiExplorerSettings(IgnoreApi = true)]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme,
            Roles = Roles.系統管理者)]
    public async Task<IActionResult> LdapSync()
    {
        bool result = await _ldapService.LdapSync();
        return Ok(result);
    }

#endif
}