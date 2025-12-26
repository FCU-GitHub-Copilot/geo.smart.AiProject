using Geo.Smart.AiAgentHub.Infras.Enums;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace Geo.Smart.AiAgentHub.WebApi.Filters;

/// <summary>
/// 使用者帳號異動歷程屬性，負責記錄使用者相關操作的歷程資訊
/// </summary>
[AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
public class UserHistoryAttribute : ActionFilterAttribute
{
    private string _userId;

    /// <summary>
    /// 使用者資料異動類型
    /// </summary>
    public UserHistoryType HistoryType { get; set; }

    /// <summary>
    /// 建構子，初始化 userId
    /// </summary>
    public UserHistoryAttribute()
    {
        _userId = string.Empty;
    }

    /// <summary>
    /// Action 執行前的紀錄
    /// </summary>
    /// <param name="context">Action 執行內容</param>
    /// <param name="next">下一個委派</param>
    /// <returns>非同步工作</returns>
    public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        if (HistoryType == UserHistoryType.登入系統
            && context.ActionArguments.TryGetValue("login", out object? value))
        {
            var model = value as LoginViewModel;
            await TryGetUserId(context, model?.UserName);
        }
        else if (HistoryType == UserHistoryType.登出系統)
        {
            // 取得並解析 Bearer token
            var token = GetToken(context);
            if (!string.IsNullOrEmpty(token))
            {
                var jwtToken = new JwtSecurityTokenHandler().ReadJwtToken(token);
                _userId = jwtToken.Claims.FirstOrDefault(c => c.Type == "nameid")?.Value ?? string.Empty;
            }
        }
        await next();
    }

    /// <summary>
    /// 嘗試取得使用者 Id
    /// </summary>
    /// <param name="context">Action 執行內容</param>
    /// <param name="userName">使用者名稱</param>
    /// <returns>非同步工作</returns>
    private async Task TryGetUserId(ActionExecutingContext context, string? userName)
    {
        var userManager = context.HttpContext.RequestServices.GetService<UserManager<ApplicationUser>>();
        if (userManager == null)
        {
            return;
        }
        if (string.IsNullOrEmpty(userName))
        {
            return;
        }
        var user = await userManager.FindByNameAsync(userName);
        if (user != null)
        {
            _userId = user.Id;
        }
    }

    /// <summary>
    /// Action 執行後的紀錄
    /// </summary>
    /// <param name="context">結果執行內容</param>
    /// <param name="next">下一個委派</param>
    /// <returns>非同步工作</returns>
    public override async Task OnResultExecutionAsync(ResultExecutingContext context, ResultExecutionDelegate next)
    {
        try
        {
            var dbContext = context.HttpContext.RequestServices.GetService<GdbContext>();
            if (dbContext == null)
            {
                await next();
                return;
            }
            await ReChackUserId(context, dbContext);
            if (string.IsNullOrEmpty(_userId))
            {
                await next();
                return;
            }
            UserHistory history;
            var ip = context.HttpContext.RequestServices.GetService<UserAgentHelper>()?.GetClientIp() ?? "-";

            if (HistoryType == UserHistoryType.登入系統)
            {
                history = GetLoginHistory(context, ip);
            }
            else
            {
                Result<object>? resultFromAction;
                if (context.Result is ObjectResult objResult && objResult.Value != null)
                {
                    resultFromAction = objResult.Value as Result<object>;
                }
                else
                {
                    await next();
                    return;
                }
                history = new UserHistory
                {
                    UserId = _userId,
                    LoginId = _userId,
                    RequestResult = resultFromAction?.Success ?? false,
                    UserHistoryType = HistoryType,
                    HistoryTypeName = HistoryType.ToString(),
                    Message = resultFromAction?.Message,
                    Ip = ip,
                };
            }
            if (history != null)
            {
                dbContext.UserHistories.Add(history);
                await dbContext.SaveChangesAsync();
            }
        }
        catch (Exception e)
        {
            var logger = context.HttpContext.RequestServices.GetService<ILogger<UserHistoryAttribute>>();
            logger?.LogError(e, "Error {Name}：{Message}", nameof(UserHistoryAttribute), e.Message);
        }

        await next();
    }

    private async Task ReChackUserId(ResultExecutingContext context, GdbContext dbContext)
    {
        if (string.IsNullOrEmpty(_userId))
        {
            _userId = context.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            if (string.IsNullOrEmpty(_userId)
                && context.HttpContext.User.Identity != null
                && context.HttpContext.User.Identity.IsAuthenticated)
            {
                var adId = context.HttpContext.User.Identity.Name!.Split('\\')[^1];
                var userId = await dbContext.Users!.AsNoTracking()
                    .Where(r => r.UserName == adId && r.IsEnabled)
                    .Select(x => x.Id)
                    .FirstOrDefaultAsync();
                if (!string.IsNullOrEmpty(userId))
                {
                    _userId = userId;
                }
            }
        }
    }

    private static string GetToken(ActionExecutingContext context)
    {
        var bearer = context.HttpContext.Request.Headers.Authorization;
        if (string.IsNullOrEmpty(bearer))
        {
            return string.Empty;
        }
        var split = bearer.ToString().Split(' ');
        if (split.Length <= 1)
        {
            return string.Empty;
        }
        return split[1];
    }

    /// <summary>
    /// 取得登入歷程資訊
    /// </summary>
    /// <param name="context">結果執行內容</param>
    /// <param name="ip"> IP </param>
    /// <returns>使用者歷程物件</returns>
    private UserHistory GetLoginHistory(ResultExecutingContext context,
        string ip)
    {
        UserHistory history = new()
        {
            UserId = _userId,
            LoginId = _userId,
            UserHistoryType = HistoryType,
            HistoryTypeName = HistoryType.ToString(),
            Message = string.Empty,
            Ip = ip,
        };
        if (context.Result is ObjectResult objectResult
            && objectResult.Value is Result<LoginResultVm> loginResult)
        {
            history.RequestResult = loginResult.Success;
            history.Message = loginResult.Message;
        }
        else
        {
            history.RequestResult = false;
            history.Message = "登入系統回傳的型態不符！";
        }

        return history;
    }
}