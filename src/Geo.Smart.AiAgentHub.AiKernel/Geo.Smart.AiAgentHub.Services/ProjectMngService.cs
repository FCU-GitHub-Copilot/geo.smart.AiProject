using Geo.Smart.AiAgentHub.AiKernel.Models.Vms;
using Geo.Smart.AiAgentHub.AiKernel.Vm;
using Geo.Smart.AiAgentHub.Entities.Vms.AiAgent;
using Geo.Smart.AiAgentHub.Infras;
using Geo.Smart.AiAgentHub.Services.Common;
using Geo.Smart.AiAgentHub.Services.Contracts;
using Geo.Smart.AiAgentHub.Services.Extension;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace Geo.Smart.AiAgentHub.Services;

/// <summary>
/// AI 專案管理服務
/// </summary>
/// <param name="dbModel">資料庫內容物件</param>
/// <param name="_logger">logger 物件</param>
public class ProjectMngService(GdbContext dbModel,
    ILogger<CommonService> _logger)
    : BaseService(dbModel, _logger), IProjectMngService
{
    /// <summary>
    /// 取得 AI 專案資料列表
    /// </summary>
    /// <param name="param">查詢條件</param>
    /// <returns>分頁的 AI 專案資料列表</returns>
    public async Task<PaginationResult<AiProjectListVm>> Query(QueryBase param)
    {
        try
        {
            // 加入 AsSplitQuery 以避免 EF Core 多集合 Include 預設的 SingleQuery 效能警告
            var query = DbModel.AiProjects.AsNoTracking()
                .AsSplitQuery()
                .Include(x => x.LlmInfoes)
                .Include(x => x.McpServers)
                .Where(x => x.IsEnabled)
                .WhereIf(!string.IsNullOrEmpty(param.Keyword), x =>
                    x.Name.Contains(param.Keyword!) ||
                    x.SystemPrompt.Contains(param.Keyword!) ||
                    (x.Description != null && x.Description.Contains(param.Keyword!))
                )
                .Select(x => new AiProjectListVm
                {
                    ProjectId = x.ProjectId,
                    Name = x.Name,
                    Description = x.Description,
                    LlmNames = x.LlmInfoes.Select(l => l.ServiceId).ToList(),
                    McpServerNames = x.McpServers.Select(m => m.Name).ToList()
                });

            return await ResultHelper.PaginationSuccessAsync(query, param);
        }
        catch (Exception ex)
        {
            LogError(ex, nameof(Query));
            return ResultHelper.PaginationFailure<AiProjectListVm>(ex.Message);
        }
    }

    /// <summary>
    /// 取得 AI 專案詳細資料
    /// </summary>
    /// <param name="projectId">專案 ID</param>
    /// <returns>AI 專案詳細資料</returns>
    public async Task<Result<AiProjectDetailVm>> Detail(Guid projectId)
    {
        try
        {
            var project = await DbModel.AiProjects.AsNoTracking()
                .Include(x => x.LlmInfoes)
                .Include(x => x.McpServers)
                .Include(x => x.ApplicationUser)
                .Where(x => x.ProjectId == projectId && x.IsEnabled)
                .Select(x => new AiProjectDetailVm
                {
                    ProjectId = x.ProjectId,
                    Name = x.Name,
                    Description = x.Description,
                    SystemPrompt = x.SystemPrompt,
                    Temperature = x.Temperature,
                    TopP = x.TopP,
                    TopK = x.TopK,
                    MaxTokens = x.MaxTokens,
                    Owner = x.ApplicationUser != null ? x.ApplicationUser.FullName : string.Empty,
                    LlmInfos = x.LlmInfoes.Select(l => new LlmListVm
                    {
                        LlmId = l.LlmId,
                        ServiceId = l.ServiceId,
                        ModelId = l.ModelId,
                        LlmSourceType = l.LlmSourceType,
                        Description = l.Description
                    }).ToList(),
                    McpServers = x.McpServers.Select(m => new McpServerListVm
                    {
                        McpServerId = m.McpServerId,
                        Name = m.Name,
                        McpServerType = m.McpServerType,
                        SseUrl = m.SseUrl,
                        StdioCommand = m.StdioCommand
                    }).ToList()
                })
                .FirstOrDefaultAsync();

            if (project == null)
            {
                return ResultHelper.Failure<AiProjectDetailVm>("查無資料或無權限");
            }

            return ResultHelper.Success(project);
        }
        catch (Exception ex)
        {
            LogError(ex, nameof(Detail));
            return ResultHelper.Failure<AiProjectDetailVm>(ex.Message);
        }
    }

    /// <summary>
    /// 新增 AI 專案資料
    /// </summary>
    /// <param name="vm">AI 專案建立 ViewModel</param>
    /// <param name="userId">擁有者 UserId</param>
    /// <returns>新增結果</returns>
    public async Task<Result<string>> Create(AiProjectCreateVm vm, string userId)
    {
        try
        {
            var valideInput = ValideProjectRequired(vm);
            if (!valideInput.Success)
            {
                return valideInput;
            }

            var exists = await DbModel.AiProjects.AsNoTracking()
                .AnyAsync(x => x.Name == vm.Name && x.IsEnabled);
            if (exists)
            {
                return ResultHelper.Failure<string>("專案名稱已存在");
            }

            var entity = new AiProject
            {
                ProjectId = Guid.NewGuid(),
                Name = vm.Name,
                Description = vm.Description,
                SystemPrompt = vm.SystemPrompt,
                Temperature = vm.Temperature,
                TopP = vm.TopP,
                TopK = vm.TopK,
                MaxTokens = vm.MaxTokens,
                UserId = userId,
                IsEnabled = true
            };

            // 關聯 LLM
            if (vm.LlmIds.Count > 0)
            {
                entity.LlmInfoes = await DbModel.LlmInfoes
                    .Where(l => vm.LlmIds.Contains(l.LlmId) && l.IsEnabled)
                    .ToListAsync();
            }

            // 關聯 MCP Server
            if (vm.McpServerIds.Count > 0)
            {
                entity.McpServers = await DbModel.McpServers
                    .Where(m => vm.McpServerIds.Contains(m.McpServerId) && m.IsEnabled)
                    .ToListAsync();
            }

            DbModel.AiProjects.Add(entity);
            await DbModel.SaveChangesAsync();

            return ResultHelper.Success(entity.ProjectId.ToString());
        }
        catch (Exception ex)
        {
            LogError(ex, nameof(Create));
            return ResultHelper.Failure<string>(ex.Message);
        }
    }

    /// <summary>
    /// 編輯 AI 專案資料
    /// </summary>
    /// <param name="vm">AI 專案更新 ViewModel</param>
    /// <returns>更新結果</returns>
    public async Task<Result<string>> Update(AiProjectUpdateVm vm)
    {
        try
        {
            var valideInput = ValideProjectRequired(vm);
            if (!valideInput.Success)
            {
                return valideInput;
            }
            var exist = await DbModel.AiProjects
                .AsNoTracking()
                .AnyAsync(x => x.ProjectId != vm.ProjectId && x.Name == vm.Name && x.IsEnabled);
            if (exist)
            {
                return ResultHelper.Failure<string>("專案名稱已經被使用，請填寫其他名稱");
            }

            var entity = await DbModel.AiProjects
                .Include(x => x.LlmInfoes)
                .Include(x => x.McpServers)
                .FirstOrDefaultAsync(x => x.ProjectId == vm.ProjectId && x.IsEnabled);
            if (entity == null)
            {
                return ResultHelper.Failure<string>("查無資料或無權限");
            }

            entity.Name = vm.Name;
            entity.Description = vm.Description;
            entity.SystemPrompt = vm.SystemPrompt;
            entity.Temperature = vm.Temperature;
            entity.TopP = vm.TopP;
            entity.TopK = vm.TopK;
            entity.MaxTokens = vm.MaxTokens;
            // 更新 LLM 關聯
            entity.LlmInfoes = await DbModel.LlmInfoes
                .Where(l => vm.LlmIds.Contains(l.LlmId) && l.IsEnabled)
                .ToListAsync();

            // 更新 MCP Server 關聯
            entity.McpServers = await DbModel.McpServers
                    .Where(m => vm.McpServerIds.Contains(m.McpServerId) && m.IsEnabled)
                    .ToListAsync();

            await DbModel.SaveChangesAsync();
            return ResultHelper.Success(entity.ProjectId.ToString());
        }
        catch (Exception ex)
        {
            LogError(ex, nameof(Update));
            return ResultHelper.Failure<string>(ex.Message);
        }
    }

    /// <summary>
    /// 驗證必要欄位
    /// </summary>
    /// <param name="vm"></param>
    /// <returns></returns>
    private static Result<string> ValideProjectRequired(AiProjectCreateVm vm)
    {
        var requiredFields = new Dictionary<string, string>
            {
                { nameof(vm.Name), vm.Name },
                { nameof(vm.SystemPrompt), vm.SystemPrompt }
            };
        var emptyField = requiredFields.FirstOrDefault(f => string.IsNullOrWhiteSpace(f.Value));
        if (!string.IsNullOrEmpty(emptyField.Key))
        {
            return ResultHelper.Failure<string>($"{emptyField.Key}為必填");
        }
        return ResultHelper.Success(string.Empty);
    }

    /// <summary>
    /// 刪除 AI 專案資料
    /// </summary>
    /// <param name="projectId">要刪除的專案 ID</param>
    /// <param name="userInfo">使用者資訊</param>
    /// <returns>刪除結果</returns>
    public async Task<Result<string>> Delete(Guid projectId, UserInfo userInfo)
    {
        try
        {
            var entity = await DbModel.AiProjects
                .Include(x => x.ApplicationUser)
                .FirstOrDefaultAsync(x => x.ProjectId == projectId && x.IsEnabled);

            if (entity == null)
            {
                return ResultHelper.Failure<string>("查無資料");
            }
            if (!DefaultCfgs.IsAdmin(userInfo)
                || userInfo.UserId != entity.UserId)
            {
                return ResultHelper.Failure<string>("無權限");
            }
            entity.IsEnabled = false;
            await DbModel.SaveChangesAsync();
            return ResultHelper.Success(entity.ProjectId.ToString());
        }
        catch (Exception ex)
        {
            LogError(ex, nameof(Delete));
            return ResultHelper.Failure<string>(ex.Message);
        }
    }

    /// <summary>
    /// 取得專案設定
    /// </summary>
    /// <param name="projectId">專案 ID</param>
    /// <returns></returns>
    public async Task<ProjectSettingVm?> GetProjectSetting(Guid projectId)
    {
        var project = await GetProjectEntity(projectId);
        if (project == null)
        {
            return null;
        }
        return GetProjectSettingVm(project);
    }

    private async Task<AiProject?> GetProjectEntity(Guid projectId)
    {
        return await DbModel.AiProjects.AsNoTracking()
            .Include(x => x.LlmInfoes)
            .Include(x => x.McpServers)
            .Where(x => x.ProjectId == projectId && x.IsEnabled)
            .FirstOrDefaultAsync();
    }

    private static ProjectSettingVm GetProjectSettingVm(AiProject project)
    {
        return new ProjectSettingVm
        {
            Name = project.Name,
            Description = project.Description,
            SystemPrompt = project.SystemPrompt,
            Temperature = project.Temperature,
            TopP = project.TopP,
            TopK = project.TopK,
            MaxTokens = project.MaxTokens,
            LlmInfos = [.. project.LlmInfoes.Select(l => new LlmSetupVm
                {
                    ServiceId = l.ServiceId,
                    ModelId = l.ModelId,
                    LlmSourceType = l.LlmSourceType,
                    ApiKey = l.ApiKey,
                    DeploymentName = l.DeploymentName,
                    Endpoint = l.Endpoint
                })],
            McpServers = [.. project.McpServers.Select(m => new McpServerVm
                {
                    Name = m.Name,
                    McpServerType = m.McpServerType,
                    SseUrl = m.SseUrl ?? string.Empty,
                    StdioCommand = m.StdioCommand,
                    StdioArgs = string.IsNullOrEmpty(m.StdioArgs) ? null : JsonSerializer.Deserialize<List<string>>(m.StdioArgs),
                    StdioEnv = string.IsNullOrEmpty(m.StdioEnv) ? null : JsonSerializer.Deserialize<Dictionary<string, string?>>(m.StdioEnv),
                    Tools = string.IsNullOrEmpty(m.Tools) ? [] : JsonSerializer.Deserialize<List<string>>(m.Tools)!
                })]
        };
    }

    /// <summary>
    /// 下載 AI 專案設定檔 JSON
    /// </summary>
    /// <param name="projectId">專案 ID</param>
    /// <param name="userInfo">使用者資訊</param>
    /// <returns></returns>
    public async Task<Result<string>> DownloadSetting(Guid projectId, UserInfo userInfo)
    {
        try
        {
            var project = await GetProjectEntity(projectId);

            if (project == null)
            {
                return ResultHelper.Failure<string>(ConstantData.Error.NoData);
            }
            if (project.UserId != userInfo.UserId && !DefaultCfgs.IsAdmin(userInfo))
            {
                return ResultHelper.Failure<string>(ConstantData.Error.NoAuthority);
            }

            var projectSetting = GetProjectSettingVm(project);

            var json = JsonSerializer.Serialize(projectSetting);
            return ResultHelper.Success(json);
        }
        catch (Exception ex)
        {
            LogError(ex, nameof(DownloadSetting));
            return ResultHelper.Failure<string>(ex.Message);
        }
    }

    /// <summary>
    /// 取得可以選取的 LLM 清單
    /// </summary>
    /// <returns></returns>
    public async Task<Result<List<ProjectLlmVm>>> GetLlms()
    {
        try
        {
            var llms = await DbModel.LlmInfoes.AsNoTracking()
           .Where(x => x.IsEnabled)
           .Select(x => new ProjectLlmVm
           {
               LlmId = x.LlmId,
               ServiceId = x.ServiceId,
               ModelId = x.ModelId,
               LlmSourceType = x.LlmSourceType
           })
           .ToListAsync();
            return ResultHelper.Success(llms);
        }
        catch (Exception ex)
        {
            LogError(ex, nameof(GetLlms));
            return ResultHelper.Failure<List<ProjectLlmVm>>(ex.Message);
        }
    }

    /// <summary>
    /// 取得可以選取的 MCP Server 清單
    /// </summary>
    /// <returns></returns>
    public async Task<Result<List<ProjectMcpVm>>> GetMcpServers()
    {
        try
        {
            var mcps = await DbModel.McpServers.AsNoTracking()
            .Where(x => x.IsEnabled)
            .Select(x => new ProjectMcpVm
            {
                McpServerId = x.McpServerId,
                Name = x.Name,
                McpServerType = x.McpServerType,
            })
            .ToListAsync();
            return ResultHelper.Success(mcps);
        }
        catch (Exception ex)
        {
            LogError(ex, nameof(GetMcpServers));
            return ResultHelper.Failure<List<ProjectMcpVm>>(ex.Message);
        }
    }

    /// <summary>
    /// 取得專案設定的 LLM 與工具清單
    /// </summary>
    /// <param name="projectId">專案 ID</param>
    /// <returns></returns>
    public async Task<Result<ModelToolsVm>> ModelTools(Guid projectId)
    {
        try
        {
            var projectSetting = await GetProjectSetting(projectId);
            if (projectSetting == null)
            {
                return ResultHelper.Failure<ModelToolsVm>(ConstantData.Error.NoData);
            }
            var vm = new ModelToolsVm
            {
                Llms = [.. projectSetting.LlmInfos.Select(x => new ModelToolsLlm
                {
                    ServiceId = x.ServiceId,
                    ModelId = x.ModelId,
                    LlmSourceType = x.LlmSourceType,
                })],
                McpServers = [.. projectSetting.McpServers.Select(x => new ModelToolsMcp
                {
                    Name = x.Name,
                    McpServerType = x.McpServerType,
                    Tools = x.Tools,
                })]
            };

            return ResultHelper.Success(vm);
        }
        catch (Exception e)
        {
            LogError(e, nameof(ModelTools));
            return ResultHelper.Failure<ModelToolsVm>(ConstantData.Error.Exception);
        }
    }
}