namespace Geo.Smart.AiAgentHub.Entities.Vms.UserManage;

/// <summary>
/// 使用者列表 VM
/// </summary>
public class UserListVm
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
    /// 姓名
    /// </summary>
    public string FullName { get; set; } = string.Empty;

    /// <summary>
    /// 組織 Id
    /// </summary>
    public Guid? OrgId { get; set; }

    /// <summary>
    /// 單位
    /// </summary>
    public string? OrgName { get; set; }

    /// <summary>
    /// 電子郵件
    /// </summary>
    public string? Email { get; set; }

    /// <summary>
    /// 權限
    /// </summary>
    public string? RoleName { get; set; }

    /// <summary>
    /// 上次登入時間
    /// </summary>
    public DateTime? LastLogin { get; set; }

    /// <summary>
    /// 狀態
    /// </summary>
    public bool IsEnabled { get; set; }
}