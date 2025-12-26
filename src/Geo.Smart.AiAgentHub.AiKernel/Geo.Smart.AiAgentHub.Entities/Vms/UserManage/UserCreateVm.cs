using System.ComponentModel.DataAnnotations;

namespace Geo.Smart.AiAgentHub.Entities.Vms.UserManage;

/// <summary>
/// 建立使用者 ViewModel
/// </summary>
public class UserCreateVm
{
    /// <summary>
    /// 帳號
    /// </summary>
    [Required(ErrorMessage = "帳號為必填")]
    public string UserName { get; set; } = string.Empty;

    /// <summary>
    /// 密碼
    /// </summary>
    [Required(ErrorMessage = "密碼為必填")]
    public string Qoo { get; set; } = string.Empty;

    /// <summary>
    /// 確認密碼
    /// </summary>
    [Compare("Qoo", ErrorMessage = "密碼與確認密碼不一致")]
    public string ConfirmQoo { get; set; } = string.Empty;

    /// <summary>
    /// 信箱
    /// </summary>
    [Required(ErrorMessage = "信箱為必填")]
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// 姓名
    /// </summary>
    [Required(ErrorMessage = "姓名為必填")]
    public string FullName { get; set; } = string.Empty;

    /// <summary>
    /// 性別 (1:男;0:女)
    /// </summary>
    public bool Gender { get; set; }

    /// <summary>
    /// 單位編號
    /// </summary>
    public Guid? OrgId { get; set; }

    /// <summary>
    /// 職稱
    /// </summary>
    [Required(ErrorMessage = "職稱為必填")]
    public string JobTitle { get; set; } = string.Empty;

    /// <summary>
    /// 連絡電話
    /// </summary>
    [Required(ErrorMessage = "電話為必填")]
    public string Tel { get; set; } = string.Empty;

    /// <summary>
    /// 分機
    /// </summary>
    public string? TelExt { get; set; }

    /// <summary>
    /// 角色Id
    /// </summary>
    [Required(ErrorMessage = "角色為必填")]
    public string RoleId { get; set; } = string.Empty;

    /// <summary>
    /// 帳號是否啟用
    /// </summary>
    public bool IsEnabled { get; set; }

    /// <summary>
    /// 最後變更密碼時間
    /// </summary>
    public DateTime LastChangeQoo { get; set; } = DateTime.Now;
}