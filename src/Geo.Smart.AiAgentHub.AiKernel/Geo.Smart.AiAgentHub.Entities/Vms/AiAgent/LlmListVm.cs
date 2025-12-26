namespace Geo.Smart.AiAgentHub.Entities.Vms.AiAgent;

/// <summary>
/// LLM 資料列表 ViewModel
/// </summary>
public class LlmListVm
{
    /// <summary>
    /// LLM ID
    /// </summary>
    public Guid LlmId { get; set; }

    /// <summary>
    /// 模型管理名稱、服務識別碼
    /// </summary>
    public string ServiceId { get; set; } = string.Empty;

    /// <summary>
    /// LLM 模型名稱
    /// </summary>
    public string ModelId { get; set; } = string.Empty;

    /// <summary>
    /// LLM 來源類型
    /// </summary>
    public LlmSourceType LlmSourceType { get; set; }

    /// <summary>
    /// 說明
    /// </summary>
    public string? Description { get; set; }
}