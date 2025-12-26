namespace Geo.Smart.AiAgentHub.Entities.Vms.AiAgent;

/// <summary>
/// LLM 詳細資料 ViewModel
/// </summary>
public class LlmDetailVm
{
    /// <summary>
    /// LLM ID
    /// </summary>
    public Guid LlmId { get; set; }

    /// <summary>
    /// 擁有者 UserId
    /// </summary>
    public string UserId { get; set; } = string.Empty;

    /// <summary>
    /// 模型管理名稱、服務識別碼
    /// </summary>
    public string ServiceId { get; set; } = string.Empty;

    /// <summary>
    /// LLM 模型名稱
    /// </summary>
    public string ModelId { get; set; } = string.Empty;

    /// <summary>
    /// LLM 來源類型
    /// </summary>
    public LlmSourceType LlmSourceType { get; set; }

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

    /// <summary>
    /// 擁有者姓名
    /// </summary>
    public string? UserName { get; set; }
}