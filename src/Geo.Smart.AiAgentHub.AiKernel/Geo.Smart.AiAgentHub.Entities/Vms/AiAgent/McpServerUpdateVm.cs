using Geo.Smart.AiAgentHub.AiKernel.Vm;

namespace Geo.Smart.AiAgentHub.Entities.Vms.AiAgent;

/// <summary>
/// 編輯 MCP Server 的 ViewModel
/// </summary>
public class McpServerUpdateVm : McpServerVm
{
    /// <summary>
    /// MCP Server ID
    /// </summary>
    public Guid McpServerId { get; set; } = Guid.NewGuid();
}