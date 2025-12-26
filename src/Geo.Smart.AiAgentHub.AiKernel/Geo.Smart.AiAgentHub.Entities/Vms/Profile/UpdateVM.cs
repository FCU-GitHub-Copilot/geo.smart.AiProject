using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Geo.Smart.AiAgentHub.Entities.Vms.Profile;

/// <summary>
/// 個人資料編輯用 ViewModel
/// </summary>
public class UpdateVm
{
    /// <summary>
    /// 使用者ID
    /// </summary>
    public string UserId { get; set; } = string.Empty;

    /// <summary>
    /// 帳號
    /// </summary>
    [Required(ErrorMessage = "帳號為必填")]
    public string? UserName { get; set; }

    /// <summary>
    /// 信箱
    /// </summary>
    [Required(ErrorMessage = "信箱為必填")]
    [EmailAddress(ErrorMessage = "信箱格式不正確")]
    public string? Email { get; set; }

    /// <summary>
    /// 姓名
    /// </summary>
    [Required(ErrorMessage = "姓名為必填")]
    public string FullName { get; set; } = string.Empty;

    /// <summary>
    /// 職稱
    /// </summary>
    [Required(ErrorMessage = "職稱為必填")]
    public string? JobTitle { get; set; }

    /// <summary>
    /// 電話
    /// </summary>
    [Required(ErrorMessage = "電話為必填")]
    [Phone(ErrorMessage = "電話格式不正確")]
    public string? Tel { get; set; }

    /// <summary>
    /// 註冊驗證完成
    /// </summary>
    [JsonIgnore]
    public bool IsRegisterVerify { get; set; }

    /// <summary>
    /// 驗證
    /// </summary>
    /// <returns>回傳驗證失敗的訊息集合</returns>
    public IEnumerable<string> Validate()
    {
        if (string.IsNullOrWhiteSpace(FullName))
        {
            yield return $"{nameof(FullName)} 不能為空白";
        }
        if (string.IsNullOrWhiteSpace(UserName))
        {
            yield return $"{nameof(UserName)} 不能為空白";
        }
        if (string.IsNullOrWhiteSpace(Email))
        {
            yield return $"{nameof(Email)} 不能為空白";
        }
        else if (!new EmailAddressAttribute().IsValid(Email))
        {
            yield return $"{nameof(Email)} 格式不正確";
        }
        if (string.IsNullOrWhiteSpace(JobTitle))
        {
            yield return $"{nameof(JobTitle)} 不能為空白";
        }
        if (string.IsNullOrWhiteSpace(Tel))
        {
            yield return $"{nameof(Tel)} 不能為空白";
        }
        else if (!new PhoneAttribute().IsValid(Tel))
        {
            yield return $"{nameof(Tel)} 格式不正確";
        }
    }
}