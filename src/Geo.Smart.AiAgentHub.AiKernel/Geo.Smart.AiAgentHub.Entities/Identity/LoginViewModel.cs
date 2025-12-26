namespace Geo.Smart.AiAgentHub.Entities.Identity;

/// <summary>
/// 登入 VM
/// </summary>
public class LoginViewModel
{
    /// <summary>
    /// 建構子
    /// </summary>
    public LoginViewModel()
    {
        UserName = string.Empty;
        Qoo = string.Empty;
        Captcha = string.Empty;
    }

    /// <summary>
    /// 帳號
    /// </summary>
    public string UserName { get; set; }

    /// <summary>
    /// 密碼
    /// </summary>
    public string Qoo { get; set; }

    /// <summary>
    /// 驗證碼 ID
    /// </summary>
    public Guid CaptchaId { get; set; }

    /// <summary>
    /// 驗證碼 Code
    /// </summary>
    public string Captcha { get; set; }

    /// <summary>
    /// 是否使用OTP驗證
    /// </summary>
    public bool UseOtp { get; set; } = false;
}