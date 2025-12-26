using Geo.Smart.AiAgentHub.Entities.Vms.UserFootprint;

namespace Geo.Smart.AiAgentHub.Services.Contracts;
/// <summary>
/// 使用紀錄
/// </summary>
public interface IUserFootprintService
{
    /// <summary>
    /// 使用紀錄清單
    /// </summary>
    /// <param name="param"></param>
    /// <returns></returns>
    Task<PaginationResult<FullUserFootprintVm>> Query(UserFootprintQueryVm param);
}