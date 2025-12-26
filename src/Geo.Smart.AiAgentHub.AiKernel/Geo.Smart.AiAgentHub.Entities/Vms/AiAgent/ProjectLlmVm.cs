using Geo.Smart.AiAgentHub.AiKernel.Models.Vms;

namespace Geo.Smart.AiAgentHub.Entities.Vms.AiAgent;

/// <summary>
/// 專案設定 LLM 可用清單
/// </summary>
public class ProjectLlmVm : ModelToolsLlm
{
    /// <summary>
    /// LLM ID
    /// </summary>
    public Guid LlmId { get; set; }
}