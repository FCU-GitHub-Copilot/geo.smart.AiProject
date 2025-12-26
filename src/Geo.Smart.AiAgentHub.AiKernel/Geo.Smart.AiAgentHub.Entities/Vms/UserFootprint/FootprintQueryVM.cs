using Geo.Smart.CommonCore.Models;

namespace Geo.Smart.AiAgentHub.Entities.Vms.UserFootprint;
/// <summary>
/// 使用者軌跡查詢條件 ViewModel
/// </summary>
public class UserFootprintQueryVm : QueryBase
{
    /// <summary>
    /// 查詢起始日期
    /// </summary>
    public DateTime? StartDate { get; set; }

    /// <summary>
    /// 查詢結束日期
    /// </summary>
    public DateTime? EndDate { get; set; }
}