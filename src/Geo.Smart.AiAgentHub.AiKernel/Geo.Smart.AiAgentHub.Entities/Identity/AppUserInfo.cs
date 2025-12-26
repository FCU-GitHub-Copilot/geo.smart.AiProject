namespace Geo.Smart.AiAgentHub.Entities.Identity;

/// <summary>
/// 登入者資訊
/// </summary>
public class AppUserInfo
{
    /// <summary>
    /// 使用者ID
    /// </summary>
    public string UserId { get; set; } = string.Empty;

    /// <summary>
    /// 登入帳號
    /// </summary>
    public string UserName { get; set; } = string.Empty;

    /// <summary>
    /// E-Mail
    /// </summary>
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// 電話
    /// </summary>
    public string? PhoneNumber { get; set; }
}