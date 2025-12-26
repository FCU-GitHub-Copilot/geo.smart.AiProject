namespace Geo.Smart.AiAgentHub.Entities.Identity;

/// <summary>
/// 會員驗證VM
/// </summary>
public class RegisterVerifyVm
{
    /// <summary>
    ///
    /// </summary>
    public RegisterVerifyVm()
    {
        UserName = string.Empty;
        RegisterVerifyCode = string.Empty;
    }

    /// <summary>
    /// 會員帳號(Email)
    /// </summary>
    public string UserName { get; set; }

    /// <summary>
    /// 驗證碼
    /// </summary>
    public string RegisterVerifyCode { get; set; }
}