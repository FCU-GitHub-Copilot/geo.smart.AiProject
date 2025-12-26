using Geo.Smart.AiAgentHub.Services.Extension;
using System.Security.Claims;
using System.Web;

namespace Geo.Smart.AiAgentHub.WebApi.Middlewares;
/// <summary>
/// 帳號強制變更密碼檢核
/// </summary>
public class MustChangeQooMiddleware
{
    private readonly RequestDelegate _next;

    /// <summary>
    /// 建構式
    /// </summary>
    /// <param name="next"></param>
    public MustChangeQooMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    /// <summary>
    /// 執行 middleware
    /// </summary>
    /// <param name="context"></param>
    /// <returns></returns>
    public async Task InvokeAsync(HttpContext context)
    {
        if (context.User.Identity != null
            && context.User.Identity.IsAuthenticated
            && ((ClaimsIdentity)context.User.Identity)
                .HasClaim(c => c.Type == ClaimTypesExt.MustChangeQooClaimType))
        {
            var claim = context.User.FindFirstValue(ClaimTypesExt.MustChangeQooClaimType);
            if (claim != null && claim.Equals("True", StringComparison.OrdinalIgnoreCase))
            {
                var returnUrl = context.Request.Path.Value == "/"
                    ? ""
                    : $"?returnUrl={HttpUtility.UrlEncode(context.Request.Path.Value)}";
                context.Response.Redirect($"/account/mustchangeqoo{returnUrl}");
            }
        }
        await _next(context).ConfigureAwait(true);
    }
}