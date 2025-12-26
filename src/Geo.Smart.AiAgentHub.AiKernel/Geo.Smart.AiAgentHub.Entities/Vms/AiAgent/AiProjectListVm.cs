namespace Geo.Smart.AiAgentHub.Entities.Vms.AiAgent;

/// <summary>
/// AI 專案列表的 ViewModel
/// </summary>
public class AiProjectListVm
{
    /// <summary>
    /// 專案 ID
    /// </summary>
    public Guid ProjectId { get; set; } = Guid.NewGuid();

    /// <summary>
    /// 專案名稱
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// 專案說明
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// 溫度，控制 LLM 的創造力，範圍 0 到 2 之間
    /// </summary>
    public double? Temperature { get; set; }

    /// <summary>
    /// 最大的 token 數量
    /// </summary>
    public int? MaxTokens { get; set; }

    /// <summary>
    /// LLM 名稱清單
    /// </summary>
    public List<string> LlmNames { get; set; } = [];

    /// <summary>
    /// MCP Server 名稱清單
    /// </summary>
    public List<string> McpServerNames { get; set; } = [];
}