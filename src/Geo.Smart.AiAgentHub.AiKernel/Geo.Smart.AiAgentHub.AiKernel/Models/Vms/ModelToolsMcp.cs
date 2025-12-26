using Geo.Smart.AiAgentHub.Infras.Enums;

namespace Geo.Smart.AiAgentHub.AiKernel.Models.Vms;

/// <summary>
/// 專案可用的 MCP Server 清單
/// </summary>
public class ModelToolsMcp
{
    /// <summary>
    /// MCP 服務名稱
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// MCP 伺服器通訊型態，0:Stdio,1:Sse,2:Streamable
    /// </summary>
    public McpServerType McpServerType { get; set; } = McpServerType.Streamable;

    /// <summary>
    /// 工具清單
    /// </summary>
    public List<string> Tools { get; set; } = [];
}