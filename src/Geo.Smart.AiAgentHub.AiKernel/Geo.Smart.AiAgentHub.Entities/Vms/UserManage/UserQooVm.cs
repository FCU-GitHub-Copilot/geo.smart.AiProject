using System.ComponentModel.DataAnnotations;

namespace Geo.Smart.AiAgentHub.Entities.Vms.UserManage;

/// <summary>
/// 使用者密碼
/// </summary>
public class UserQooVm
{
    /// <summary>
    /// 使用者 Id
    /// </summary>
    public string UserId { get; set; } = string.Empty;

    /// <summary>原始密碼</summary>
    [Required(ErrorMessage = "原始密碼為必填")]
    public string OldQoo { get; set; } = default!;

    /// <summary>
    /// 新密碼
    /// </summary>
    [Required(ErrorMessage = "新密碼為必填")]
    public string NewQoo { get; set; } = default!;

    /// <summary>
    /// 再次確認新密碼
    /// </summary>
    [Required(ErrorMessage = "請再次輸入新密碼")]
    [Compare("NewQoo", ErrorMessage = "新密碼輸入不一致")]
    public string ConfirmQoo { get; set; } = default!;
}