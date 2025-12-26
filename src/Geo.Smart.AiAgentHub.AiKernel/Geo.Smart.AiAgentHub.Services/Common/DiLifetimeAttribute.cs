using Microsoft.Extensions.DependencyInjection;

namespace Geo.Smart.AiAgentHub.Services.Common;
/// <summary>
/// 標示 DI 生命週期的 Attribute
/// </summary>
[AttributeUsage(AttributeTargets.Class)]
public class DiLifetimeAttribute : Attribute
{
    /// <summary>
    /// 取得 DI 生命週期
    /// </summary>
    public ServiceLifetime Lifetime { get; }

    /// <summary>
    /// 建立 DiLifetimeAttribute 實例
    /// </summary>
    /// <param name="lifetime">DI 生命週期</param>
    public DiLifetimeAttribute(ServiceLifetime lifetime)
    {
        Lifetime = lifetime;
    }
}