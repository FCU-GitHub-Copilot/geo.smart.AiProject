namespace Geo.Smart.AiAgentHub.Entities.Identity;

/// <summary>
/// App-忘記密碼k檢查驗證碼VM
/// </summary>
public class ForgotQooCheckVerifyVm
{
    /// <summary>
    /// Email
    /// </summary>
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// 驗證碼
    /// </summary>
    public string VerifyCode { get; set; } = string.Empty;

    /// <summary>
    /// 驗證
    /// </summary>
    /// <returns></returns>
    public (bool success, List<string> errors) Validate()
    {
        var errors = new List<string>();
        if (string.IsNullOrWhiteSpace(Email))
        {
            errors.Add($"{nameof(Email)} is empty");
        }

        if (string.IsNullOrWhiteSpace(VerifyCode))
        {
            errors.Add($"{nameof(VerifyCode)} is empty");
        }

        return (errors.Count == 0, errors);
    }
}