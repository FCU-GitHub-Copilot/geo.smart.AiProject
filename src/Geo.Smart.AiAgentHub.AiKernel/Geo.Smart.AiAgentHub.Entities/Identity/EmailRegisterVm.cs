using Geo.Smart.AiAgentHub.Entities.Enums;
using Geo.Smart.CommonCore.Helpers;
using System.Net.Mail;

namespace Geo.Smart.AiAgentHub.Entities.Identity;
/// <summary>
/// Email註冊Model
/// </summary>
public class EmailRegisterVm : RegisterVmBase
{
    /// <summary>
    /// 建構式
    /// </summary>
    public EmailRegisterVm()
    {
        Email = string.Empty;
        Qoo = string.Empty;
    }

    /// <summary>
    /// Email
    /// </summary>
    public string Email { get; set; }

    /// <summary>
    /// 密碼
    /// </summary>
    public string Qoo { get; set; }

    /// <summary>
    /// 驗證
    /// </summary>
    /// <returns></returns>
    public override IEnumerable<string> Validate()
    {
        if (string.IsNullOrWhiteSpace(Email))
        {
            yield return $"{((int)AccountFailStatus.Email必填).ToString()}";
        }

        if (!string.IsNullOrWhiteSpace(Email) && !IsEmail(Email))
        {
            yield return $"{((int)AccountFailStatus.Email格式錯誤).ToString()}";
        }

        if (string.IsNullOrWhiteSpace(Qoo))
        {
            yield return $"{((int)AccountFailStatus.密碼必填).ToString()}";
        }

        if (!string.IsNullOrWhiteSpace(Qoo) && !RegexHelper.ValidPassword(Qoo))
        {
            yield return $"{((int)AccountFailStatus.密碼格式錯誤).ToString()}";
        }

        foreach (var err in base.Validate())
        {
            yield return err;
        }

        static bool IsEmail(string input)
        {
            return MailAddress.TryCreate(input, out var _);
        }
    }
}