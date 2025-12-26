namespace Geo.Smart.AiAgentHub.Entities.Vms.UserManage;

/// <summary>
/// 使用紀錄資料
/// </summary>
public class FullUserFootprintVm
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
    /// 姓名
    /// </summary>
    public string FullName { get; set; } = string.Empty;

    /// <summary>
    /// 操作項目
    /// </summary>
    public string? PageName { get; set; }

    /// <summary>
    /// IP
    /// </summary>
    public string Ip { get; set; } = string.Empty;

    /// <summary>
    /// 瀏覽器版本
    /// </summary>
    public string? Browser { get; set; }

    /// <summary>
    /// 作業平台
    /// </summary>
    public string? Os { get; set; }

    /// <summary>
    /// 操作連結
    /// </summary>
    public string Url { get; set; } = string.Empty;

    /// <summary>
    /// 操作時間
    /// </summary>
    public DateTime RequestTime { get; set; } = DateTime.Now;
}