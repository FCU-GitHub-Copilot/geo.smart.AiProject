namespace Geo.Smart.AiAgentHub.Entities.Identity;

/// <summary>
/// 註冊 基本資料 Model
/// </summary>
public class RegisterVmBase
{
    /// <summary>
    /// 建構式
    /// </summary>
    public RegisterVmBase()
    {
        FullName = string.Empty;
    }

    /// <summary>
    /// 使用者名稱 Name
    /// </summary>
    public string FullName { get; set; }

    /// <summary>
    /// 生日
    /// </summary>
    public DateTime? Birthday { get; set; }

    /// <summary>
    /// 聯絡電話
    /// </summary>
    public string? PhoneNumber { get; set; }

    /// <summary>
    /// 驗證
    /// </summary>
    /// <returns></returns>
    public virtual IEnumerable<string> Validate()
    {
        if (string.IsNullOrWhiteSpace(FullName))
        {
            yield return "Name Empty";
        }
    }
}