namespace Geo.Smart.AiAgentHub.Entities.Vms.AiAgent;

/// <summary>
/// 建立 LLM 設定資料
/// </summary>
public class LlmCreateVm
{
    /// <summary>
    /// 模型管理名稱、服務識別碼
    /// </summary>
    public string ServiceId { get; set; } = string.Empty;

    /// <summary>
    /// LLM 模型名稱，gpt-4o
    /// </summary>
    public required string ModelId { get; set; } = string.Empty;

    /// <summary>
    /// LLM 來源類型，0:OpenAi,1:AzureOpenAi,2:Ollama,3:Gemini,4:Afs
    /// </summary>
    public LlmSourceType LlmSourceType { get; set; } = (LlmSourceType)0;

    /// <summary>
    /// API 金鑰
    /// </summary>
    public string? ApiKey { get; set; }

    /// <summary>
    /// 端點網址
    /// </summary>
    public string? Endpoint { get; set; }

    /// <summary>
    /// 部署名稱
    /// </summary>
    public string? DeploymentName { get; set; }

    /// <summary>
    /// 說明
    /// </summary>
    public string? Description { get; set; }
}