using Geo.Smart.AiAgentHub.KmRag.Internals;
using Microsoft.KernelMemory;
using Microsoft.OpenApi.Models;

namespace Geo.Smart.AiAgentHub.KmRag.Internals;

/// <summary>
/// OpenAPI 設定工具類別
/// </summary>
internal static class OpenAPI
{
    /// <summary>
    /// 設定 Swagger 文件產生器
    /// </summary>
    /// <param name="appBuilder">Web 應用程式建置器</param>
    /// <param name="config">核心記憶體設定</param>
    public static void ConfigureSwagger(this WebApplicationBuilder appBuilder, KernelMemoryConfig config)
    {
        if (!config.Service.RunWebService || !config.Service.OpenApiEnabled)
        {
            return;
        }

        appBuilder.Services.AddEndpointsApiExplorer();

        // Note: this call is required even if service auth is disabled
        appBuilder.Services.AddSwaggerGen(c =>
        {
            if (!config.ServiceAuthorization.Enabled) { return; }

            const string ReqName = "auth";
            c.AddSecurityDefinition(ReqName, new OpenApiSecurityScheme
            {
                Description = "The API key to access the API",
                Type = SecuritySchemeType.ApiKey,
                Scheme = "ApiKeyScheme",
                Name = config.ServiceAuthorization.HttpHeaderName,
                In = ParameterLocation.Header,
            });

            var scheme = new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Id = ReqName,
                    Type = ReferenceType.SecurityScheme,
                },
                In = ParameterLocation.Header
            };

            var requirement = new OpenApiSecurityRequirement
            {
                { scheme, new List<string>() }
            };

            c.AddSecurityRequirement(requirement);
        });
    }

    /// <summary>
    /// 啟用 Swagger 中介軟體
    /// </summary>
    /// <param name="app">Web 應用程式</param>
    /// <param name="config">核心記憶體設定</param>
    public static void UseSwagger(this WebApplication app, KernelMemoryConfig config)
    {
        if (!config.Service.RunWebService || !config.Service.OpenApiEnabled)
        {
            return;
        }

        // URL: http://localhost:9001/swagger/index.html
        app.UseSwagger();
        app.UseSwaggerUI();
    }
}