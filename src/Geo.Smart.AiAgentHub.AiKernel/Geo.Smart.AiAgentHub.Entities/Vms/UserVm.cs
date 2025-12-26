using System.Text.Json.Serialization;

namespace Geo.Smart.AiAgentHub.Entities.Vms;
/// <summary>
/// 個人資料
/// </summary>
public class UserVm
{
    /// <summary>
    /// 建構子
    /// </summary>
    public UserVm()
    {
        UserId = string.Empty;
        UserName = string.Empty;
        Email = string.Empty;
        FullName = string.Empty;
        Gender = true;
        IsRegisterVerify = false;
    }

    ///<summary>
    /// 使用者ID
    ///</summary>
    public string UserId { get; set; }

    /// <summary>
    /// 使用者 帳號
    /// </summary>
    public string? UserName { get; set; }

    /// <summary>
    /// Email
    /// </summary>
    public string? Email { get; set; }

    /// <summary>
    /// 姓名
    /// </summary>
    public string FullName { get; set; }

    ///<summary>
    /// 性別 1:男, 0:女
    ///</summary>
    public bool Gender { get; set; }

    /// <summary>
    /// 生日
    /// </summary>
    public DateTime? Birthday { get; set; }

    ///<summary>
    /// 職稱
    ///</summary>
    public string? JobTitle { get; set; }

    ///<summary>
    /// 聯絡電話
    ///</summary>
    public string? Tel { get; set; }

    /// <summary>
    /// 註冊驗證完成
    /// </summary>
    [JsonIgnore]
    public bool IsRegisterVerify { get; set; }

    /// <summary>
    /// 驗證
    /// </summary>
    /// <returns></returns>
    public IEnumerable<string> Validate()
    {
        if (string.IsNullOrWhiteSpace(FullName))
        {
            yield return $"{nameof(FullName)} Empty";
        }
    }
}