namespace Geo.Smart.AiAgentHub.AiKernel.Models.Vms;

/// <summary>
/// 聊天室訊息內容
/// </summary>
public class ChatMessageVm
{
    /// <summary>
    /// 訊息主鍵
    /// </summary>
    public Guid MessageId { get; set; } = Guid.NewGuid();

    /// <summary>
    /// 發送者角色（user/system/ai）
    /// </summary>
    public string Role { get; set; } = string.Empty;

    /// <summary>
    /// 訊息內容
    /// </summary>
    public string Content { get; set; } = string.Empty;

    /// <summary>
    /// 訊息發送時間
    /// </summary>
    public DateTime SentAt { get; set; } = DateTime.Now;
}