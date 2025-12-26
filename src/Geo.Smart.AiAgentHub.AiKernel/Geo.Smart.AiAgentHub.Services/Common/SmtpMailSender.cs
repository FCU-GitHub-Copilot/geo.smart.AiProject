using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Net;
using System.Net.Mail;
using System.Text;

namespace Geo.Smart.AiAgentHub.Services.Common;
/// <summary>
/// Mail Server 發送通知信
/// </summary>
public class SmtpMailSender(IConfiguration _configuration, ILogger<SmtpMailSender> _logger) : IEmailSender
{
    /// <summary>
    /// 發送通知信
    /// </summary>
    /// <param name="email"></param>
    /// <param name="subject"></param>
    /// <param name="htmlMessage"></param>
    /// <returns></returns>
    public async Task SendEmailAsync(string email, string subject, string htmlMessage)
    {
        var setting = _configuration.GetSection("SMTP").Get<SmtpSetting>()!;
        var smtpServer = new SmtpClient
        {
            Host = setting.Host,
            Port = setting.Port,
            // 巨鷗測試用 MAIL Server 不使用 SSL，測試時請自行關閉
            EnableSsl = setting.EnableSsl,
            Credentials = new NetworkCredential(setting.Account, setting.Qoo),
        };

        var mail = new MailMessage
        {
            From = new MailAddress(setting.FromMail, setting.DisplayName),
            To = { email },
            Subject = subject,
            Body = htmlMessage,
            IsBodyHtml = true,
            BodyEncoding = Encoding.UTF8,
            SubjectEncoding = Encoding.UTF8,
        };
        try
        {
            await smtpServer.SendMailAsync(mail);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "SendEmailAsync 郵件發送失敗!");
        }
    }
}