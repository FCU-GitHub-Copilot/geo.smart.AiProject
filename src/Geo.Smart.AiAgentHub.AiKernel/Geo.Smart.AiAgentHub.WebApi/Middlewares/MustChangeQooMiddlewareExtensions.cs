namespace Geo.Smart.AiAgentHub.WebApi.Middlewares;

/// <summary>
/// 帳號強制變更密碼檢核 - 註冊用
/// </summary>
public static class MustChangeQooMiddlewareExtensions
{
    /// <summary>
    /// 註冊 帳號強制變更密碼檢核
    /// </summary>
    /// <param name="builder"></param>
    /// <returns></returns>
    public static IApplicationBuilder UseMustChangeQoo(
        this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<MustChangeQooMiddleware>();
    }
}