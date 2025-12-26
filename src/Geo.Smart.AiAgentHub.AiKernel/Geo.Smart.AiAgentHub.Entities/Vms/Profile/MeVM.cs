using System.Text.Json.Serialization;

namespace Geo.Smart.AiAgentHub.Entities.Vms.Profile;

/// <summary>
/// 個人資料顯示用 ViewModel
/// </summary>
public class MeVm
{
    /// <summary>
    /// 使用者ID
    /// </summary>
    public string UserId { get; set; } = string.Empty;

    /// <summary>
    /// 帳號
    /// </summary>
    public string? UserName { get; set; }

    /// <summary>
    /// 信箱
    /// </summary>
    public string? Email { get; set; }

    /// <summary>
    /// 姓名
    /// </summary>
    public string FullName { get; set; } = string.Empty;

    /// <summary>
    /// 職稱
    /// </summary>
    public string? JobTitle { get; set; }

    /// <summary>
    /// 單位
    /// </summary>
    public string? OrgName { get; set; }

    /// <summary>
    /// 電話
    /// </summary>
    public string? Tel { get; set; }

    /// <summary>
    /// 註冊驗證完成
    /// </summary>
    [JsonIgnore]
    public bool IsRegisterVerify { get; set; } = false;
}