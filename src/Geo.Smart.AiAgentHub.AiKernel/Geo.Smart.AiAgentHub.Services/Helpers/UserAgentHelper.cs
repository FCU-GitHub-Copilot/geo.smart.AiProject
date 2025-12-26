using Geo.Smart.AiAgentHub.Services.Common;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace Geo.Smart.AiAgentHub.Services.Helpers;
/// <summary>
/// 使用者代理資訊小助手
/// 取得 使用者IP、瀏覽器資訊 等
/// </summary>
/// <param name="_httpContextAccessor"></param>
[DiLifetime(ServiceLifetime.Scoped)]
public class UserAgentHelper(IHttpContextAccessor _httpContextAccessor)
{
    /// <summary>
    /// 抓取ClientIp
    /// </summary>
    /// <returns></returns>
    public string GetClientIp()
    {
        var ip = "unknow";
        if (_httpContextAccessor.HttpContext == null)
        {
            return ip;
        }
        try
        {
            var request = _httpContextAccessor.HttpContext.Request;

            if (request.Headers.TryGetValue("X-Forwarded-For", out var ipValue))
            {
                ip = ipValue;
            }
            else if (request.HttpContext.Items.ContainsKey("MS_HttpContext"))
            {
                dynamic ctx = request.HttpContext.Items["MS_HttpContext"]!;
                if (ctx != null)
                {
                    ip = ctx.Request.UserHostAddress;
                }
            }
            else if (!string.IsNullOrEmpty(request.HttpContext.Connection.RemoteIpAddress?.ToString()))
            {
                ip = request.HttpContext.Connection.RemoteIpAddress.ToString();
            }

            return ip ?? string.Empty;
        }
        catch
        {
            return ip!;
        }
    }

    /// <summary>
    /// 取得使用者代理資訊
    /// </summary>
    public UserAgentVm GetUserAgentInfo()
    {
        var vm = new UserAgentVm
        {
            Ip = GetClientIp(),
        };
        if (_httpContextAccessor.HttpContext == null)
        {
            return vm;
        }
        if (_httpContextAccessor.HttpContext.Request.Headers.TryGetValue("User-Agent", out var ua))
        {
            vm.UserAgent = ua.ToString();
            var clientInfo = UAParser.Parser.GetDefault().Parse(ua);
            if (clientInfo == null)
            {
                return vm;
            }
            vm.Browser = clientInfo.UA.ToString();
            vm.Os = clientInfo.OS.ToString();
        }
        return vm;
    }
}