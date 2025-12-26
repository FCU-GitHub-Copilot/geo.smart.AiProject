namespace Geo.Smart.AiAgentHub.WebApi.Middlewares;

/// <summary>
/// 使用者操作歷程自動記錄 - 註冊用
/// </summary>
public static class UserFootprintMiddlewareExtension
{
    /// <summary>
    /// 註冊 使用者操作歷程
    /// </summary>
    /// <param name="builder"></param>
    /// <returns></returns>
    public static IApplicationBuilder UseUserFootprint(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<UserFootprintMiddleware>();
    }
}