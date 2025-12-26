using Geo.Smart.AiAgentHub.Entities.Vms.UserLog;

namespace Geo.Smart.AiAgentHub.Services.Contracts;

/// <summary>
/// 登入紀錄相關服務
/// </summary>
public interface IUserLogService
{
    /// <summary>
    /// 使用者登入紀錄清單
    /// </summary>
    /// <param name="param">使用者登入紀錄查詢條件</param>
    /// <returns></returns>
    Task<PaginationResult<UserLogVm>> Query(UserLogQueryVm param);
}