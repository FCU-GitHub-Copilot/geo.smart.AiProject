using Geo.Smart.AiAgentHub.Infras.Enums;

namespace Geo.Smart.AiAgentHub.AiKernel.Vm;

/// <summary>
/// MCP Server 的設定類別。
/// </summary>
public class McpServerVm
{
    /// <summary>
    /// 伺服器名稱。只能是英數字以及底線
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// MCP 伺服器通訊型態
    /// </summary>
    public McpServerType McpServerType { get; set; } = McpServerType.Sse;

    /// <summary>
    /// 伺服器的 SSE 端點 URL。
    /// </summary>
    public string SseUrl { get; set; } = string.Empty;

    /// <summary>
    /// stdio 的指令
    /// </summary>
    public string? StdioCommand { get; set; }

    /// <summary>
    /// stdio 指令參數，存 JSON array
    /// </summary>
    public List<string>? StdioArgs { get; set; }

    /// <summary>
    /// stdio 環境變數，存 JSON object
    /// </summary>
    public Dictionary<string, string?>? StdioEnv { get; set; }

    /// <summary>
    /// 工具清單，存 JSON object，Name、Description
    /// </summary>
    public List<string> Tools { get; set; } = [];
}