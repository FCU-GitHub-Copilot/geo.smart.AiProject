namespace Geo.Smart.AiAgentHub.AiKernel.Models;

public partial class ChatCompletionLog
{
    /// <summary>
    /// 聊天紀錄流水號
    /// </summary>
    public int LogSeq { get; set; }

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
    public DateTimeOffset CreatedDate { get; set; } = DateTimeOffset.Now;

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