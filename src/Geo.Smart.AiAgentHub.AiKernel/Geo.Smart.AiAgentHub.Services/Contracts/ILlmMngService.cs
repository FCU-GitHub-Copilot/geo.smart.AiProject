using Geo.Smart.AiAgentHub.Entities.Vms.AiAgent;

namespace Geo.Smart.AiAgentHub.Services.Contracts;

/// <summary>
/// LLM 管理服務
/// </summary>
public interface ILlmMngService
{
    /// <summary>
    /// 取得 LLM 資料列表
    /// </summary>
    /// <param name="param">查詢條件</param>
    /// <returns>分頁的 LLM 資料列表</returns>
    Task<PaginationResult<LlmListVm>> Query(QueryBase param);

    /// <summary>
    /// 新增 LlmInfo 資料
    /// </summary>
    /// <param name="vm">LLM 設定 ViewModel</param>
    /// <param name="userId">擁有者 UserId</param>
    /// <returns>新增結果</returns>
    Task<Result<string>> Create(LlmCreateVm vm, string userId);

    /// <summary>
    /// 取得 LLM 詳細資料
    /// </summary>
    /// <param name="llmId">LLM Id</param>
    /// <returns>LLM 詳細資料</returns>
    Task<Result<LlmDetailVm>> Detail(Guid llmId);

    /// <summary>
    /// 編輯 LLM 資料
    /// </summary>
    /// <param name="vm">LLM 更新 ViewModel</param>
    /// <returns>更新結果</returns>
    Task<Result<string>> Update(LlmUpdateVm vm);

    /// <summary>
    /// 刪除 LLM 資料
    /// </summary>
    /// <param name="llmId">要刪除的 LLM Id</param>
    /// <param name="userInfo">使用者資訊</param>
    /// <returns>刪除結果</returns>
    Task<Result<string>> Delete(Guid llmId, UserInfo userInfo);
}