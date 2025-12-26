using Geo.Smart.AiAgentHub.Entities.Vms.UserLog;
using Geo.Smart.AiAgentHub.Services.Common;
using Geo.Smart.AiAgentHub.Services.Contracts;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Geo.Smart.AiAgentHub.Services;

/// <summary>
/// 提供使用者登入紀錄相關服務
/// </summary>
public class UserLogService(GdbContext dbModel,
    ILogger<CommonService> _logger
    ) : BaseService(dbModel, _logger), IUserLogService
{
    /// <summary>
    /// 取得使用者登入紀錄清單（支援關鍵字與日期區間查詢）
    /// </summary>
    /// <param name="param">查詢條件（含關鍵字、開始日期、結束日期、分頁、排序等）</param>
    /// <returns>分頁的使用者登入紀錄清單</returns>
    public async Task<PaginationResult<UserLogVm>> Query(UserLogQueryVm param)
    {
        try
        {
            if (param.StartDateTime.HasValue && param.EndDateTime.HasValue && param.EndDateTime < param.StartDateTime)
            {
                return ResultHelper.PaginationFailure<UserLogVm>("結束時間不能早於開始時間");
            }

            var loginQuery = DbModel.UserHistories.AsNoTracking()
                .Where(x => x.UserHistoryType == Infras.Enums.UserHistoryType.登入系統);

            if (param.StartDateTime.HasValue)
            {
                loginQuery = loginQuery.Where(x => x.RequestTime >= param.StartDateTime.Value);
            }
            if (param.EndDateTime.HasValue)
            {
                loginQuery = loginQuery.Where(x => x.RequestTime <= param.EndDateTime.Value);
            }
            if (param.LoginStatus.HasValue)
            {
                loginQuery = loginQuery.Where(x => x.RequestResult == param.LoginStatus.Value);
            }

            var query = loginQuery.Join(DbModel.ApplicationUsers.AsNoTracking(),
                        userHistory => userHistory.UserId, user => user.Id,
                        (userHistory, user) => new { userHistory, user })
                        .Where(x => x.user.IsEnabled)
                        .Select(x => new UserLogVm
                        {
                            UserName = x.user.UserName,
                            FullName = x.user.FullName,
                            Ip = x.userHistory.Ip,
                            IsRequestResult = x.userHistory.RequestResult,
                            RequestTime = x.userHistory.RequestTime,
                        })
                .WhereIf(!string.IsNullOrEmpty(param.Keyword),
                log => log.UserName!.Contains(param.Keyword) ||
                (log.FullName != null && log.FullName.Contains(param.Keyword)));

            return await ResultHelper.PaginationSuccessAsync(query, param);
        }
        catch (Exception ex)
        {
            LogError(ex, nameof(Query));
            return ResultHelper.PaginationFailure<UserLogVm>(ex.Message);
        }
    }
}