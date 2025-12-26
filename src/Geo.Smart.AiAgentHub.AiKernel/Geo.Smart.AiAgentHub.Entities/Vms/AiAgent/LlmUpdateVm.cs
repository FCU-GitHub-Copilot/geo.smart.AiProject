namespace Geo.Smart.AiAgentHub.Entities.Vms.AiAgent;

/// <summary>
/// 編輯 LLM 設定資料的 ViewModel
/// </summary>
public class LlmUpdateVm : LlmCreateVm
{
    /// <summary>
    /// LLM 的唯一識別碼
    /// </summary>
    public Guid LlmId { get; set; }
}