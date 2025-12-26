namespace Geo.Smart.AiAgentHub.AiKernel.Vm;

/// <summary>
/// AI 回覆紀錄 ViewModel
/// </summary>
public class ChatCompletionLogVm
{
    /// <summary>
    /// AI 回應的唯一值
    /// </summary>
    public string? LogId { get; set; }

    /// <summary>
    /// 記錄所有的 METADATA 內容
    /// </summary>
    public string? Metadata { get; set; }

    /// <summary>
    /// 紀錄時間
    /// </summary>
    public DateTimeOffset CreatedDate { get; set; }

    /// <summary>
    /// Prompt Token 數量
    /// </summary>
    public long? PromptToken { get; set; }

    /// <summary>
    /// Completion Token 數量
    /// </summary>
    public long? CompletionToken { get; set; }

    /// <summary>
    /// 總 Token 數量
    /// </summary>
    public long? TotalToken { get; set; }
}