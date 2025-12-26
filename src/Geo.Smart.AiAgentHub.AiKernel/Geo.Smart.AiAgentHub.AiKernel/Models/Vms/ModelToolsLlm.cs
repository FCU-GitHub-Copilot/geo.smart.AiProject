using Geo.Smart.AiAgentHub.Infras.Enums;

namespace Geo.Smart.AiAgentHub.AiKernel.Models.Vms;

/// <summary>
/// 專案可用的 LLM 清單
/// </summary>
public class ModelToolsLlm
{
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
}