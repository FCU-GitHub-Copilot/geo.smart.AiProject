namespace Geo.Smart.AiAgentHub.Entities.Vms.AiAgent;

/// <summary>
/// AI 專案更新的 ViewModel
/// </summary>
public class AiProjectUpdateVm : AiProjectCreateVm
{
    /// <summary>
    /// 專案 ID
    /// </summary>
    public Guid ProjectId { get; set; } = Guid.NewGuid();
}