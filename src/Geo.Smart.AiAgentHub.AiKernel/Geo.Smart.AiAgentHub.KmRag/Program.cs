using Geo.Smart.AiAgentHub.KmRag;
using Geo.Smart.AiAgentHub.KmRag.HttpFilters;
using Geo.Smart.AiAgentHub.KmRag.Internals;
using Geo.Smart.AiAgentHub.KmRag.Services;
using Geo.Smart.AiAgentHub.KmRag.Services.Contracts;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.KernelMemory;
using Microsoft.KernelMemory.AI;
using Microsoft.KernelMemory.Configuration;
using Microsoft.KernelMemory.Diagnostics;
using Microsoft.KernelMemory.DocumentStorage;
using Microsoft.KernelMemory.MemoryStorage;
using Microsoft.KernelMemory.Pipeline;
using Serilog;
using System.Globalization;

var s_start = DateTimeOffset.UtcNow;

SensitiveDataLogger.Enabled = false;

// *************************** APP BUILD *******************************

int asyncHandlersCount = 0;
int syncHandlersCount = 0;
string memoryType = string.Empty;

// Usual .NET web app builder with settings from appsettings.json, appsettings.<ENV>.json, and env vars
var builder = WebApplication.CreateBuilder();

// 配置 Serilog
builder.Host.UseSerilog((context, services, configuration) => configuration
    .ReadFrom.Configuration(context.Configuration)
    .ReadFrom.Services(services)
    .Enrich.FromLogContext());

// Add config files, user secretes, and env vars
builder.Configuration.AddKernelMemoryConfigurationSources();

// Read KM settings, needed before building the app.
KernelMemoryConfig config = builder.Configuration.GetSection("KernelMemory").Get<KernelMemoryConfig>()
                            ?? throw new ConfigurationException("Unable to load configuration");

// Some OpenAPI Explorer/Swagger dependencies
builder.ConfigureSwagger(config);

// Prepare memory builder, sharing the service collection used by the hosting service
// Internally build the memory client and make it available for dependency injection
builder.AddKernelMemory(memoryBuilder =>
{
    // Prepare the builder with settings from config files
    memoryBuilder.ConfigureDependencies(builder.Configuration).WithoutDefaultHandlers();

    // When using distributed orchestration, handlers are hosted in the current app and need to be con
    asyncHandlersCount = AddHandlersAsHostedServices(config, memoryBuilder, builder);
},
    memory =>
    {
        // When using in process orchestration, handlers are hosted by the memory orchestrator
        syncHandlersCount = AddHandlersToServerlessMemory(config, memory);

        memoryType = ((memory is MemoryServerless) ? "Sync - " : "Async - ") + memory.GetType().FullName;
    },
    services =>
    {
        // 註冊文件上傳服務
        services.AddScoped<IIngestionService, IngestionService>();
        
        // 註冊檢索服務
        services.AddScoped<IRetrievalService, RetrievalService>();

        long? maxSize = config.Service.GetMaxUploadSizeInBytes();
        if (!maxSize.HasValue) { return; }

        services.Configure<IISServerOptions>(x => { x.MaxRequestBodySize = maxSize.Value; });
        services.Configure<KestrelServerOptions>(x => { x.Limits.MaxRequestBodySize = maxSize.Value; });
        services.Configure<FormOptions>(x =>
        {
            x.MultipartBodyLengthLimit = maxSize.Value;
            x.ValueLengthLimit = int.MaxValue;
        });
    });

// CORS
bool enableCORS = false;
const string CORSPolicyName = "KM-CORS";
if (enableCORS && config.Service.RunWebService)
{
    builder.Services.AddCors(options =>
    {
        options.AddPolicy(name: CORSPolicyName, policy =>
        {
            policy
                .WithMethods("HEAD", "GET", "POST", "PUT", "DELETE")
                .WithExposedHeaders("Content-Type", "Content-Length", "Last-Modified");
            // .AllowAnyOrigin()
            // .WithOrigins(...)
            // .AllowAnyHeader()
            // .WithHeaders(...)
        });
    });
}

// Build .NET web app as usual
WebApplication app = builder.Build();

// 加入 Serilog 請求日誌中間層
app.UseSerilogRequestLogging(options =>
{
    options.MessageTemplate = "HTTP {RequestMethod} {RequestPath} responded {StatusCode} in {Elapsed:0.0000} ms";

    // 過濾不需要記錄的路徑
    options.GetLevel = (httpContext, elapsed, ex) =>
    {
        // 正常請求使用 Information 層級
        if (httpContext.Response.StatusCode >= 500)
        {
            return Serilog.Events.LogEventLevel.Error;
        }

        if (httpContext.Response.StatusCode >= 400)
        {
            return Serilog.Events.LogEventLevel.Warning;
        }

        return Serilog.Events.LogEventLevel.Information;
    };

    options.EnrichDiagnosticContext = (diagnosticContext, httpContext) =>
    {
        diagnosticContext.Set("RequestHost", httpContext.Request.Host.Value);
        diagnosticContext.Set("RequestScheme", httpContext.Request.Scheme);
        diagnosticContext.Set("RemoteIpAddress", httpContext.Connection.RemoteIpAddress);
    };
});

if (config.Service.RunWebService)
{
    if (enableCORS) { app.UseCors(CORSPolicyName); }

    app.UseSwagger(config);
    var errorFilter = new HttpErrorsEndpointFilter();
    var authFilter = new HttpAuthEndpointFilter(config.ServiceAuthorization);
    app.MapGet("/", () => Results.Ok("Ingestion service is running. " +
                                     "Uptime: " + (DateTimeOffset.UtcNow.ToUnixTimeSeconds()
                                                   - s_start.ToUnixTimeSeconds()) + " secs " +
                                     $"- Environment: {Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")}"))
        .AddEndpointFilter(errorFilter)
        .AddEndpointFilter(authFilter)
        .WithName("ServiceStatus")
        .WithDisplayName("ServiceStatus")
        .WithDescription("Show the service status and uptime.")
        .WithSummary("Show the service status and uptime.")
        .Produces<string>(StatusCodes.Status200OK)
        .Produces<ProblemDetails>(StatusCodes.Status401Unauthorized)
        .Produces<ProblemDetails>(StatusCodes.Status403Forbidden);

    // Add HTTP endpoints using minimal API (https://learn.microsoft.com/aspnet/core/fundamentals/minimal-apis)
    app.AddKernelMemoryEndpoints("/", config, [errorFilter, authFilter]);

    // Health probe
    app.MapGet("/health", () => Results.Ok("Service is running."))
        .WithName("ServiceHealth")
        .WithDisplayName("ServiceHealth")
        .WithDescription("Show if the service is healthy.")
        .WithSummary("Show if the service is healthy.")
        .Produces<string>(StatusCodes.Status200OK)
        .Produces<ProblemDetails>(StatusCodes.Status401Unauthorized)
        .Produces<ProblemDetails>(StatusCodes.Status403Forbidden);

    if (config.ServiceAuthorization.Enabled && config.ServiceAuthorization.AccessKey1 == config.ServiceAuthorization.AccessKey2)
    {
        app.Logger.LogError("KM Web Service: Access keys 1 and 2 have the same value. Keys should be different to allow rotation.");
    }
}

// *************************** START ***********************************

var env = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
if (string.IsNullOrEmpty(env))
{
    app.Logger.LogError("ASPNETCORE_ENVIRONMENT env var not defined.");
}

Console.WriteLine("***************************************************************************************************************************");
Console.WriteLine("* Environment         : " + (string.IsNullOrEmpty(env) ? "WARNING: ASPNETCORE_ENVIRONMENT env var not defined" : env));
Console.WriteLine("* Memory type         : " + memoryType);
Console.WriteLine("* Pipeline handlers   : " + $"{syncHandlersCount} synchronous / {asyncHandlersCount} asynchronous");
Console.WriteLine("* Web service         : " + (config.Service.RunWebService ? "Enabled" : "Disabled"));

if (config.Service.RunWebService)
{
    const double AspnetDefaultMaxUploadSize = 30000000d / 1024 / 1024;
    Console.WriteLine("* Web service auth    : " + (config.ServiceAuthorization.Enabled ? "Enabled" : "Disabled"));
    Console.WriteLine("* Max HTTP req size   : " + (config.Service.MaxUploadSizeMb ?? AspnetDefaultMaxUploadSize).ToString("0.#", CultureInfo.CurrentCulture) + " Mb");
    Console.WriteLine("* OpenAPI swagger     : " + (config.Service.OpenApiEnabled ? "Enabled (/swagger/index.html)" : "Disabled"));
}

Console.WriteLine("* Memory Db           : " + app.Services.GetService<IMemoryDb>()?.GetType().FullName);
Console.WriteLine("* Document storage    : " + app.Services.GetService<IDocumentStorage>()?.GetType().FullName);
Console.WriteLine("* Embedding generation: " + app.Services.GetService<ITextEmbeddingGenerator>()?.GetType().FullName);
Console.WriteLine("* Text generation     : " + app.Services.GetService<ITextGenerator>()?.GetType().FullName);
Console.WriteLine("* Content moderation  : " + app.Services.GetService<IContentModeration>()?.GetType().FullName);
Console.WriteLine("* Log level           : " + app.Logger.GetLogLevelName());
Console.WriteLine("***************************************************************************************************************************");

app.Logger.LogInformation(
    "Starting Kernel Memory service, .NET Env: {EnvironmentType}, Log Level: {LogLevel}, Web service: {WebServiceEnabled}, Auth: {WebServiceAuthEnabled}, Pipeline handlers: {HandlersEnabled}",
    env,
    app.Logger.GetLogLevelName(),
    config.Service.RunWebService,
    config.ServiceAuthorization.Enabled,
    config.Service.RunHandlers);

try
{
    // 寫入一筆到 SEQ 確認連線正常
    app.Logger.LogWarning("Kernel Memory RAG Server 完成啟動");
    app.Run();
}
catch (IOException e)
{
    Console.WriteLine($"I/O error: {e.Message}");
    Environment.Exit(-1);
}

/// <summary>
/// Register handlers as asynchronous hosted services
/// </summary>
static int AddHandlersAsHostedServices(
   KernelMemoryConfig config,
   IKernelMemoryBuilder memoryBuilder,
   WebApplicationBuilder appBuilder)
{
    if (!string.Equals(config.DataIngestion.OrchestrationType, KernelMemoryConfig.OrchestrationTypeDistributed, StringComparison.OrdinalIgnoreCase))
    {
        return 0;
    }

    if (!config.Service.RunHandlers) { return 0; }

    // Handlers are enabled via configuration in appsettings.json and/or appsettings.<env>.json
    memoryBuilder.WithoutDefaultHandlers();

    // You can add handlers in the configuration or manually here using one of these syntaxes:
    // appBuilder.Services.AddHandlerAsHostedService<...CLASS...>("...STEP NAME...");
    // appBuilder.Services.AddHandlerAsHostedService("...assembly file name...", "...type full name...", "...STEP NAME...");

    // Register all pipeline handlers defined in the configuration to run as hosted services
    foreach (KeyValuePair<string, HandlerConfig> handlerConfig in config.Service.Handlers)
    {
        appBuilder.Services.AddHandlerAsHostedService(config: handlerConfig.Value, stepName: handlerConfig.Key);
    }

    // Return registered handlers count
    return appBuilder.Services.Count(s => typeof(IPipelineStepHandler).IsAssignableFrom(s.ServiceType));
}

/// <summary>
/// Register handlers instances inside the synchronous orchestrator
/// </summary>
static int AddHandlersToServerlessMemory(
   KernelMemoryConfig config, IKernelMemory memory)
{
    if (memory is not MemoryServerless) { return 0; }

    var orchestrator = ((MemoryServerless)memory).Orchestrator;
    foreach (KeyValuePair<string, HandlerConfig> handlerConfig in config.Service.Handlers)
    {
        orchestrator.AddSynchronousHandler(handlerConfig.Value, handlerConfig.Key);
    }

    return orchestrator.HandlerNames.Count;
}