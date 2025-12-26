using Geo.Smart.AiAgentHub.Entities.Identity.Token;
using Geo.Smart.AiAgentHub.Infras.Enums;
using Geo.Smart.AiAgentHub.WebApi.Filters;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Geo.Smart.AiAgentHub.WebApi.Controllers;

/// <summary>
/// Token 控制項
/// </summary>
[ApiController]
[Route("[controller]/[action]")]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
public class TokenController(
    ITokenService _tokenService
) : ControllerBase
{
    /// <summary>
    /// 取得驗證碼 Captcha
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    [AllowAnonymous]
    public async Task<ActionResult<Result<CaptchaVm>>> Captcha()
    {
        return Ok(await _tokenService.Captcha());
    }

    /// <summary>
    /// 使用帳密登入，取得 TOKEN
    /// </summary>
    /// <param name="login"></param>
    /// <returns></returns>
    [AllowAnonymous]
    [HttpPost()]
    [UserHistory(HistoryType = UserHistoryType.登入系統)]
    public async Task<Result<LoginResultVm>> Login(LoginViewModel login)
    {
        return await _tokenService.Login(login);
    }

    /// <summary>
    /// LDAP 登入
    /// </summary>
    /// <param name="login"></param>
    /// <returns></returns>
    [AllowAnonymous]
    [HttpPost()]
    [UserHistory(HistoryType = UserHistoryType.登入系統)]
    public async Task<Result<LoginResultVm>> Ldap(LoginViewModel login)
    {
        return await _tokenService.Ldap(login);
    }

    /// <summary>
    /// 紀錄登出紀錄
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    [AllowAnonymous]
    [UserHistory(HistoryType = UserHistoryType.登出系統)]
    public ActionResult<bool> Logout()
    {
        return Ok(new Result<object>(true));
    }

    /// <summary>
    /// 透過 Refresh Token，更新 Access Token
    /// </summary>
    /// <returns></returns>
    [AllowAnonymous]
    [HttpPost]
    public async Task<Result<LoginResultVm>> Refresh([FromBody] TokenVm vm)
    {
        return await _tokenService.Refresh(vm);
    }

    /// <summary>
    /// 檢查Email可否使用
    /// </summary>
    /// <param name="email"></param>
    /// <returns></returns>
    [AllowAnonymous]
    [HttpGet]
    public async Task<ActionResult<Result<object>>> IsEmailCanUse(string email)
    {
        return Ok(await _tokenService.IsEmailCanUse(email));
    }

    /// <summary>
    /// 【先隱藏】驗證 JWT Token 是否正確
    /// </summary>
    /// <param name="token"></param>
    /// <returns></returns>
    [ApiExplorerSettings(IgnoreApi = true)]
    [HttpGet]
    [AllowAnonymous]
    public ActionResult<bool> ValidateJwt(string token)
    {
        return Ok(_tokenService.ValidateJwt(token));
    }
}