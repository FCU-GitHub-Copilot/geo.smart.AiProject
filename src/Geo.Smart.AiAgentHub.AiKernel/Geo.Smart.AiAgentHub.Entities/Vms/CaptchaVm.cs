namespace Geo.Smart.AiAgentHub.Entities.Vms;

/// <summary>
/// 產製出的 captcha 資訊
/// </summary>
public class CaptchaVm
{
    /// <summary>
    /// Captcha Id
    /// </summary>
    public Guid CaptchaId { get; set; }

    /// <summary>
    /// Captcha 圖示內容
    /// </summary>
    public string CaptchaBase64 { get; set; } = string.Empty;
}