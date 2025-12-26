using Geo.Smart.AiAgentHub.DataAccess;
using Geo.Smart.AiAgentHub.DataAccess.Entities;
using Geo.Smart.AiAgentHub.Infras.Enums;
using Microsoft.EntityFrameworkCore;

namespace Geo.Smart.AiAgentHub.ServicesTests;

/// <summary>
/// 提供建立 InMemory GdbContext 的工廠類別
/// </summary>
public static class DbContextFactory
{
    public const string AdminUserId = "00e9847d-bd23-4e98-9a17-f5d18245494b";
    public const string AdminRoleId = "0b1aac62-10eb-4c16-9558-f86e9979c90b";

    /// <summary>
    /// 建立 InMemory DbContext
    /// </summary>
    /// <returns>回傳 GdbContext 實例</returns>
    public static GdbContext GetInMemoryDbContext()
    {
        var options = new DbContextOptionsBuilder<GdbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        var db = new GdbContext(options);

        AddUsers(db);
        AddLlmInfo(db);
        AddMcpServer(db);
        db.SaveChanges();
        AddAiProject(db);
        db.SaveChanges();

        return db;
    }

    /// <summary>
    /// 新增兩筆 AiProject 測試資料，並關聯 LlmInfo 與 McpServer
    /// </summary>
    /// <param name="db">資料庫內容物件</param>
    private static void AddAiProject(GdbContext db)
    {
        // 取得已存在的 LlmInfo 與 McpServer
        var llm = db.LlmInfoes.FirstOrDefault(x => x.LlmId == Guid.Parse("00000001-AAAA-AAAA-AAAA-AAAAAAAAAAAA"));
        var mcp1 = db.McpServers.FirstOrDefault(x => x.McpServerId == Guid.Parse("10000000-AAAA-AAAA-AAAA-AAAAAAAAAAAA"));
        var mcp2 = db.McpServers.FirstOrDefault(x => x.McpServerId == Guid.Parse("20000000-BBBB-BBBB-BBBB-BBBBBBBBBBBB"));

        var projectA = new AiProject
        {
            ProjectId = Guid.Parse("30000000-CCCC-CCCC-CCCC-CCCCCCCCCCCC"),
            Name = "測試專案 A",
            Description = "這是測試專案 A 的描述",
            SystemPrompt = "你是一個有用的助理",
            UserId = AdminUserId
        };
        if (llm != null)
        {
            projectA.LlmInfoes.Add(llm);
        }
        if (mcp1 != null)
        {
            projectA.McpServers.Add(mcp1);
        }
        db.AiProjects.Add(projectA);

        var projectB = new AiProject
        {
            ProjectId = Guid.Parse("40000000-DDDD-DDDD-DDDD-DDDDDDDDDDDD"),
            Name = "測試專案 B",
            Description = "這是測試專案 B 的描述",
            UserId = AdminUserId
        };
        if (llm != null)
        {
            projectB.LlmInfoes.Add(llm);
        }
        if (mcp2 != null)
        {
            projectB.McpServers.Add(mcp2);
        }
        db.AiProjects.Add(projectB);
    }

    /// <summary>
    /// 新增兩筆 McpServer 測試資料
    /// </summary>
    /// <param name="db">資料庫內容物件</param>
    private static void AddMcpServer(GdbContext db)
    {
        db.McpServers.Add(new McpServer
        {
            McpServerId = Guid.Parse("10000000-AAAA-AAAA-AAAA-AAAAAAAAAAAA"),
            Name = "Stdio_Server",
            McpServerType = McpServerType.Stdio,
            SseUrl = null,
            StdioCommand = "test-stdio-cmd",
            StdioArgs = "[\"--test\"]",
            StdioEnv = "{\"ENV\":\"My_TEST_KEY\"}",
            UserId = AdminUserId,
            Tools = "[\"forecast\"]",
            IsEnabled = true
        });

        db.McpServers.Add(new McpServer
        {
            McpServerId = Guid.Parse("20000000-BBBB-BBBB-BBBB-BBBBBBBBBBBB"),
            Name = "Sse_Server",
            McpServerType = McpServerType.Sse,
            SseUrl = "https://sse.test.com",
            StdioCommand = null,
            StdioArgs = null,
            StdioEnv = null,
            UserId = AdminUserId,
            IsEnabled = true
        });
    }

    /// <summary>
    /// 新增 GeoAdmin 使用者及角色
    /// </summary>
    /// <param name="db">資料庫內容物件</param>
    private static void AddUsers(GdbContext db)
    {
        var admin = new ApplicationUser
        {
            Id = AdminUserId,
            UserName = "GeoAdmin",
            NormalizedUserName = "GEOADMIN",
            Email = "joe@geo.com.tw",
            NormalizedEmail = "JOE@GEO.COM.TW",
            EmailConfirmed = true,
            PasswordHash = "AQAAAAIAAYagAAAAEG2vcmVVPLb0+N5hZ46eg1+d+vqmojHnjJMO/P3Mn9jJRB3gD3gVPUPo8ZREWjWtbA==",
            SecurityStamp = "YCGVSAKZMIOS3FXU3U4BV4327PW4DJVJ",
            ConcurrencyStamp = "e74dec50-273a-410e-8207-e4911208f494",
            PhoneNumber = "0987654321",
            PhoneNumberConfirmed = false,
            TwoFactorEnabled = false,
            LockoutEnd = new DateTime(2025, 7, 1, 3, 57, 24, 802, DateTimeKind.Utc).AddTicks(8038),
            LockoutEnabled = false,
            AccessFailedCount = 0,
            OrgId = null,
            CreatedDate = new DateTime(2021, 7, 3, 0, 10, 38, DateTimeKind.Utc),
            IsEnabled = true,
            FullName = "系統管理員",
            RegisterVerifyCode = null,
            IsRegisterVerify = true,
            IsForgotPwd = false,
            ForgotPwdVerifyCode = null,
            LastChangeQoo = new DateTime(2033, 2, 3, 16, 34, 6, 387, DateTimeKind.Utc),
            MustChangeQoo = false,
            Gender = true,
            Birthday = new DateTime(2023, 2, 3, 0, 0, 0, 0, DateTimeKind.Utc),
            JobTitle = "系統管理者",
            Tel = "0422985258",
            LastLogin = new DateTime(2025, 8, 29, 15, 8, 21, 910, DateTimeKind.Utc),
            LoginTimes = 1715,
            UseOtp = false,
            OtpSecret = "True",
            TelExt = "501",
            IsDelete = false,
            LoginType = LoginType.帳密登入
        };
        db.Users.Add(admin);

        var role = new ApplicationRole
        {
            Id = AdminRoleId,
            Name = "系統管理者",
            NormalizedName = "系統管理者",
            ConcurrencyStamp = "f18bddd3-01ec-4130-a8dc-0ac4277c8cbd",
        };
        db.ApplicationRoles.Add(role);
        admin.ApplicationRoles.Add(role);
    }

    /// <summary>
    /// 新增一筆 LlmInfo 測試資料
    /// </summary>
    /// <param name="db">資料庫內容物件</param>
    private static void AddLlmInfo(GdbContext db)
    {
        db.LlmInfoes.Add(new LlmInfo
        {
            LlmId = Guid.Parse("00000001-AAAA-AAAA-AAAA-AAAAAAAAAAAA"),
            ServiceId = "ServiceId-A-1",
            ModelId = "gpt-4o",
            LlmSourceType = LlmSourceType.OpenAi,
            ApiKey = "ServiceId-Akey",
            Endpoint = "https://test.com",
            DeploymentName = "openai-a-1",
            Description = "說明 ServiceId-A-1",
            IsEnabled = true,
            UserId = AdminUserId
        });
    }
}