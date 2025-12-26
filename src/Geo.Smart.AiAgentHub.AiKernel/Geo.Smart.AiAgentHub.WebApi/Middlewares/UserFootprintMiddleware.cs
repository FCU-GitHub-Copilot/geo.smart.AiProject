using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Geo.Smart.AiAgentHub.DataAccess;
using Geo.Smart.AiAgentHub.DataAccess.Entities;

namespace Geo.Smart.AiAgentHub.WebApi.Middlewares;

/// <summary>
/// 使用者操作歷程自動記錄
/// </summary>
/// <param name="_next">下一個中介軟體的委派</param>
/// <param name="_logger">LOGGER 實體</param>
/// <param name="_serviceScopeFactory">服務範圍工廠</param>
public class UserFootprintMiddleware(
    RequestDelegate _next,
    ILogger<UserFootprintMiddleware> _logger,
    IServiceScopeFactory _serviceScopeFactory)
{
    /// <summary>
    /// 執行 middleware
    /// </summary>
    /// <param name="context"></param>
    /// <returns></returns>
    public async Task InvokeAsync(HttpContext context)
    {
        //未登入則不紀錄
        if (context.User.Identity != null
            && context.User.Identity.IsAuthenticated)
        {
            await WriteLogAsync(context);
        }

        await _next(context);
    }

    /// <summary>
    /// 寫入資料庫
    /// </summary>
    /// <param name="context"></param>
    private async Task WriteLogAsync(HttpContext context)
    {
        try
        {
            var actionInfo = GetActionInfo(context);
            if (actionInfo == null)
            {
                return;
            }

            // 排除 footprint controller 自身的記錄
            if (!string.IsNullOrEmpty(actionInfo.ControllerName)
                && actionInfo.ControllerName.Equals("footprint", StringComparison.CurrentCultureIgnoreCase))
            {
                return;
            }

            using var scope = _serviceScopeFactory.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<GdbContext>();

            // 取得使用者 ID
            var userId = context.User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value ?? string.Empty;
            var userName = context.User.Identity?.Name ?? string.Empty;

            // 取得 IP
            var ip = context.Connection.RemoteIpAddress?.ToString() ?? string.Empty;

            // 取得 UserAgent
            var userAgent = context.Request.Headers.UserAgent.ToString();

            // 建立足跡記錄
            var footprint = new UserFootprint
            {
                UserId = userId,
                Auth = userName,
                LogType = actionInfo.IsApi ? "API" : "MVC",
                Url = $"{context.Request.Path}{context.Request.QueryString}",
                HttpVerb = context.Request.Method,
                QueryString = context.Request.QueryString.ToString(),
                PostBody = string.Empty, // 可根據需求讀取 Request Body
                UserAgent = userAgent,
                Ip = ip,
                RequestTime = DateTime.Now,
                Controller = actionInfo.ControllerName,
                Action = actionInfo.ActionName,
                PageName = $"{actionInfo.ControllerName}/{actionInfo.ActionName}",
                Browser = GetBrowserInfo(userAgent),
                Os = GetOsInfo(userAgent)
            };

            dbContext.UserFootprints.Add(footprint);
            await dbContext.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            // 記錄錯誤但不影響正常請求流程
            _logger.LogError(ex, "寫入使用者足跡失敗");
        }
    }

    /// <summary>
    /// 判斷為 MVC or API
    /// </summary>
    /// <param name="context"></param>
    /// <returns></returns>
    private static ActionInfo? GetActionInfo(HttpContext context)
    {
        var controllerActionDescriptor = context
           .GetEndpoint()?
           .Metadata
           .GetMetadata<ControllerActionDescriptor>();
        if (controllerActionDescriptor == null)
        {
            return null;
        }

        var controllerName = controllerActionDescriptor.ControllerName;
        var actionName = controllerActionDescriptor.ActionName;
        var attribute = controllerActionDescriptor
            .ControllerTypeInfo
            .CustomAttributes
            .FirstOrDefault(c => c.AttributeType == typeof(ApiControllerAttribute));

        return new ActionInfo
        {
            IsApi = attribute != null,
            ActionName = actionName,
            ControllerName = controllerName,
        };
    }

    /// <summary>
    /// 從 UserAgent 提取瀏覽器資訊
    /// </summary>
    /// <param name="userAgent"></param>
    /// <returns></returns>
    private static string? GetBrowserInfo(string userAgent)
    {
        if (string.IsNullOrEmpty(userAgent))
        {
            return null;
        }

        if (userAgent.Contains("Edg/"))
        {
            return "Edge";
        }
        if (userAgent.Contains("Chrome/"))
        {
            return "Chrome";
        }
        if (userAgent.Contains("Firefox/"))
        {
            return "Firefox";
        }
        if (userAgent.Contains("Safari/") && !userAgent.Contains("Chrome/"))
        {
            return "Safari";
        }

        return "Other";
    }

    /// <summary>
    /// 從 UserAgent 提取作業系統資訊
    /// </summary>
    /// <param name="userAgent"></param>
    /// <returns></returns>
    private static string? GetOsInfo(string userAgent)
    {
        if (string.IsNullOrEmpty(userAgent))
        {
            return null;
        }

        if (userAgent.Contains("Windows NT"))
        {
            return "Windows";
        }
        if (userAgent.Contains("Mac OS X"))
        {
            return "macOS";
        }
        if (userAgent.Contains("Linux"))
        {
            return "Linux";
        }
        if (userAgent.Contains("Android"))
        {
            return "Android";
        }
        if (userAgent.Contains("iPhone") || userAgent.Contains("iPad"))
        {
            return "iOS";
        }

        return "Other";
    }

    private sealed class ActionInfo
    {
        public bool IsApi { get; set; }
        public string? ActionName { get; set; }
        public string? ControllerName { get; set; }
    }
}