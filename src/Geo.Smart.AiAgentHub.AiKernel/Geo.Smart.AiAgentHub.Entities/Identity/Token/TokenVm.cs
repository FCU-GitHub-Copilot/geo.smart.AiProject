using Geo.Smart.AiAgentHub.Entities.Enums;

namespace Geo.Smart.AiAgentHub.Entities.Identity.Token;
/// <summary>
/// 產製 JWT Token 的 ViewModel
/// </summary>
public class TokenVm
{
    /// <summary>
    /// Access Token
    /// </summary>
    public required string AccessToken { get; set; }

    /// <summary>
    /// Refresh Token
    /// </summary>
    public required string RefreshToken { get; set; }

    /// <summary>
    /// 登入失敗狀態
    /// </summary>
    public LoginFailStatus? Status { get; set; }
}