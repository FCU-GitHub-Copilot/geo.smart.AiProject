using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace Geo.Smart.AiAgentHub.WebApi.Middlewares;

/// <summary>
/// 全域例外處理中介軟體，用於攔截並處理請求過程中發生的未處理例外。
/// </summary>
/// <param name="_next">下一個中介軟體的委派</param>
/// <param name="_logger">LOGGER 實體</param>
/// <param name="_hostEnvironmen">主機環境資訊</param>
public class ExceptionMiddleware(
    RequestDelegate _next,
    ILogger<ExceptionMiddleware> _logger,
    IHostEnvironment _hostEnvironmen)
{
    /// <summary>
    /// 處理 HTTP 請求，攔截並處理未處理的例外。
    /// </summary>
    /// <param name="context">HTTP 請求內容。</param>
    /// <returns>非同步作業。</returns>
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "捕捉到未處裡的異常 TraceId: {TraceId}",
                context.TraceIdentifier);

            await HandleExceptionAsync(context, ex);
        }
    }

    /// <summary>
    /// 處理例外並回傳標準化的 JSON 錯誤回應。
    /// </summary>
    /// <param name="context">HTTP 請求內容。</param>
    /// <param name="exception">捕捉到的例外。</param>
    /// <returns>非同步作業。</returns>
    private Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
        context.Response.ContentType = "application/problem+json";

        var problemDetails = new ProblemDetails
        {
            Status = StatusCodes.Status500InternalServerError,
            Title = "內部伺服器錯誤",
            Detail = _hostEnvironmen.IsDevelopment()
                ? exception.ToString()
                : "發生意外錯誤",
            Instance = context.Request.Path
        };

        problemDetails.Extensions["traceId"] = context.TraceIdentifier;
        return context.Response.WriteAsJsonAsync(problemDetails);
    }
}