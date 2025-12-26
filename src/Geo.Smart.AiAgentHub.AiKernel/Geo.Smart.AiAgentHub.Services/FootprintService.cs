using Geo.Smart.AiAgentHub.DataAccess.Entities;
using Geo.Smart.AiAgentHub.Services.Common;
using Geo.Smart.AiAgentHub.Services.Contracts;
using Geo.Smart.AiAgentHub.Services.Helpers;
using Microsoft.Extensions.Logging;

namespace Geo.Smart.AiAgentHub.Services;

/// <summary>
/// 提供使用者頁面軌跡記錄的服務
/// </summary>
public class FootprintService(
    GdbContext dbModel, 
    UserAgentHelper userAgentHelper,
    ILogger<FootprintService> logger)
    : BaseService(dbModel, logger), IFootprintService
{
    private readonly UserAgentHelper _userAgentHelper = userAgentHelper;

    /// <summary>
    /// 使用者頁面軌跡，前端呼叫
    /// </summary>
    /// <param name="vm">使用者頁面軌跡的檢視模型</param>
    /// <param name="userId">使用者的唯一識別碼</param>
    /// <returns>操作結果，包含成功或失敗的訊息</returns>
    public Result<string> Frontend(UserFootprintVm vm, string userId)
    {
        try
        {
            var userAgent = _userAgentHelper.GetUserAgentInfo();

            // 建立前端足跡記錄
            var footprint = new UserFootprint
            {
                UserId = userId,
                Auth = string.Empty, // 前端呼叫時帳號資訊可從 userId 關聯取得
                LogType = "FE",
                Url = vm.Url,
                HttpVerb = "GET", // 前端頁面瀏覽通常為 GET
                QueryString = string.Empty,
                PostBody = string.Empty,
                UserAgent = userAgent.UserAgent,
                Ip = userAgent.Ip,
                RequestTime = DateTime.Now,
                Controller = null,
                Action = null,
                PageName = vm.PageName,
                Browser = userAgent.Browser,
                Os = userAgent.Os
            };

            DbModel.UserFootprints.Add(footprint);
            DbModel.SaveChanges();

            return ResultHelper.Success(string.Empty);
        }
        catch (Exception e)
        {
            LogError(e, nameof(Frontend));
            return ResultHelper.Failure<string>(e.Message);
        }
    }
}