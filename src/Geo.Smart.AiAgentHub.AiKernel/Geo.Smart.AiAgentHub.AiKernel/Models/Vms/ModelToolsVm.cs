namespace Geo.Smart.AiAgentHub.AiKernel.Models.Vms;

/// <summary>
/// 專案可用的 LLM 與 MCP Server 工具清單 ViewModel
/// </summary>
public class ModelToolsVm
{
    /// <summary>
    /// 專案可用的 LLM 清單
    /// </summary>
    public List<ModelToolsLlm> Llms { get; set; } = [];

    /// <summary>
    /// 專案可用的 MCP Server 清單
    /// </summary>
    public List<ModelToolsMcp> McpServers { get; set; } = [];
}