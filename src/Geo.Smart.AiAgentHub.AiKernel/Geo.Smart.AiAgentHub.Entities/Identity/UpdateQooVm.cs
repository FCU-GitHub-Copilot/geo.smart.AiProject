namespace Geo.Smart.AiAgentHub.Entities.Identity;

/// <summary>
/// App-修改密碼VM
/// </summary>
public class UpdateQooVm
{
    /// <summary>
    /// 舊密碼
    /// </summary>
    public string OldQoo { get; set; } = string.Empty;

    /// <summary>
    /// 新密碼
    /// </summary>
    public string NewQoo { get; set; } = string.Empty;

    /// <summary>
    /// 再次輸入新密碼
    /// </summary>
    public string NewQooConfirm { get; set; } = string.Empty;

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
        if (string.IsNullOrWhiteSpace(NewQooConfirm))
        {
            errors.Add($"{nameof(NewQooConfirm)} is empty");
        }

        if (!NewQoo.Equals(NewQooConfirm))
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