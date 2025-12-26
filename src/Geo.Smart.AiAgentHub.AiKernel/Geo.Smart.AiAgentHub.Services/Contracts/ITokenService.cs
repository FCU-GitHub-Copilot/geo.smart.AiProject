using Geo.Smart.AiAgentHub.Entities.Identity.Token;

namespace Geo.Smart.AiAgentHub.Services.Contracts;

/// <summary>
/// Token 服務介面，定義驗證碼、登入、權杖刷新、Email 檢查與 JWT 驗證相關功能
/// </summary>
public interface ITokenService
{
    /// <summary>
    /// 取得驗證碼
    /// </summary>
    /// <returns>回傳包含驗證碼資訊的結果物件</returns>
    Task<Result<CaptchaVm>> Captcha();

    /// <summary>
    /// 使用者登入
    /// </summary>
    /// <param name="login">登入資訊 ViewModel</param>
    /// <returns>回傳登入結果物件</returns>
    Task<Result<LoginResultVm>> Login(LoginViewModel login);

    /// <summary>
    /// 使用 AD 帳密登入，取得 TOKEN
    /// </summary>
    /// <param name="login">登入 VM</param>
    /// <returns></returns>
    Task<Result<LoginResultVm>> Ldap(LoginViewModel login);

    /// <summary>
    /// 刷新權杖
    /// </summary>
    /// <param name="vm">Token ViewModel</param>
    /// <returns>回傳登入結果物件</returns>
    Task<Result<LoginResultVm>> Refresh(TokenVm vm);

    /// <summary>
    /// 檢查 Email 是否可用
    /// </summary>
    /// <param name="email">欲檢查的電子郵件地址</param>
    /// <returns>回傳結果物件，Data 為 true 表示可用</returns>
    Task<Result<object>> IsEmailCanUse(string email);

    /// <summary>
    /// 驗證 JWT 權杖是否合法
    /// </summary>
    /// <param name="token">JWT 權杖字串</param>
    /// <returns>是否驗證通過</returns>
    bool ValidateJwt(string token);
}