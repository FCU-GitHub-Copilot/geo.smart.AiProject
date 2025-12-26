using Geo.Smart.CommonCore.Models;

namespace Geo.Smart.AiAgentHub.Entities.Vms.UserLog;
/// <summary>
/// 使用者登入紀錄查詢條件 ViewModel
/// </summary>
public class UserLogQueryVm : QueryBase
{
    /// <summary>
    /// 開始時間（含日期與時分）
    /// </summary>
    public DateTime? StartDateTime { get; set; }

    /// <summary>
    /// 結束時間（含日期與時分）
    /// </summary>
    public DateTime? EndDateTime { get; set; }

    /// <summary>
    /// 登入狀態（true=成功, false=失敗）
    /// </summary>
    public bool? LoginStatus { get; set; }
}