using Geo.Smart.AiAgentHub.AiKernel.Services.Contracts;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Geo.Smart.AiAgentHub.AiKernel.Middlewares;

/// <summary>
/// 聊天室中介軟體
/// </summary>
public class ChatRoomMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ChatRoomMiddleware> _logger;

    /// <summary>
    /// 建構式
    /// </summary>
    /// <param name="next">下一個中介軟體</param>
    /// <param name="logger">日誌記錄器</param>
    public ChatRoomMiddleware(RequestDelegate next, ILogger<ChatRoomMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    /// <summary>
    /// 執行中介軟體邏輯
    /// </summary>
    /// <param name="context">HTTP 上下文</param>
    /// <param name="chatRoomService">聊天室服務</param>
    public async Task InvokeAsync(HttpContext context, IChatRoomService chatRoomService)
    {
        try
        {
            // 這裡可以加入聊天室相關的前置處理邏輯
            // 例如：驗證聊天室權限、記錄聊天室訪問等

            await _next(context);

            // 這裡可以加入聊天室相關的後置處理邏輯
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "ChatRoomMiddleware 發生錯誤：{Message}", ex.Message);
            throw;
        }
    }
}