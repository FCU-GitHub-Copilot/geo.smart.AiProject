using Geo.Smart.CommonCore.Helpers;

namespace Geo.Smart.AiAgentHub.Entities.Identity;

/// <summary>
/// App-忘記密碼修改密碼VM
/// </summary>
public class ForgotUpdateQooVm
{
    /// <summary>
    /// VeriyCodeId
    /// </summary>
    public string VeriyCodeId { get; set; } = string.Empty;

    /// <summary>
    /// 新密碼
    /// </summary>
    public string NewQoo { get; set; } = string.Empty;

    /// <summary>
    /// 新設密碼，再次卻認
    /// </summary>
    public string QooConfirm { get; set; } = string.Empty;

    /// <summary>
    /// 驗證
    /// </summary>
    /// <returns></returns>
    public (bool success, List<string> errors) Validate()
    {
        var errors = new List<string>();
        if (string.IsNullOrWhiteSpace(VeriyCodeId))
        {
            errors.Add($"{nameof(VeriyCodeId)} is empty");
        }

        if (string.IsNullOrWhiteSpace(NewQoo))
        {
            errors.Add($"{nameof(NewQoo)} is empty");
        }

        if (!string.IsNullOrWhiteSpace(NewQoo) && !RegexHelper.ValidPassword(NewQoo))
        {
            errors.Add($"Password Regular Error");
        }

        if (string.IsNullOrWhiteSpace(QooConfirm))
        {
            errors.Add($"{nameof(QooConfirm)} is empty");
        }

        if (!string.IsNullOrWhiteSpace(QooConfirm) && !RegexHelper.ValidPassword(QooConfirm))
        {
            errors.Add($"Password Regular Error");
        }

        if (!string.IsNullOrWhiteSpace(NewQoo) && !string.IsNullOrWhiteSpace(QooConfirm) && NewQoo != QooConfirm)
        {
            errors.Add($"NewQoo and QooConfirm must be the same");
        }

        return (errors.Count == 0, errors);
    }
}