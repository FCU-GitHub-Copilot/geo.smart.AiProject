using Geo.Smart.CommonCore.Models;

namespace Geo.Smart.AiAgentHub.AiKernel.Models;

public partial class ChatMessage : AuditableEntity
{
    /// <summary>
    /// 訊息主鍵
    /// </summary>
    public Guid MessageId { get; set; } = Guid.NewGuid();

    /// <summary>
    /// 聊天室主鍵
    /// </summary>
    public Guid RoomId { get; set; }

    /// <summary>
    /// 發送者角色（user/system/assistant）
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

    /// <summary>
    /// LLM 服務識別碼
    /// </summary>
    public string LlmServiceId { get; set; } = string.Empty;

    /// <summary>
    /// AI 回應的唯一值
    /// </summary>
    public string? LogId { get; set; }

    /// <summary>
    /// 總 Token 數量
    /// </summary>
    public long? Tokens { get; set; }

    /// <summary>
    /// 提問使用時者選取的工具
    /// </summary>
    public string? ToolSelected { get; set; } = "{}";

    public ChatRoom ChatRoom { get; set; }
}