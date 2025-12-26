namespace Geo.Smart.AiAgentHub.Entities.Vms.AiAgent;

/// <summary>
/// MCP Server 資料列表 ViewModel
/// </summary>
public class McpServerListVm
{
    /// <summary>
    /// MCP Server ID
    /// </summary>
    public Guid McpServerId { get; set; } = Guid.NewGuid();

    /// <summary>
    /// MCP 服務名稱
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// MCP 伺服器通訊型態，0:Stdio,1:Sse,2:Streamable
    /// </summary>
    public McpServerType McpServerType { get; set; } = (McpServerType)0;

    /// <summary>
    /// 伺服器的 SSE 端點 URL
    /// </summary>
    public string? SseUrl { get; set; }

    /// <summary>
    /// stdio 的指令
    /// </summary>
    public string? StdioCommand { get; set; }
}