namespace Geo.Smart.AiAgentHub.Entities;

/// <summary>
/// SMTP 設定
/// </summary>
public class SmtpSetting
{
    /// <summary>
    /// SMTP Server
    /// </summary>
    public required string Host { get; set; }

    /// <summary>
    /// SMTP Port
    /// </summary>
    public required int Port { get; set; }

    /// <summary>
    /// 是否使用 SSL
    /// </summary>
    public required bool EnableSsl { get; set; }

    /// <summary>
    /// 寄信帳號
    /// </summary>
    public required string Account { get; set; }

    /// <summary>
    /// 密碼
    /// </summary>
    public required string Qoo { get; set; }

    /// <summary>
    /// 寄件者 Email
    /// </summary>
    public required string FromMail { get; set; }

    /// <summary>
    /// 寄件者
    /// </summary>
    public required string DisplayName { get; set; }
}