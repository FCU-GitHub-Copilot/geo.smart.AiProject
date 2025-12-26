namespace Geo.Smart.AiAgentHub.Entities.Vms.AiAgent;

/// <summary>
/// MCP Server 詳細資訊
/// </summary>
public class McpServerDetailVm
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
    /// MCP 伺服器通訊型態，0:Stdio,1:Sse
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

    /// <summary>
    /// stdio 指令參數，存 JSON array
    /// </summary>
    public List<string>? StdioArgs { get; set; } = [];

    /// <summary>
    /// stdio 環境變數，存 JSON object
    /// </summary>
    public Dictionary<string, string?>? StdioEnv { get; set; } = [];

    /// <summary>
    /// 工具清單，存 JSON object，Name、Description
    /// </summary>
    public List<string>? Tools { get; set; } = [];

    /// <summary>
    /// 擁有者姓名
    /// </summary>
    public string? UserName { get; set; }
}