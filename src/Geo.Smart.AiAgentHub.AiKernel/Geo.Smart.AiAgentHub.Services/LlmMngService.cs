using Geo.Smart.AiAgentHub.Entities.Vms.AiAgent;
using Geo.Smart.AiAgentHub.Infras;
using Geo.Smart.AiAgentHub.Services.Common;
using Geo.Smart.AiAgentHub.Services.Contracts;
using Geo.Smart.AiAgentHub.Services.Extension;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Geo.Smart.AiAgentHub.Services;

/// <summary>
/// LLM 管理服務
/// </summary>
/// <param name="dbModel">資料庫內容物件</param>
public class LlmMngService(GdbContext dbModel,
    ILogger<CommonService> _logger
    ) : BaseService(dbModel, _logger), ILlmMngService
{
    /// <summary>
    /// 取得 LLM 資料列表
    /// </summary>
    /// <param name="param">查詢條件</param>
    /// <returns>分頁的 LLM 資料列表</returns>
    public async Task<PaginationResult<LlmListVm>> Query(QueryBase param)
    {
        try
        {
            var query = DbModel.LlmInfoes.AsNoTracking()
                .Where(x => x.IsEnabled)
                .WhereIf(!string.IsNullOrEmpty(param.Keyword), x =>
                    x.ServiceId.Contains(param.Keyword!) ||
                    x.ModelId.Contains(param.Keyword!) ||
                    (x.Description != null && x.Description.Contains(param.Keyword!))
                )
                .Select(x => new LlmListVm
                {
                    LlmId = x.LlmId,
                    ServiceId = x.ServiceId,
                    ModelId = x.ModelId,
                    LlmSourceType = x.LlmSourceType,
                    Description = x.Description,
                });
            return await ResultHelper.PaginationSuccessAsync(query, param);
        }
        catch (Exception ex)
        {
            LogError(ex, nameof(Query));
            return ResultHelper.PaginationFailure<LlmListVm>(ex.Message);
        }
    }

    /// <summary>
    /// 取得 LLM 詳細資料
    /// </summary>
    /// <param name="llmId">LLM Id</param>
    /// <returns>LLM 詳細資料</returns>
    public async Task<Result<LlmDetailVm>> Detail(Guid llmId)
    {
        try
        {
            var llm = await DbModel.LlmInfoes.AsNoTracking()
                .Include(x => x.ApplicationUser)
                .Where(x => x.LlmId == llmId && x.IsEnabled)
                .Select(x => new LlmDetailVm
                {
                    LlmId = x.LlmId,
                    UserId = x.UserId,
                    ServiceId = x.ServiceId,
                    ModelId = x.ModelId,
                    LlmSourceType = x.LlmSourceType,
                    ApiKey = x.ApiKey,
                    Endpoint = x.Endpoint,
                    DeploymentName = x.DeploymentName,
                    Description = x.Description,

                    UserName = x.ApplicationUser != null ? x.ApplicationUser.FullName : null,
                })
                .FirstOrDefaultAsync();

            if (llm == null)
            {
                return ResultHelper.Failure<LlmDetailVm>(ConstantData.Error.NoData);
            }

            return ResultHelper.Success(llm);
        }
        catch (Exception ex)
        {
            LogError(ex, nameof(Detail));
            return ResultHelper.Failure<LlmDetailVm>(ex.Message);
        }
    }

    /// <summary>
    /// 新增 LlmInfo 資料
    /// </summary>
    /// <param name="vm">LLM 設定 ViewModel</param>
    /// <param name="userId">擁有者 UserId</param>
    /// <returns>新增結果</returns>
    public async Task<Result<string>> Create(LlmCreateVm vm, string userId)
    {
        try
        {
            var exists = await DbModel.LlmInfoes.AsNoTracking()
                .AnyAsync(x => x.ServiceId == vm.ServiceId && x.IsEnabled);
            if (exists)
            {
                return ResultHelper.Failure<string>("服務識別碼 已存在");
            }

            var optionResult = CheckOptionFields(vm);
            if (!optionResult.Success)
            {
                return optionResult;
            }

            var entity = new LlmInfo
            {
                LlmId = Guid.NewGuid(),
                UserId = userId,
                ServiceId = vm.ServiceId,
                ModelId = vm.ModelId,
                LlmSourceType = vm.LlmSourceType,
                ApiKey = vm.ApiKey,
                Endpoint = vm.Endpoint,
                DeploymentName = vm.DeploymentName,
                Description = vm.Description,
            };

            DbModel.LlmInfoes.Add(entity);
            await DbModel.SaveChangesAsync();

            return ResultHelper.Success(entity.LlmId.ToString());
        }
        catch (Exception ex)
        {
            LogError(ex, nameof(Create));
            return ResultHelper.Failure<string>(ex.Message);
        }
    }

    /// <summary>
    /// 編輯 LLM 資料
    /// </summary>
    /// <param name="vm">LLM 更新 ViewModel</param>
    /// <returns>更新結果</returns>
    public async Task<Result<string>> Update(LlmUpdateVm vm)
    {
        try
        {
            var requiredFields = new Dictionary<string, string>
            {
                { nameof(vm.LlmId), vm.LlmId.ToString() },
                { nameof(vm.ServiceId), vm.ServiceId },
                { nameof(vm.ModelId), vm.ModelId },
                { nameof(vm.LlmSourceType), vm.LlmSourceType.ToString() }
            };
            var emptyField = requiredFields.FirstOrDefault(f => string.IsNullOrWhiteSpace(f.Value));
            if (!string.IsNullOrEmpty(emptyField.Key))
            {
                return ResultHelper.Failure<string>($"{emptyField.Key}為必填");
            }
            var optionResult = CheckOptionFields(vm);
            if (!optionResult.Success)
            {
                return optionResult;
            }
            var exist = await DbModel.LlmInfoes
                .AsNoTracking()
                .AnyAsync(x => x.LlmId != vm.LlmId && x.ServiceId == vm.ServiceId && x.IsEnabled);
            if (exist)
            {
                return ResultHelper.Failure<string>("服務識別碼已經被使用，請填寫其他識別碼");
            }

            var entity = await DbModel.LlmInfoes
                .FirstOrDefaultAsync(x => x.LlmId == vm.LlmId && x.IsEnabled);
            if (entity == null)
            {
                return ResultHelper.Failure<string>(ConstantData.Error.NoData);
            }

            entity.ServiceId = vm.ServiceId;
            entity.ModelId = vm.ModelId;
            entity.LlmSourceType = vm.LlmSourceType;
            entity.ApiKey = vm.ApiKey;
            entity.Endpoint = vm.Endpoint;
            entity.DeploymentName = vm.DeploymentName;
            entity.Description = vm.Description;

            await DbModel.SaveChangesAsync();
            return ResultHelper.Success(entity.LlmId.ToString());
        }
        catch (Exception ex)
        {
            LogError(ex, nameof(Update));
            return ResultHelper.Failure<string>(ex.Message);
        }
    }

    /// <summary>
    /// 刪除 LLM 資料
    /// </summary>
    /// <param name="llmId">要刪除的 LLM Id</param>
    /// <param name="userInfo">使用者資訊</param>
    /// <returns>刪除結果</returns>
    public async Task<Result<string>> Delete(Guid llmId, UserInfo userInfo)
    {
        try
        {
            var entity = await DbModel.LlmInfoes
                .FirstOrDefaultAsync(x => x.LlmId == llmId && x.IsEnabled);

            if (entity == null)
            {
                return ResultHelper.Failure<string>(ConstantData.Error.NoData);
            }
            if (!DefaultCfgs.IsAdmin(userInfo)
                || userInfo.UserId != entity.UserId)
            {
                return ResultHelper.Failure<string>(ConstantData.Error.NoAuthority);
            }
            entity.IsEnabled = false;
            await DbModel.SaveChangesAsync();
            return ResultHelper.Success(entity.LlmId.ToString());
        }
        catch (Exception ex)
        {
            LogError(ex, nameof(Delete));
            return ResultHelper.Failure<string>(ex.Message);
        }
    }

    /// <summary>
    /// 檢查 LLM 來源類型相關必填欄位
    /// </summary>
    /// <param name="vm">LLM 設定 ViewModel</param>
    /// <returns>檢查結果</returns>
    private static Result<string> CheckOptionFields(LlmCreateVm vm)
    {
        if (string.IsNullOrWhiteSpace(vm.ApiKey)
            && (vm.LlmSourceType == LlmSourceType.OpenAi
                || vm.LlmSourceType == LlmSourceType.AzureOpenAi
                || vm.LlmSourceType == LlmSourceType.Gemini
                || vm.LlmSourceType == LlmSourceType.Afs))
        {
            return ResultHelper.Failure<string>("API 金鑰 為必填");
        }

        if (string.IsNullOrWhiteSpace(vm.Endpoint)
            && (vm.LlmSourceType == LlmSourceType.AzureOpenAi
                || vm.LlmSourceType == LlmSourceType.Ollama
                || vm.LlmSourceType == LlmSourceType.Afs))
        {
            return ResultHelper.Failure<string>("端點網址 為必填");
        }

        if (string.IsNullOrWhiteSpace(vm.DeploymentName)
            && vm.LlmSourceType == LlmSourceType.AzureOpenAi)
        {
            return ResultHelper.Failure<string>("部署名稱 為必填");
        }

        return ResultHelper.Success(string.Empty);
    }
}