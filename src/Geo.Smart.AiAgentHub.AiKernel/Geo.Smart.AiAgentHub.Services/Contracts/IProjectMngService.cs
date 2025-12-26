using Geo.Smart.AiAgentHub.AiKernel.Models.Vms;
using Geo.Smart.AiAgentHub.AiKernel.Vm;
using Geo.Smart.AiAgentHub.Entities.Vms.AiAgent;

namespace Geo.Smart.AiAgentHub.Services.Contracts;

/// <summary>
/// AI 專案管理服務介面，定義查詢、詳細、建立、更新、刪除等相關功能
/// </summary>
public interface IProjectMngService
{
    /// <summary>
    /// 查詢 AI 專案列表
    /// </summary>
    /// <param name="param">查詢參數物件</param>
    /// <returns>回傳分頁結果，包含 AI 專案列表 ViewModel</returns>
    Task<PaginationResult<AiProjectListVm>> Query(QueryBase param);

    /// <summary>
    /// 取得 AI 專案詳細資訊
    /// </summary>
    /// <param name="projectId">專案唯一識別碼</param>
    /// <returns>回傳結果物件，包含 AI 專案詳細 ViewModel</returns>
    Task<Result<AiProjectDetailVm>> Detail(Guid projectId);

    /// <summary>
    /// 建立 AI 專案
    /// </summary>
    /// <param name="vm">AI 專案建立 ViewModel</param>
    /// <param name="userId">建立者的使用者 ID</param>
    /// <returns>回傳結果物件，包含建立結果字串</returns>
    Task<Result<string>> Create(AiProjectCreateVm vm, string userId);

    /// <summary>
    /// 更新 AI 專案
    /// </summary>
    /// <param name="vm">AI 專案更新 ViewModel</param>
    /// <returns>回傳結果物件，包含更新結果字串</returns>
    Task<Result<string>> Update(AiProjectUpdateVm vm);

    /// <summary>
    /// 刪除 AI 專案
    /// </summary>
    /// <param name="projectId">專案 ID</param>
    /// <param name="userInfo">執行者的使用者資訊</param>
    /// <returns>回傳結果物件，包含刪除結果字串</returns>
    Task<Result<string>> Delete(Guid projectId, UserInfo userInfo);

    /// <summary>
    /// 下載 AI 專案設定檔 JSON
    /// </summary>
    /// <param name="projectId">專案 ID</param>
    /// <param name="userInfo">使用者資訊</param>
    /// <returns></returns>
    Task<Result<string>> DownloadSetting(Guid projectId, UserInfo userInfo);

    /// <summary>
    /// 取得可以選取的 LLM 清單
    /// </summary>
    /// <returns></returns>
    Task<Result<List<ProjectLlmVm>>> GetLlms();

    /// <summary>
    /// 取得可以選取的 MCP Server 清單
    /// </summary>
    /// <returns></returns>
    Task<Result<List<ProjectMcpVm>>> GetMcpServers();

    /// <summary>
    /// 取得專案設定
    /// </summary>
    /// <param name="projectId">專案 ID</param>
    /// <returns></returns>
    Task<ProjectSettingVm?> GetProjectSetting(Guid projectId);

    /// <summary>
    /// 取得專案設定的 LLM 與工具清單
    /// </summary>
    /// <param name="projectId">專案 ID</param>
    /// <returns></returns>
    Task<Result<ModelToolsVm>> ModelTools(Guid projectId);
}