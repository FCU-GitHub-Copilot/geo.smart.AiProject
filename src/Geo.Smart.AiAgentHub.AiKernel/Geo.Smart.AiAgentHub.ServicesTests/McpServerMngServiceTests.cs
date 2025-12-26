using Geo.Smart.AiAgentHub.AiKernel.Vm;
using Geo.Smart.AiAgentHub.Entities;
using Geo.Smart.AiAgentHub.Infras.Enums;
using Geo.Smart.AiAgentHub.Services;
using Geo.Smart.CommonCore.Models;
using Microsoft.Extensions.Logging.Abstractions;

namespace Geo.Smart.AiAgentHub.ServicesTests;

/// <summary>
/// 測試 McpServerMngService 的 Query 方法
/// </summary>
[TestClass]
public class McpServerMngServiceTests
{
    /// <summary>
    /// 測試 Query 方法是否能正確取得所有啟用的 MCP Server 資料
    /// </summary>
    [TestMethod]
    public async Task Query_ShouldReturnEnabledMcpServers()
    {
        McpServerMngService service = GetService();

        // 查詢所有資料
        var param = new QueryBase
        {
            Keyword = null,
            CurrentPage = 1,
            PageSize = 10,
            Sorting = "Name"
        };

        var result = await service.Query(param);

        Assert.IsNotNull(result);
        Assert.IsTrue(result.Success);
        var data = result.Data.ToList();
        Assert.IsGreaterThan(0, data.Count);
        Assert.HasCount(2, data);
        Assert.IsTrue(data.Exists(x => x.Name == "Stdio_Server"));
        Assert.IsTrue(data.Exists(x => x.Name == "Sse_Server"));
    }

    private static McpServerMngService GetService()
    {
        var logger = NullLogger<CommonService>.Instance;
        var db = DbContextFactory.GetInMemoryDbContext();
        var service = new McpServerMngService(db, logger);
        return service;
    }

    /// <summary>
    /// 測試 Query 方法能依關鍵字正確篩選 MCP Server 資料
    /// </summary>
    [TestMethod]
    public async Task Query_WithKeyword_ShouldFilterMcpServers()
    {
        McpServerMngService service = GetService();

        // 關鍵字查詢
        var param = new QueryBase
        {
            Keyword = "Stdio",
            CurrentPage = 1,
            PageSize = 10,
            Sorting = "Name"
        };

        var result = await service.Query(param);
        Assert.IsNotNull(result);
        Assert.IsTrue(result.Success);
        var data = result.Data.ToList();

        Assert.HasCount(1, data);
        Assert.AreEqual("Stdio_Server", actual: data[0].Name);
    }

    /// <summary>
    /// 測試 Detail 方法能正確取得 MCP Server 詳細資料
    /// </summary>
    [TestMethod]
    public async Task Detail_ShouldReturnMcpServerDetail()
    {
        McpServerMngService service = GetService();

        // 取 Stdio Server 詳細資料
        var mcpServerId = Guid.Parse("10000000-AAAA-AAAA-AAAA-AAAAAAAAAAAA");
        var result = await service.Detail(mcpServerId);

        Assert.IsNotNull(result);
        Assert.IsTrue(result.Success);
        Assert.IsNotNull(result.Data);
        Assert.AreEqual(mcpServerId, result.Data.McpServerId);
        Assert.AreEqual("Stdio_Server", result.Data.Name);
        Assert.AreEqual("系統管理員", result.Data.UserName);
        Assert.IsNotNull(result.Data.Tools);
        Assert.IsGreaterThan(0, result.Data.Tools.Count);
        Assert.AreEqual("forecast", result.Data.Tools[0]);
    }

    /// <summary>
    /// 測試 Detail 方法查詢不存在的 MCP Server 時，應回傳失敗
    /// </summary>
    [TestMethod]
    public async Task Detail_ShouldReturnFailure_WhenNotFound()
    {
        McpServerMngService service = GetService();

        // 查詢不存在的 ID
        var mcpServerId = Guid.NewGuid();
        var result = await service.Detail(mcpServerId);

        Assert.IsNotNull(result);
        Assert.IsFalse(result.Success);
    }

    /// <summary>
    /// 測試 Create 方法遇到名稱為空時應回傳失敗
    /// </summary>
    [TestMethod]
    public async Task Create_ShouldReturnFailure_WhenNameIsEmpty()
    {
        McpServerMngService service = GetService();

        var vm = new McpServerVm
        {
            Name = "",
            McpServerType = McpServerType.Stdio,
            StdioCommand = "cmd",
            StdioArgs = ["--arg"],
            StdioEnv = new Dictionary<string, string?> { { "ENV", "Test" } }
        };

        var userId = DbContextFactory.AdminUserId;
        var result = await service.Create(vm, userId);

        Assert.IsNotNull(result);
        Assert.IsFalse(result.Success);
        Assert.Contains("必填", result.Message);
    }

    /// <summary>
    /// 測試 Delete 方法能成功刪除 MCP Server
    /// </summary>
    [TestMethod]
    public async Task Delete_ShouldDisableMcpServer()
    {
        var logger = NullLogger<CommonService>.Instance;
        var db = DbContextFactory.GetInMemoryDbContext();
        var service = new McpServerMngService(db, logger);

        var entity = db.McpServers.First(x => x.Name == "Stdio_Server");
        var userInfo = new UserInfo
        {
            UserId = DbContextFactory.AdminUserId,
            RoleId = DbContextFactory.AdminRoleId,
            Account = "GeoAdmin"
        };

        var result = await service.Delete(entity.McpServerId, userInfo);

        Assert.IsNotNull(result);
        Assert.IsTrue(result.Success);
        var deleted = db.McpServers.First(x => x.McpServerId == entity.McpServerId);
        Assert.IsFalse(deleted.IsEnabled);
    }

    /// <summary>
    /// 測試 Delete 方法遇到不存在的 MCP Server 時應回傳失敗
    /// </summary>
    [TestMethod]
    public async Task Delete_ShouldReturnFailure_WhenNotFound()
    {
        McpServerMngService service = GetService();

        var userInfo = new UserInfo
        {
            UserId = DbContextFactory.AdminUserId,
            Account = "GeoAdmin"
        };

        var result = await service.Delete(Guid.NewGuid(), userInfo);

        Assert.IsNotNull(result);
        Assert.IsFalse(result.Success);
        Assert.Contains("查無資料", result.Message);
    }

    /// <summary>
    /// 測試 Delete 方法遇到無權限時應回傳失敗
    /// </summary>
    [TestMethod]
    public async Task Delete_ShouldReturnFailure_WhenNoAuthority()
    {
        var logger = NullLogger<CommonService>.Instance;
        var db = DbContextFactory.GetInMemoryDbContext();
        var service = new McpServerMngService(db, logger);

        var entity = db.McpServers.First(x => x.Name == "Stdio_Server");
        var userInfo = new UserInfo
        {
            UserId = "other-user-id",
            Account = "OtherUser"
        };

        var result = await service.Delete(entity.McpServerId, userInfo);

        Assert.IsNotNull(result);
        Assert.IsFalse(result.Success);
        Assert.Contains("無權限", result.Message);
    }
}