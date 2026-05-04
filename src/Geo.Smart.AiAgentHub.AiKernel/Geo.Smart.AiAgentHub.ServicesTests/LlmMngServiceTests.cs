using Geo.Smart.AiAgentHub.DataAccess.Entities;
using Geo.Smart.AiAgentHub.Entities;
using Geo.Smart.AiAgentHub.Entities.Vms.AiAgent;
using Geo.Smart.AiAgentHub.Infras;
using Geo.Smart.AiAgentHub.Infras.Enums;
using Geo.Smart.AiAgentHub.ServicesTests;
using Geo.Smart.CommonCore.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;

namespace Geo.Smart.AiAgentHub.Services.Tests;

/// <summary>
/// LLM 管理服務測試
/// </summary>
[TestClass()]
public class LlmMngServiceTests
{
    /// <summary>
    /// 測試：ServiceId 已存在時應回傳失敗
    /// </summary>
    [TestMethod()]
    public async Task Create_ShouldFail_WhenServiceIdExists()
    {
        var db = DbContextFactory.GetInMemoryDbContext();
        db.LlmInfoes.Add(new LlmInfo
        {
            LlmId = Guid.NewGuid(),
            ServiceId = "sid1",
            IsEnabled = true,
            UserId = "user1",
            ModelId = "gpt-4o"
        });
        await db.SaveChangesAsync();
        var logger = NullLogger<CommonService>.Instance;
        var service = new LlmMngService(db, logger);
        var vm = new LlmCreateVm
        {
            ServiceId = "sid1",
            ModelId = "gpt-4o",
            LlmSourceType = LlmSourceType.OpenAi,
            ApiKey = "key"
        };
        var result = await service.Create(vm, "user1");
        Assert.IsFalse(result.Success);
        Assert.AreEqual("服務識別碼 已存在", result.Message);
    }

    /// <summary>
    /// 測試：缺少 ApiKey 時應回傳失敗
    /// </summary>
    [TestMethod()]
    public async Task Create_ShouldFail_WhenApiKeyRequired()
    {
        var db = DbContextFactory.GetInMemoryDbContext();
        var logger = NullLogger<CommonService>.Instance;
        var service = new LlmMngService(db, logger);
        var vm = new LlmCreateVm
        {
            ServiceId = "sid2",
            ModelId = "gpt-4o",
            LlmSourceType = LlmSourceType.OpenAi,
            ApiKey = null
        };
        var result = await service.Create(vm, "user2");
        Assert.IsFalse(result.Success);
        Assert.AreEqual("API 金鑰 為必填", result.Message);
    }

    /// <summary>
    /// 測試：缺少 Endpoint 時應回傳失敗
    /// </summary>
    [TestMethod()]
    public async Task Create_ShouldFail_WhenEndpointRequired()
    {
        var db = DbContextFactory.GetInMemoryDbContext();
        var logger = NullLogger<CommonService>.Instance;
        var service = new LlmMngService(db, logger);
        var vm = new LlmCreateVm
        {
            ServiceId = "sid3",
            ModelId = "gpt-4o",
            LlmSourceType = LlmSourceType.AzureOpenAi,
            ApiKey = "key",
            Endpoint = null
        };
        var result = await service.Create(vm, "user3");
        Assert.IsFalse(result.Success);
        Assert.AreEqual("端點網址 為必填", result.Message);
    }

    /// <summary>
    /// 測試：缺少 DeploymentName 時應回傳失敗
    /// </summary>
    [TestMethod()]
    public async Task Create_ShouldFail_WhenDeploymentNameRequired()
    {
        var db = DbContextFactory.GetInMemoryDbContext();
        var logger = NullLogger<CommonService>.Instance;
        var service = new LlmMngService(db, logger);
        var vm = new LlmCreateVm
        {
            ServiceId = "sid4",
            ModelId = "gpt-4o",
            LlmSourceType = LlmSourceType.AzureOpenAi,
            ApiKey = "key",
            Endpoint = "https://test"
            // DeploymentName 缺漏
        };
        var result = await service.Create(vm, "user4");
        Assert.IsFalse(result.Success);
        Assert.AreEqual("部署名稱 為必填", result.Message);
    }

    /// <summary>
    /// 測試：資料正確時應回傳成功
    /// </summary>
    [TestMethod()]
    public async Task Create_ShouldSuccess_WhenValid()
    {
        // InMemory Database 不會有資料表關聯檢核
        // 所以 user5 可以不存在於 [AspNetUsers] 中
        var db = DbContextFactory.GetInMemoryDbContext();
        var logger = NullLogger<CommonService>.Instance;
        var service = new LlmMngService(db, logger);
        var vm = new LlmCreateVm
        {
            ServiceId = "sid5",
            ModelId = "gpt-4o",
            LlmSourceType = LlmSourceType.OpenAi,
            ApiKey = "key"
        };
        var result = await service.Create(vm, "user5");
        Assert.IsTrue(result.Success);
        Assert.IsFalse(string.IsNullOrWhiteSpace(result.Data));
        var entity = await db.LlmInfoes.FirstOrDefaultAsync(x => x.LlmId.ToString() == result.Data);
        Assert.IsNotNull(entity);
        Assert.AreEqual("user5", entity.UserId);
        Assert.AreEqual("sid5", entity.ServiceId);
    }

    /// <summary>
    /// 測試：Query 關鍵字查詢應正確過濾資料
    /// </summary>
    [TestMethod()]
    public async Task Query_ShouldFilterByKeyword()
    {
        var db = DbContextFactory.GetInMemoryDbContext();
        db.LlmInfoes.Add(new LlmInfo
        {
            LlmId = Guid.NewGuid(),
            ServiceId = "sidA",
            ModelId = "gpt-4o",
            LlmSourceType = LlmSourceType.OpenAi,
            Description = "測試A",
            IsEnabled = true,
            UserId = "userA"
        });
        db.LlmInfoes.Add(new LlmInfo
        {
            LlmId = Guid.NewGuid(),
            ServiceId = "sidB",
            ModelId = "gpt-3.5",
            LlmSourceType = LlmSourceType.OpenAi,
            Description = "測試B",
            IsEnabled = true,
            UserId = "userB"
        });
        db.LlmInfoes.Add(new LlmInfo
        {
            LlmId = Guid.NewGuid(),
            ServiceId = "sidC",
            ModelId = "gpt-4o",
            LlmSourceType = LlmSourceType.OpenAi,
            Description = "其他",
            IsEnabled = false,
            UserId = "userC"
        });
        await db.SaveChangesAsync();

        var logger = NullLogger<CommonService>.Instance;
        var service = new LlmMngService(db, logger);
        var param = new QueryBase
        {
            Keyword = "sidA",
            CurrentPage = 1,
            PageSize = 10,
            SortingDesc = true,
            Sorting = "ServiceID",
        };
        var result = await service.Query(param);

        Assert.IsTrue(result.Success);
        Assert.AreEqual(1, result.Data.Count());
        Assert.AreEqual("sidA", result.Data.First().ServiceId);
    }

    /// <summary>
    /// 測試：Detail 應正確取得 LLM 詳細資料
    /// </summary>
    [TestMethod]
    public async Task Detail_ShouldReturnCorrectData_WhenExists()
    {
        var db = DbContextFactory.GetInMemoryDbContext();
        var logger = NullLogger<CommonService>.Instance;
        var service = new LlmMngService(db, logger);
        var llmId = Guid.Parse("00000001-AAAA-AAAA-AAAA-AAAAAAAAAAAA");

        var result = await service.Detail(llmId);
        Assert.IsTrue(result.Success);
        Assert.IsNotNull(result.Data);
        Assert.AreEqual(llmId, result.Data.LlmId);
        Assert.AreEqual("ServiceId-A-1", result.Data.ServiceId);
        Assert.AreEqual("gpt-4o", result.Data.ModelId);
        Assert.AreEqual("說明 ServiceId-A-1", result.Data.Description);
        Assert.AreEqual(DbContextFactory.AdminUserId, result.Data.UserId);
        Assert.AreEqual("系統管理員", result.Data.UserName);
    }

    /// <summary>
    /// 測試：Detail 查無資料時應回傳失敗
    /// </summary>
    [TestMethod]
    public async Task Detail_ShouldFail_WhenNotExists()
    {
        var db = DbContextFactory.GetInMemoryDbContext();
        var logger = NullLogger<CommonService>.Instance;
        var service = new LlmMngService(db, logger);
        var result = await service.Detail(Guid.NewGuid());

        Assert.IsFalse(result.Success);
        Assert.AreEqual(ConstantData.Error.NoData, result.Message);
    }

    /// <summary>
    /// 測試：Update 應正確更新現有資料
    /// </summary>
    [TestMethod]
    public async Task Update_ShouldSuccess_WhenValid()
    {
        var db = DbContextFactory.GetInMemoryDbContext();
        var logger = NullLogger<CommonService>.Instance;
        var service = new LlmMngService(db, logger);
        var llmId = Guid.Parse("00000001-AAAA-AAAA-AAAA-AAAAAAAAAAAA");

        var vm = new LlmUpdateVm
        {
            LlmId = llmId,
            ServiceId = "ServiceId-A-1-Update",
            ModelId = "gpt-4o-update",
            LlmSourceType = LlmSourceType.OpenAi,
            ApiKey = "ServiceId-Akey-Update",
            Endpoint = "https://test-update.com",
            DeploymentName = "openai-a-1-update",
            Description = "說明 ServiceId-A-1-Update"
        };

        var result = await service.Update(vm);

        Assert.IsTrue(result.Success);
        Assert.AreEqual(llmId.ToString(), result.Data);

        var entity = await db.LlmInfoes.FirstOrDefaultAsync(x => x.LlmId == llmId);
        Assert.IsNotNull(entity);
        Assert.AreEqual("ServiceId-A-1-Update", entity.ServiceId);
        Assert.AreEqual("gpt-4o-update", entity.ModelId);
        Assert.AreEqual("ServiceId-Akey-Update", entity.ApiKey);
        Assert.AreEqual("https://test-update.com", entity.Endpoint);
        Assert.AreEqual("openai-a-1-update", entity.DeploymentName);
        Assert.AreEqual("說明 ServiceId-A-1-Update", entity.Description);
    }

    /// <summary>
    /// 測試：Update 查無資料時應回傳失敗
    /// </summary>
    [TestMethod]
    public async Task Update_ShouldFail_WhenNotExists()
    {
        var db = DbContextFactory.GetInMemoryDbContext();
        var logger = NullLogger<CommonService>.Instance;
        var service = new LlmMngService(db, logger);

        var vm = new LlmUpdateVm
        {
            LlmId = Guid.NewGuid(),
            ServiceId = "ServiceId-NotExists",
            ModelId = "gpt-4o",
            LlmSourceType = LlmSourceType.OpenAi,
            ApiKey = "key",
            Endpoint = "https://test.com",
            DeploymentName = "dep",
            Description = "說明"
        };

        var result = await service.Update(vm);

        Assert.IsFalse(result.Success);
        Assert.AreEqual(ConstantData.Error.NoData, result.Message);
    }

    /// <summary>
    /// 測試：Delete 應正確刪除現有資料（管理者身分）
    /// </summary>
    [TestMethod]
    public async Task Delete_ShouldSuccess_WhenAdmin()
    {
        var db = DbContextFactory.GetInMemoryDbContext();
        var logger = NullLogger<CommonService>.Instance;
        var service = new LlmMngService(db, logger);
        var llmId = Guid.Parse("00000001-AAAA-AAAA-AAAA-AAAAAAAAAAAA");
        var userInfo = new UserInfo
        {
            UserId = DbContextFactory.AdminUserId,
            RoleId = DbContextFactory.AdminRoleId,
            OrgId = string.Empty
        };

        var result = await service.Delete(llmId, userInfo);

        Assert.IsTrue(result.Success);
        Assert.AreEqual(llmId.ToString(), result.Data);

        var entity = await db.LlmInfoes.FirstOrDefaultAsync(x => x.LlmId == llmId);
        Assert.IsNotNull(entity);
        Assert.IsFalse(entity.IsEnabled);
    }

    /// <summary>
    /// 測試：Delete 查無資料時應回傳失敗
    /// </summary>
    [TestMethod]
    public async Task Delete_ShouldFail_WhenNotExists()
    {
        var db = DbContextFactory.GetInMemoryDbContext();
        var logger = NullLogger<CommonService>.Instance;
        var service = new LlmMngService(db, logger);
        var userInfo = new UserInfo
        {
            UserId = DbContextFactory.AdminUserId,
            RoleId = DbContextFactory.AdminRoleId,
            OrgId = string.Empty
        };

        var result = await service.Delete(Guid.NewGuid(), userInfo);

        Assert.IsFalse(result.Success);
        Assert.AreEqual(ConstantData.Error.NoData, result.Message);
    }

    /// <summary>
    /// 測試：Delete 非管理者或非擁有者時應回傳失敗
    /// </summary>
    [TestMethod]
    public async Task Delete_ShouldFail_WhenNoPermission()
    {
        var db = DbContextFactory.GetInMemoryDbContext();
        var logger = NullLogger<CommonService>.Instance;
        var service = new LlmMngService(db, logger);
        var llmId = Guid.Parse("00000001-AAAA-AAAA-AAAA-AAAAAAAAAAAA");
        var userInfo = new UserInfo
        {
            UserId = "other-user",
            RoleId = "other-role",
            OrgId = string.Empty
        };

        var result = await service.Delete(llmId, userInfo);

        Assert.IsFalse(result.Success);
        Assert.AreEqual(ConstantData.Error.NoAuthority, result.Message);
    }

    /// <summary>
    /// 測試：Delete 使用者為資料擁有者（非管理者）時應回傳成功
    /// </summary>
    [TestMethod]
    public async Task Delete_ShouldSuccess_WhenOwnerNotAdmin()
    {
        var db = DbContextFactory.GetInMemoryDbContext();
        var logger = NullLogger<CommonService>.Instance;
        var service = new LlmMngService(db, logger);

        var nonAdminUserId = "non-admin-user-id";
        var nonAdminLlm = new LlmInfo
        {
            LlmId = Guid.NewGuid(),
            ServiceId = "sid-non-admin",
            ModelId = "gpt-4o",
            LlmSourceType = LlmSourceType.OpenAi,
            ApiKey = "key",
            IsEnabled = true,
            UserId = nonAdminUserId
        };
        db.LlmInfoes.Add(nonAdminLlm);
        await db.SaveChangesAsync();

        var userInfo = new UserInfo
        {
            UserId = nonAdminUserId,
            RoleId = "some-other-role",
            OrgId = string.Empty
        };

        var result = await service.Delete(nonAdminLlm.LlmId, userInfo);

        Assert.IsTrue(result.Success);
        Assert.AreEqual(nonAdminLlm.LlmId.ToString(), result.Data);
        var entity = await db.LlmInfoes.FirstOrDefaultAsync(x => x.LlmId == nonAdminLlm.LlmId);
        Assert.IsNotNull(entity);
        Assert.IsFalse(entity.IsEnabled);
    }

    /// <summary>
    /// 測試：Delete 管理者但非擁有者時應回傳成功
    /// </summary>
    [TestMethod]
    public async Task Delete_ShouldSuccess_WhenAdminNotOwner()
    {
        var db = DbContextFactory.GetInMemoryDbContext();
        var logger = NullLogger<CommonService>.Instance;
        var service = new LlmMngService(db, logger);

        var anotherUserId = "another-user-id";
        var anotherLlm = new LlmInfo
        {
            LlmId = Guid.NewGuid(),
            ServiceId = "sid-another",
            ModelId = "gpt-4o",
            LlmSourceType = LlmSourceType.OpenAi,
            ApiKey = "key",
            IsEnabled = true,
            UserId = anotherUserId
        };
        db.LlmInfoes.Add(anotherLlm);
        await db.SaveChangesAsync();

        var adminUserInfo = new UserInfo
        {
            UserId = DbContextFactory.AdminUserId,
            RoleId = DbContextFactory.AdminRoleId,
            OrgId = string.Empty
        };

        var result = await service.Delete(anotherLlm.LlmId, adminUserInfo);

        Assert.IsTrue(result.Success);
        Assert.AreEqual(anotherLlm.LlmId.ToString(), result.Data);
        var entity = await db.LlmInfoes.FirstOrDefaultAsync(x => x.LlmId == anotherLlm.LlmId);
        Assert.IsNotNull(entity);
        Assert.IsFalse(entity.IsEnabled);
    }
}