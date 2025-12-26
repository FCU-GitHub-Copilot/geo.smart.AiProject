using Geo.Smart.AiAgentHub.AiKernel.Models.Vms;

namespace Geo.Smart.AiAgentHub.Entities.Vms.AiAgent;

/// <summary>
/// 專案設定 MCP Server 清單
/// </summary>
public class ProjectMcpVm : ModelToolsMcp
{
    /// <summary>
    /// MCP Server ID
    /// </summary>
    public Guid McpServerId { get; set; } = Guid.NewGuid();
}