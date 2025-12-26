namespace Geo.Smart.AiAgentHub.Entities.Identity;

/// <summary>
/// 登入驗證結果 ViewModel
/// </summary>
public class LoginResultVm
{
    /// <summary>
    /// 登入結果
    /// </summary>
    public string Status { get; set; } = string.Empty;

    /// <summary>
    /// 登入 Token
    /// </summary>
    public string? AccessToken { get; set; }

    /// <summary>
    /// RefreshToken
    /// </summary>
    public string? RefreshToken { get; set; }
}