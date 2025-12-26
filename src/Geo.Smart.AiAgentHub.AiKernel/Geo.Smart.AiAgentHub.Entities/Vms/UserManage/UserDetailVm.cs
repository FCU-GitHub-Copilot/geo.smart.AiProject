namespace Geo.Smart.AiAgentHub.Entities.Vms.UserManage;

/// <summary>
/// 使用者詳細資訊 ViewModel
/// </summary>
public class UserDetailVm
{
    /// <summary>
    /// 使用者 Id
    /// </summary>
    public string UserId { get; set; } = string.Empty;

    /// <summary>
    /// 帳號
    /// </summary>
    public string? UserName { get; set; }

    /// <summary>
    /// Email
    /// </summary>
    public string? Email { get; set; }

    /// <summary>
    /// 姓名
    /// </summary>
    public string FullName { get; set; } = string.Empty;

    /// <summary>
    /// 連絡電話
    /// </summary>
    public string? Tel { get; set; }

    /// <summary>
    /// 分機
    /// </summary>
    public string? TelExt { get; set; }

    /// <summary>
    /// 職稱
    /// </summary>
    public string? JobTitle { get; set; }

    /// <summary>
    /// 使用角色
    /// </summary>
    public string? RoleName { get; set; }

    /// <summary>
    /// 性別 (1:男;0:女)
    /// </summary>
    public bool Gender { get; set; } = true;

    /// <summary>
    /// 單位 Id
    /// </summary>
    public Guid? OrgId { get; set; }

    /// <summary>
    /// 單位
    /// </summary>
    public string? OrgName { get; set; }

    /// <summary>
    /// 帳號狀態
    /// </summary>
    public bool IsEnabled { get; set; } = true;

    /// <summary>
    /// 上次密碼變更時間
    /// </summary>
    public DateTime LastChangeQoo { get; set; } = DateTime.Now;

    /// <summary>
    /// 角色ID
    /// </summary>
    public string? RoleId { get; set; }

    /// <summary>
    /// 密碼是否需要變更
    /// </summary>
    public bool? IsNeedQooChange { get; set; }

    /// <summary>
    /// 還剩幾天密碼到期(三個月)
    /// </summary>
    public int? RemainQooChange { get; set; }
}