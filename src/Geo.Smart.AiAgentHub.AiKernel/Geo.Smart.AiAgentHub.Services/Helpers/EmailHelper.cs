using Geo.Smart.AiAgentHub.Services.Common;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Geo.Smart.AiAgentHub.Services.Helpers;

/// <summary>
/// 寄信小助手，所有的寄信資料處理與範本設定
/// </summary>
[DiLifetime(ServiceLifetime.Scoped)]
public class EmailHelper(IEmailSender _emailSender, IHostEnvironment _hostEnvironment)
{
    /// <summary>
    /// 第一次登入時寄送的通知信
    /// </summary>
    /// <param name="email"></param>
    /// <returns></returns>
    public async Task SendFirstLogin(string email)
    {
        var path = $"{_hostEnvironment.ContentRootPath}/MailTemplates/FirstLogin.html";
        await SendTemplateEmail(
            email, path, [], "【SMART 專案範本 CORE7】- 首次登入"
        );
    }

    /// <summary>
    /// 寄送管理員重設郵件帳號密碼通知
    /// </summary>
    /// <param name="email"></param>
    /// <param name="sysAcc"></param>
    /// <param name="newQoo"></param>
    /// <returns></returns>
    public async Task SendSysAccQooReset(string email, string sysAcc, string newQoo)
    {
        var dictionary = new Dictionary<string, string>
             {
                { "SysAcc", sysAcc },
                { "NewQoo", newQoo }
             };
        var path = $"{_hostEnvironment.ContentRootPath}/MailTemplates/SysAccQooReset.html";
        await SendTemplateEmail(
            email, path, dictionary, "【SMART 專案範本 CORE7】- 管理員重設密碼"
        );
    }

    /// <summary>
    /// 讀取範本寄送Mail
    /// </summary>
    /// <param name="email">寄送位址</param>
    /// <param name="path">範本路徑</param>
    /// <param name="dictionary">取代範本內容Dictionary</param>
    /// <param name="subject">信標標題</param>
    /// <returns></returns>
    public async Task SendTemplateEmail(string email, string path, Dictionary<string, string> dictionary, string subject)
    {
        //讀取範本
        await using var fileStream = new FileStream(path, FileMode.Open);
        using var streamReader = new StreamReader(fileStream);
        var mailContent = await streamReader.ReadToEndAsync();

        //取代文字
        mailContent = TemplateReplace(mailContent, dictionary);

        //寄送
        await _emailSender.SendEmailAsync(email, subject, mailContent);
    }

    private static string TemplateReplace(string template, Dictionary<string, string> dic, string prefix = "{{",
        string postfix = "}}", bool ignoreCase = true)
    {
        var output = template;
        var ordinalIgnoreCase = ignoreCase ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal;

        foreach (var kv in dic)
        {
            output = output.Replace($"{prefix}{kv.Key.ToLower()}{postfix}", kv.Value,
                ordinalIgnoreCase);
        }

        return output;
    }
}