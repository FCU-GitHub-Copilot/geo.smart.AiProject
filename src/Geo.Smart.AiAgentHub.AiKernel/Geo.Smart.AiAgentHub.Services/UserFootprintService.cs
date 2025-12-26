using Geo.Smart.AiAgentHub.Entities.Vms.UserFootprint;
using Geo.Smart.AiAgentHub.Services.Common;
using Geo.Smart.AiAgentHub.Services.Contracts;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Geo.Smart.AiAgentHub.Services;

/// <summary>
/// 提供查詢使用者操作軌跡的相關功能
/// </summary>
public class UserFootprintService(GdbContext dbModel,
    ILogger<CommonService> _logger
    ) : BaseService(dbModel, _logger), IUserFootprintService
{
    /// <summary>
    /// 取得使用者軌跡紀錄清單（分頁）
    /// </summary>
    /// <param name="param">查詢條件（含日期區間、分頁、排序等）</param>
    /// <returns>分頁的使用者軌跡紀錄清單</returns>
    public async Task<PaginationResult<FullUserFootprintVm>> Query(UserFootprintQueryVm param)
    {
        try
        {
            var dbQuery = DbModel.UserFootprints.AsNoTracking()
                .Include(x => x.ApplicationUser)
                .Where(x => x.ApplicationUser.IsEnabled && x.LogType == "FE")
                .Select(x => new FullUserFootprintVm
                {
                    UserId = x.UserId,
                    UserName = x.ApplicationUser.UserName,
                    FullName = x.ApplicationUser.FullName,
                    PageName = x.PageName,
                    Ip = x.Ip,
                    Browser = x.Browser,
                    Os = x.Os,
                    Url = x.Url,
                    RequestTime = x.RequestTime
                });

            if (param.StartDate.HasValue && param.EndDate.HasValue)
            {
                if (param.EndDate.Value < param.StartDate.Value)
                {
                    return ResultHelper.PaginationFailure<FullUserFootprintVm>("結束日期不能早於開始日期");
                }
                dbQuery = dbQuery.Where(x =>
                    x.RequestTime >= param.StartDate.Value &&
                    x.RequestTime < param.EndDate.Value.AddDays(1)
                );
            }
            else if (param.StartDate.HasValue)
            {
                dbQuery = dbQuery.Where(x => x.RequestTime >= param.StartDate.Value);
            }
            else if (param.EndDate.HasValue)
            {
                dbQuery = dbQuery.Where(x => x.RequestTime < param.EndDate.Value.AddDays(1));
            }
            return await ResultHelper.PaginationSuccessAsync(dbQuery, param);
        }
        catch (Exception ex)
        {
            LogError(ex, nameof(Query));
            return ResultHelper.PaginationFailure<FullUserFootprintVm>(ex.Message);
        }
    }
}