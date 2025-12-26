using System.ComponentModel.DataAnnotations;

namespace Geo.Smart.AiAgentHub.Entities.Vms.UserManage;

/// <summary>
/// 更新使用者 ViewModel
/// </summary>
public class UserUpdateVm
{
    /// <summary>
    /// 使用者 Id
    /// </summary>
    [Required(ErrorMessage = "使用者Id為必填")]
    public string UserId { get; set; } = string.Empty;

    /// <summary>
    /// 帳號
    /// </summary>
    [Required(ErrorMessage = "帳號為必填")]
    public string UserName { get; set; } = string.Empty;

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
    /// 角色Id
    /// </summary>
    [Required(ErrorMessage = "角色為必填")]
    public string RoleId { get; set; } = string.Empty;

    /// <summary>
    /// 性別 (1:男;0:女)
    /// </summary>
    public bool Gender { get; set; } = true;

    /// <summary>
    /// 單位編號
    /// </summary>
    public Guid? OrgId { get; set; }

    /// <summary>
    /// 分機
    /// </summary>
    public string? TelExt { get; set; }

    /// <summary>
    /// 帳號是否啟用
    /// </summary>
    public bool IsEnabled { get; set; } = true;
}