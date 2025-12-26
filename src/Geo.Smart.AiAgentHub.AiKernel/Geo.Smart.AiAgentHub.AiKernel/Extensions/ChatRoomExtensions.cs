using Geo.Smart.AiAgentHub.AiKernel.Middlewares;
using Geo.Smart.AiAgentHub.AiKernel.Models;
using Geo.Smart.AiAgentHub.AiKernel.Services;
using Geo.Smart.AiAgentHub.AiKernel.Services.Contracts;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Geo.Smart.AiAgentHub.AiKernel.Extensions;

/// <summary>
/// 聊天室擴充方法
/// </summary>
public static class ChatRoomExtensions
{
    /// <summary>
    /// 註冊聊天室相關服務
    /// </summary>
    /// <param name="services">服務集合</param>
    /// <param name="connectionString">資料庫連線字串</param>
    /// <returns>服務集合</returns>
    public static IServiceCollection AddChatRoomServices(
        this IServiceCollection services,
        string connectionString)
    {
        services.AddDbContext<AiHubContext>((serviceProvider, options) =>
        {
            var httpContextAccessor = serviceProvider.GetRequiredService<IHttpContextAccessor>();
            options.UseSqlServer(connectionString);
        });

        services.AddScoped<IChatRoomService, ChatRoomService>();

        return services;
    }

    /// <summary>
    /// 使用聊天室中介軟體
    /// </summary>
    /// <param name="app">應用程式建構器</param>
    /// <returns>應用程式建構器</returns>
    public static IApplicationBuilder UseChatRoom(this IApplicationBuilder app)
    {
        return app.UseMiddleware<ChatRoomMiddleware>();
    }
}