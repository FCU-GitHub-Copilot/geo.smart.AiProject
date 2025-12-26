namespace Geo.Smart.AiAgentHub.Services.Contracts;

/// <summary>
/// 定義使用者頁面軌跡服務的介面
/// </summary>
public interface IFootprintService
{
    /// <summary>
    /// 使用者頁面軌跡，前端呼叫
    /// </summary>
    /// <param name="vm">使用者頁面軌跡的檢視模型</param>
    /// <param name="userId">使用者的唯一識別碼</param>
    /// <returns>操作結果，包含成功或失敗的訊息</returns>
    Result<string> Frontend(UserFootprintVm vm, string userId);
}