using Geo.Smart.AiAgentHub.AiKernel.Models.Vms;
using Geo.Smart.CommonCore.Models;

namespace Geo.Smart.AiAgentHub.AiKernel.Services.Contracts;

/// <summary>
/// 聊天室服務
/// </summary>
public interface IChatRoomService
{
    /// <summary>
    /// 取得指定使用者的聊天室清單
    /// </summary>
    /// <param name="userId">使用者識別碼</param>
    /// <param name="param">分頁參數</param>
    /// <returns>包含聊天室清單的結果物件</returns>
    Task<PaginationResult<ChatRoomVm>> Query(QueryBase param, string userId);

    /// <summary>
    /// 取得指定聊天室詳細資訊
    /// </summary>
    /// <param name="roomId">聊天室 ID</param>
    /// <param name="userId">使用者 ID</param>
    /// <returns>包含聊天室詳細資訊的結果物件</returns>
    Task<Result<ChatRoomDetailVm>> Datail(Guid roomId, string userId);

    /// <summary>
    /// 使用者聊天室提問
    /// </summary>
    /// <param name="askVm">提問內容</param>
    /// <param name="userId">使用者 ID</param>
    /// <param name="cancellationToken">用於中斷請求的 CancellationToken</param>
    /// <returns></returns>
    Task<Result<string>> Ask(ClientAskVm askVm, string userId, CancellationToken cancellationToken);

    /// <summary>
    /// 更新聊天室名稱
    /// </summary>
    /// <param name="idName">ID 與 名稱</param>
    /// <param name="userId">使用者 ID</param>
    /// <returns></returns>
    Task<Result<string>> Rename(IdName idName, string userId);

    /// <summary>
    /// 刪除聊天室
    /// </summary>
    /// <param name="roomId">聊天室 ID</param>
    /// <param name="userId">使用者 ID</param>
    /// <returns></returns>
    Task<Result<string>> Delete(Guid roomId, string userId);

    /// <summary>
    /// 取得專案設定的 LLM 與工具清單
    /// </summary>
    /// <returns></returns>
    Task<Result<ModelToolsVm>> ModelTools();
}