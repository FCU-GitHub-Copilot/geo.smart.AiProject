#pragma warning disable CS1591 // 遺漏公用可見類型或成員的 XML 註解

namespace Geo.Smart.AiAgentHub.Infras.Enums;

/// <summary>
/// 記錄 MCP 伺服器型態的列舉
/// </summary>
public enum McpServerType
{
    /// <summary>
    /// 標準輸入輸出型態
    /// </summary>
    Stdio,

    /// <summary>
    /// Server-Sent Events 型態
    /// </summary>
    Sse,

    /// <summary>
    /// Streamable HTTP 型態
    /// </summary>
    Streamable
}