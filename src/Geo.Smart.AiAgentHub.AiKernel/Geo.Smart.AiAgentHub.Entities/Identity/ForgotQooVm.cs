using Geo.Smart.AiAgentHub.Entities.Enums;

namespace Geo.Smart.AiAgentHub.Entities.Identity;

/// <summary>
/// APP-忘記密碼VM
/// </summary>
public class ForgotQooVm
{
    /// <summary>
    /// 忘記密碼 帳號
    /// </summary>
    public string UserName { get; set; } = string.Empty;

    /// <summary>
    /// 忘記密碼 姓名
    /// </summary>
    public string FullName { get; set; } = string.Empty;

    /// <summary>
    /// 驗證碼 ID
    /// </summary>
    public Guid CaptchaId { get; set; }

    /// <summary>
    /// 驗證碼 Code
    /// </summary>
    public string Captcha { get; set; } = string.Empty;

    /// <summary>
    /// 驗證
    /// </summary>
    /// <returns></returns>
    public (bool success, List<AccountFailStatus> errors) Validate()
    {
        var errors = new List<AccountFailStatus>();
        if (string.IsNullOrWhiteSpace(UserName))
        {
            errors.Add(AccountFailStatus.帳號必填);
        }
        if (string.IsNullOrWhiteSpace(FullName))
        {
            errors.Add(AccountFailStatus.姓名必填);
        }
        return (errors.Count == 0, errors);
    }
}