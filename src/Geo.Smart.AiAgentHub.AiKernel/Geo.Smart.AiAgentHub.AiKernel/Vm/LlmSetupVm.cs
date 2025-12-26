using Geo.Smart.AiAgentHub.Infras.Enums;

namespace Geo.Smart.AiAgentHub.AiKernel.Vm;

/// <summary>
/// 記錄 LLM 服務設定資訊
/// </summary>
public class LlmSetupVm
{
    /// <summary>
    /// 模型管理名稱、服務識別碼，必要，建議使用服務來源與 ModelId 組合
    /// </summary>
    public string ServiceId { get; set; } = string.Empty;

    /// <summary>
    /// LLM 模型名稱，必要，例如：gpt-4o、gpt-4o-mini
    /// </summary>
    public string ModelId { get; set; } = string.Empty;

    /// <summary>
    ///  LLM 服務來源類型
    /// </summary>
    public LlmSourceType LlmSourceType { get; set; } = LlmSourceType.OpenAi;

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
}