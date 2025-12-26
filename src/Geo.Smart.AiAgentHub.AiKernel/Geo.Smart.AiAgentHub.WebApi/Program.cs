using Geo.Smart.AiAgentHub.AiKernel.Extensions;
using Geo.Smart.AiAgentHub.AiKernel.Vm;
using Geo.Smart.AiAgentHub.WebApi.Middlewares;
using Geo.Smart.FileManagerCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using NetTopologySuite.IO.Converters;
using Serilog;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

// 設定 Serilog
builder.Host.UseSerilog((context, services, configuration) => configuration
    .ReadFrom.Configuration(context.Configuration)
    .ReadFrom.Services(services)
    .Enrich.FromLogContext());

builder.Services.AddHttpContextAccessor();
builder.Services.AddMemoryCache();

builder.Services.Configure<ConnectionSettings>(builder.Configuration.GetSection("ConnectionStrings"));
builder.Services.Configure<ProjectSettingVm>(builder.Configuration.GetSection("AiHubProject"));

// 連線字串，需要先解密
var connectionString = ConnectionHelper.GetDecrypt(
    builder.Configuration.GetConnectionString("GdbConnection") ?? string.Empty
);
var storageCs = ConnectionHelper.GetDecrypt(
    builder.Configuration.GetConnectionString("StorageConnection") ?? string.Empty
);

//var connectionBuilder = new SqlConnectionStringBuilder(connectionString);

builder.Services.AddDbContext<GdbContext>(options =>
    options.UseSqlServer(connectionString, x => x.UseNetTopologySuite()));

// 註冊 ChatRoom 相關服務
builder.Services.AddChatRoomServices(connectionString);

// SmtpMailSender 獨立註冊
builder.Services.AddScoped<IEmailSender, SmtpMailSender>();
// 註冊 Services 與 Helpers
var serviceAssembly = typeof(CommonService).Assembly;
DiServiceLifetime(serviceAssembly);
DiHelperLifetime(serviceAssembly);
// 註冊 FileManagerCore 模組
DiFileManagerLifetime(connectionString, storageCs);

builder.Services.AddIdentity<ApplicationUser, ApplicationRole>(options =>
{
    options.SignIn.RequireConfirmedAccount = true;
    options.SignIn.RequireConfirmedEmail = false;
    options.SignIn.RequireConfirmedPhoneNumber = false;

    options.Password.RequiredLength = 8;
    options.Password.RequireUppercase = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireDigit = true;
    options.Password.RequireNonAlphanumeric = true;
    options.Password.RequiredUniqueChars = 1;

    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(15);
    options.Lockout.MaxFailedAccessAttempts = 3;
    options.Lockout.AllowedForNewUsers = false;

    options.User.AllowedUserNameCharacters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";
    options.User.RequireUniqueEmail = false;
})
    .AddEntityFrameworkStores<GdbContext>()
    .AddClaimsPrincipalFactory<QooUserClaimsPrincipalFactory>()
    .AddDefaultTokenProviders();

builder.Services.AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        // 當驗證失敗時，回應標頭會包含 WWW-Authenticate 標頭，這裡會顯示失敗的詳細錯誤原因
        options.IncludeErrorDetails = true; // 預設值為 true，有時會特別關閉
        var jwtSection = builder.Configuration.GetSection("JwtSettings").Get<JwtSetting>();
        options.TokenValidationParameters = JwtHelper.GetTokenValidationParameters(jwtSection ?? new JwtSetting());
    });

builder.Services.AddAuthorization();

builder.Services
    .AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new GeoJsonConverterFactory());
    });

// NLog: Setup NLog for Dependency injection
builder.Logging.ClearProviders();

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Version = "v1",
        Title = "SMART AI Agent Hub",
        Description = @"",
    });

    // 設定所有 Geo.Smart.*.xml 說明檔案
    var xmlFiles = Directory.GetFiles(AppContext.BaseDirectory, "Geo.Smart.*.xml");
    foreach (var xmlFile in xmlFiles)
    {
        options.IncludeXmlComments(xmlFile, includeControllerXmlComments: true);
    }

    var jwtSecurityScheme = new OpenApiSecurityScheme
    {
        Scheme = "bearer",
        BearerFormat = "JWT",
        Name = "JWT Authentication",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Description = "輸入 **_JWT_** token，用來測試需要驗證的 API!",

        Reference = new OpenApiReference
        {
            Id = JwtBearerDefaults.AuthenticationScheme,
            Type = ReferenceType.SecurityScheme
        }
    };

    options.AddSecurityDefinition(jwtSecurityScheme.Reference.Id, jwtSecurityScheme);
    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        { jwtSecurityScheme, Array.Empty<string>() }
    });
});

var app = builder.Build();

// 加入 Serilog 的 HTTP 請求記錄
app.UseSerilogRequestLogging();

app.UseMiddleware<ExceptionMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
else
{
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();
app.UseMustChangeQoo();
app.UseUserFootprint();
app.UseChatRoom();

app.MapControllers();

// 寫入一筆 LOG
app.Logger.LogWarning("Geo.Smart.AiAgentHub.WebApi 完成啟動");

await app.RunAsync();

/* ==== 以下為整理後的私有方法 ==== */

// 自動註冊 Services 專案的所有服務
// 命名空間下所有以 Service 結尾且有對應介面的類別
void DiServiceLifetime(Assembly serviceAssembly)
{
    var serviceTypes = serviceAssembly.GetTypes()
        .Where(t => t.IsClass && !t.IsAbstract && t.Name.EndsWith("Service"))
        .Select(t => new
        {
            ServiceType = t.GetInterfaces().FirstOrDefault(i =>
                i.Name == $"I{t.Name}"
            ),
            ImplementationType = t
        })
        .Where(x => x.ServiceType != null);

    foreach (var type in serviceTypes)
    {
        builder.Services.AddScoped(type.ServiceType!, type.ImplementationType);
    }
}

// 自動註冊 Services 專案的所有 Helper
// 依據 DiLifetimeAttribute 決定生命週期
void DiHelperLifetime(Assembly serviceAssembly)
{
    var helperTypes = serviceAssembly.GetTypes()
        .Where(t => t.IsClass && !t.IsAbstract && t.Name.EndsWith("Helper"));

    foreach (var type in helperTypes)
    {
        var attr = type.GetCustomAttribute<DiLifetimeAttribute>();
        // 預設 Transient
        var lifetime = attr?.Lifetime ?? ServiceLifetime.Transient;

        switch (lifetime)
        {
            case ServiceLifetime.Singleton:
                builder.Services.AddSingleton(type);
                break;

            case ServiceLifetime.Scoped:
                builder.Services.AddScoped(type);
                break;

            case ServiceLifetime.Transient:
                builder.Services.AddTransient(type);
                break;
        }
    }
}

// 註冊 FileManagerCore 模組
void DiFileManagerLifetime(string connectionString, string storageCs)
{
    // 註冊 圖片上傳
    builder.Services.AddScoped(x =>
        new PhotoManager(connectionString, storageCs,
            x.GetRequiredService<IHttpContextAccessor>()
        )
    );
    // 註冊 檔案上傳
    builder.Services.AddScoped(x =>
        new FilesManager(connectionString, storageCs,
            x.GetRequiredService<IHttpContextAccessor>()
        )
    );
}