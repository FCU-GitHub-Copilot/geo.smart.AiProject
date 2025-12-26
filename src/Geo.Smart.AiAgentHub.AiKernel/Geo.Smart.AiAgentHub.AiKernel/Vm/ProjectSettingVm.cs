namespace Geo.Smart.AiAgentHub.AiKernel.Vm;

/// <summary>
/// AI 專案設定檔
/// </summary>
public class ProjectSettingVm
{
    /// <summary>
    /// 專案名稱
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// 專案說明
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// 系統提示詞
    /// </summary>
    public string SystemPrompt { get; set; } = string.Empty;

    /// <summary>
    /// 溫度，控制 LLM 的創造力，範圍 0 到 2 之間
    /// </summary>
    public double? Temperature { get; set; }

    /// <summary>
    /// 控制 LLM 文本生成的機率篩選器，範圍 0.1 到 2 之間
    /// </summary>
    public double? TopP { get; set; }

    /// <summary>
    /// LLM 只會從機率最高的 k 個 Tokens 中進行選擇
    /// </summary>
    public int? TopK { get; set; }

    /// <summary>
    /// 最大的 token 數量
    /// </summary>
    public int? MaxTokens { get; set; }

    /// <summary>
    /// LLM 清單
    /// </summary>
    public List<LlmSetupVm> LlmInfos { get; set; } = [];

    /// <summary>
    /// MCP Server 清單
    /// </summary>
    public List<McpServerVm> McpServers { get; set; } = [];
}