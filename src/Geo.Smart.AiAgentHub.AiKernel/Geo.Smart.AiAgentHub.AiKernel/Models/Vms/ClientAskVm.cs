using Geo.Smart.AiAgentHub.AiKernel.Vm;

namespace Geo.Smart.AiAgentHub.AiKernel.Models.Vms;

/// <summary>
/// 使用者提問內容，
/// ProjectId 提供 AI 專案設定時的測試使用，
/// 各專案引用時應忽略，或直接使用 AskVm 即可
/// </summary>
public class ClientAskVm : AskVm
{
    /// <summary>
    /// 專案 ID
    /// </summary>
    public Guid? ProjectId { get; set; } = Guid.NewGuid();
}