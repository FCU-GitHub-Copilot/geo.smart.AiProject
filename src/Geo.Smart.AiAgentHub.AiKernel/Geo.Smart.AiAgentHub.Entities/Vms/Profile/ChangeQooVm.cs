using System.ComponentModel.DataAnnotations;

namespace Geo.Smart.AiAgentHub.Entities.Vms.Profile;
/// <summary>
/// 變更密碼用 ViewModel
/// </summary>
public class ChangeQooVm
{
    /// <summary>舊密碼</summary>
    [Required(ErrorMessage = "舊密碼為必填")]
    public string OldQoo { get; set; } = string.Empty;

    /// <summary>新密碼</summary>
    [Required(ErrorMessage = "新密碼為必填")]
    public string NewQoo { get; set; } = string.Empty;

    /// <summary>確認新密碼</summary>
    [Required(ErrorMessage = "確認新密碼為必填")]
    [Compare(nameof(NewQoo), ErrorMessage = "新密碼與確認新密碼不一致")]
    public string ConfirmQoo { get; set; } = string.Empty;

    /// <summary>
    /// 驗證
    /// </summary>
    /// <returns></returns>
    public (bool success, List<string> errors) Validate()
    {
        var errors = new List<string>();
        if (string.IsNullOrWhiteSpace(OldQoo))
        {
            errors.Add($"{nameof(OldQoo)} is empty");
        }

        if (string.IsNullOrWhiteSpace(NewQoo))
        {
            errors.Add($"{nameof(NewQoo)} is empty");
        }
        if (string.IsNullOrWhiteSpace(ConfirmQoo))
        {
            errors.Add($"{nameof(ConfirmQoo)} is empty");
        }

        if (!NewQoo.Equals(ConfirmQoo))
        {
            errors.Add($"Password and confirm password does not match!");
        }

        if (OldQoo?.Equals(NewQoo) ?? false)
        {
            errors.Add($"The password is duplicate");
        }

        return (errors.Count == 0, errors);
    }
}