using Geo.Smart.AiAgentHub.DataAccess;
using Geo.Smart.AiAgentHub.DataAccess.Entities;
using Geo.Smart.AiAgentHub.Entities;
using Geo.Smart.AiAgentHub.Entities.Vms.AiAgent;
using Geo.Smart.AiAgentHub.Services;
using Geo.Smart.CommonCore.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace Geo.Smart.AiAgentHub.ServicesTests;

[TestClass()]
public class ProjectMngServiceTests
{
    /// <summary>
    /// 建立 ProjectMngService 的輔助方法，並提供必要的 ProjectSettingVm 選項
    /// </summary>
    /// <param name="db">資料庫上下文</param>
    private static ProjectMngService CreateService(GdbContext db)
    {
        var logger = NullLogger<CommonService>.Instance;
        return new ProjectMngService(db, logger);
    }

    /// <summary>
    /// 測試 Query 方法能正確取得所有啟用的 AI 專案資料
    /// </summary>
    [TestMethod]
    public async Task Query_ShouldReturnEnabledProjects()
    {
        var db = DbContextFactory.GetInMemoryDbContext();
        var service = CreateService(db);

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
        Assert.IsTrue(data.Exists(x => x.Name == "測試專案 B"));
    }

    /// <summary>
    /// 測試 Query 方法能依關鍵字正確篩選 AI 專案資料
    /// </summary>
    [TestMethod]
    public async Task Query_WithKeyword_ShouldFilterProjects()
    {
        var db = DbContextFactory.GetInMemoryDbContext();
        var service = CreateService(db);

        var param = new QueryBase
        {
            Keyword = "B",
            CurrentPage = 1,
            PageSize = 10,
            Sorting = "Name"
        };

        var result = await service.Query(param);

        Assert.IsNotNull(result);
        Assert.IsTrue(result.Success);
        var data = result.Data.ToList();
        Assert.HasCount(1, data);
        Assert.AreEqual("測試專案 B", data[0].Name);
    }

    /// <summary>
    /// 測試 Create 方法能成功新增 AI 專案
    /// </summary>
    [TestMethod]
    public async Task Create_ShouldAddNewProject()
    {
        var db = DbContextFactory.GetInMemoryDbContext();
        var service = CreateService(db);

        var llmId = db.LlmInfoes.First().LlmId;
        var mcpId = db.McpServers.First().McpServerId;

        var vm = new AiProjectCreateVm
        {
            Name = "新增專案",
            Description = "新增專案描述",
            SystemPrompt = "新的系統提示詞",
            LlmIds = [llmId],
            McpServerIds = [mcpId]
        };

        var userId = DbContextFactory.AdminUserId;
        var result = await service.Create(vm, userId);

        Assert.IsNotNull(result);
        Assert.IsTrue(result.Success);
        Assert.IsFalse(string.IsNullOrWhiteSpace(result.Data));

        var entity = db.AiProjects.FirstOrDefault(x => x.Name == "新增專案");
        Assert.IsNotNull(entity);
        Assert.AreEqual(userId, entity.UserId);
        Assert.AreEqual("新增專案描述", entity.Description);
        Assert.IsTrue(entity.LlmInfoes.Any(l => l.LlmId == llmId));
        Assert.IsTrue(entity.McpServers.Any(m => m.McpServerId == mcpId));
    }

    /// <summary>
    /// 測試 Create 方法遇到重複名稱時應回傳失敗
    /// </summary>
    [TestMethod]
    public async Task Create_ShouldReturnFailure_WhenNameExists()
    {
        var db = DbContextFactory.GetInMemoryDbContext();
        var service = CreateService(db);

        var llmId = db.LlmInfoes.First().LlmId;
        var mcpId = db.McpServers.First().McpServerId;

        var vm = new AiProjectCreateVm
        {
            Name = "測試專案 A", // 已存在
            Description = "重複名稱",
            SystemPrompt = "新的系統提示詞",
            LlmIds = [llmId],
            McpServerIds = [mcpId]
        };

        var userId = DbContextFactory.AdminUserId;
        var result = await service.Create(vm, userId);

        Assert.IsNotNull(result);
        Assert.IsFalse(result.Success);
        Assert.Contains("已存在", result.Message);
    }

    /// <summary>
    /// 測試 Detail 方法能正確取得 AI 專案詳細資料
    /// </summary>
    [TestMethod]
    public async Task Detail_ShouldReturnProjectDetail()
    {
        var db = DbContextFactory.GetInMemoryDbContext();
        var service = CreateService(db);

        var projectId = Guid.Parse("30000000-CCCC-CCCC-CCCC-CCCCCCCCCCCC");
        var result = await service.Detail(projectId);

        Assert.IsNotNull(result);
        Assert.IsTrue(result.Success);
        Assert.IsNotNull(result.Data);
        Assert.AreEqual(projectId, result.Data.ProjectId);
        Assert.AreEqual("測試專案 A", result.Data.Name);
        Assert.AreEqual("系統管理員", result.Data.Owner);
        Assert.IsGreaterThan(0, result.Data.LlmInfos.Count);
        Assert.IsGreaterThan(0, result.Data.McpServers.Count);
    }

    /// <summary>
    /// 測試 Detail 方法查詢不存在的專案時，應回傳失敗
    /// </summary>
    [TestMethod]
    public async Task Detail_ShouldReturnFailure_WhenNotFound()
    {
        var db = DbContextFactory.GetInMemoryDbContext();
        var service = CreateService(db);

        var projectId = Guid.NewGuid();
        var result = await service.Detail(projectId);

        Assert.IsNotNull(result);
        Assert.IsFalse(result.Success);
    }

    /// <summary>
    /// 測試 Update 方法能成功更新 AI 專案資料
    /// </summary>
    [TestMethod]
    public async Task Update_ShouldUpdateProject()
    {
        var db = DbContextFactory.GetInMemoryDbContext();
        var service = CreateService(db);

        var origin = db.AiProjects.First(x => x.Name == "測試專案 A");
        var llmId = db.LlmInfoes.First().LlmId;
        var mcpId = db.McpServers.First().McpServerId;

        var vm = new AiProjectUpdateVm
        {
            ProjectId = origin.ProjectId,
            Name = "更新後專案",
            Description = "更新後描述",
            SystemPrompt = "新的系統提示詞",
            LlmIds = [llmId],
            McpServerIds = [mcpId]
        };

        var result = await service.Update(vm);

        Assert.IsNotNull(result);
        Assert.IsTrue(result.Success);
        Assert.AreEqual(origin.ProjectId.ToString(), result.Data);

        var updated = db.AiProjects.First(x => x.ProjectId == origin.ProjectId);
        Assert.AreEqual("更新後專案", updated.Name);
        Assert.AreEqual("更新後描述", updated.Description);
        Assert.AreEqual("新的系統提示詞", updated.SystemPrompt);
        Assert.IsTrue(updated.LlmInfoes.Any(l => l.LlmId == llmId));
        Assert.IsTrue(updated.McpServers.Any(m => m.McpServerId == mcpId));
    }

    /// <summary>
    /// 測試 Update 方法遇到重複名稱時應回傳失敗
    /// </summary>
    [TestMethod]
    public async Task Update_ShouldReturnFailure_WhenNameExists()
    {
        var db = DbContextFactory.GetInMemoryDbContext();
        var service = CreateService(db);

        var projectA = db.AiProjects.First(x => x.Name == "測試專案 A");
        var projectB = db.AiProjects.First(x => x.Name == "測試專案 B");
        var llmId = db.LlmInfoes.First().LlmId;
        var mcpId = db.McpServers.First().McpServerId;

        var vm = new AiProjectUpdateVm
        {
            ProjectId = projectA.ProjectId,
            Name = projectB.Name, // 使用另一筆已存在的名稱
            Description = "重複名稱",
            SystemPrompt = "新的系統提示詞",
            LlmIds = [llmId],
            McpServerIds = [mcpId]
        };

        var result = await service.Update(vm);

        Assert.IsNotNull(result);
        Assert.IsFalse(result.Success);
        Assert.Contains("已經被使用", result.Message);
    }

    /// <summary>
    /// 測試 Update 方法遇到不存在的專案時應回傳失敗
    /// </summary>
    [TestMethod]
    public async Task Update_ShouldReturnFailure_WhenNotFound()
    {
        var db = DbContextFactory.GetInMemoryDbContext();
        var service = CreateService(db);

        var llmId = db.LlmInfoes.First().LlmId;
        var mcpId = db.McpServers.First().McpServerId;

        var vm = new AiProjectUpdateVm
        {
            ProjectId = Guid.NewGuid(),
            Name = "不存在的專案",
            Description = "不存在",
            SystemPrompt = "不存在的系統提示詞",
            LlmIds = [llmId],
            McpServerIds = [mcpId]
        };

        var result = await service.Update(vm);

        Assert.IsNotNull(result);
        Assert.IsFalse(result.Success);
        Assert.Contains("查無資料", result.Message);
    }

    /// <summary>
    /// 測試 Delete 方法能成功刪除 AI 專案
    /// </summary>
    [TestMethod]
    public async Task Delete_ShouldDisableProject()
    {
        var db = DbContextFactory.GetInMemoryDbContext();
        var service = CreateService(db);

        var entity = db.AiProjects.First(x => x.Name == "測試專案 A");
        var userInfo = new UserInfo
        {
            UserId = DbContextFactory.AdminUserId,
            RoleId = DbContextFactory.AdminRoleId,
            Account = "GeoAdmin"
        };

        var result = await service.Delete(entity.ProjectId, userInfo);

        Assert.IsNotNull(result);
        Assert.IsTrue(result.Success);
        var deleted = db.AiProjects.First(x => x.ProjectId == entity.ProjectId);
        Assert.IsFalse(deleted.IsEnabled);
    }

    /// <summary>
    /// 測試 Delete 方法遇到不存在的專案時應回傳失敗
    /// </summary>
    [TestMethod]
    public async Task Delete_ShouldReturnFailure_WhenNotFound()
    {
        var db = DbContextFactory.GetInMemoryDbContext();
        var service = CreateService(db);

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
        var db = DbContextFactory.GetInMemoryDbContext();
        var service = CreateService(db);

        var entity = db.AiProjects.First(x => x.Name == "測試專案 A");
        var userInfo = new UserInfo
        {
            UserId = "other-user-id",
            Account = "OtherUser"
        };

        var result = await service.Delete(entity.ProjectId, userInfo);

        Assert.IsNotNull(result);
        Assert.IsFalse(result.Success);
        Assert.Contains("無權限", result.Message);
    }

    /// <summary>
    /// 測試 Delete 方法在使用者為資料擁有者（非管理者）時應回傳成功
    /// </summary>
    [TestMethod]
    public async Task Delete_ShouldSuccess_WhenOwnerNotAdmin()
    {
        var db = DbContextFactory.GetInMemoryDbContext();
        var service = CreateService(db);

        var nonAdminUserId = "non-admin-user-id";
        var nonAdminProject = new AiProject
        {
            ProjectId = Guid.NewGuid(),
            Name = "非管理者專案",
            SystemPrompt = "你是 AI 助手",
            UserId = nonAdminUserId,
            IsEnabled = true
        };
        db.AiProjects.Add(nonAdminProject);
        await db.SaveChangesAsync();

        var userInfo = new UserInfo
        {
            UserId = nonAdminUserId,
            Account = "NonAdminUser"
        };

        var result = await service.Delete(nonAdminProject.ProjectId, userInfo);

        Assert.IsNotNull(result);
        Assert.IsTrue(result.Success);
        var deleted = db.AiProjects.First(x => x.ProjectId == nonAdminProject.ProjectId);
        Assert.IsFalse(deleted.IsEnabled);
    }

    /// <summary>
    /// 測試 Delete 方法在使用者為管理者但非擁有者時應回傳成功
    /// </summary>
    [TestMethod]
    public async Task Delete_ShouldSuccess_WhenAdminNotOwner()
    {
        var db = DbContextFactory.GetInMemoryDbContext();
        var service = CreateService(db);

        var anotherUserId = "another-user-id";
        var anotherProject = new AiProject
        {
            ProjectId = Guid.NewGuid(),
            Name = "他人的專案",
            SystemPrompt = "你是 AI 助手",
            UserId = anotherUserId,
            IsEnabled = true
        };
        db.AiProjects.Add(anotherProject);
        await db.SaveChangesAsync();

        var adminUserInfo = new UserInfo
        {
            UserId = DbContextFactory.AdminUserId,
            RoleId = DbContextFactory.AdminRoleId,
            Account = "GeoAdmin"
        };

        var result = await service.Delete(anotherProject.ProjectId, adminUserInfo);

        Assert.IsNotNull(result);
        Assert.IsTrue(result.Success);
        var deleted = db.AiProjects.First(x => x.ProjectId == anotherProject.ProjectId);
        Assert.IsFalse(deleted.IsEnabled);
    }
}