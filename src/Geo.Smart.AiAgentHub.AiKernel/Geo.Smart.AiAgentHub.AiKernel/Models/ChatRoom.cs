using Geo.Smart.CommonCore.Models;

namespace Geo.Smart.AiAgentHub.AiKernel.Models;

public partial class ChatRoom : AuditableEntity
{
    /// <summary>
    /// 聊天室主鍵
    /// </summary>
    public Guid RoomId { get; set; } = Guid.NewGuid();

    /// <summary>
    /// 聊天室名稱
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// 聊天紀錄，ChatHistory序列化
    /// </summary>
    public string History { get; set; } = string.Empty;

    /// <summary>
    /// 最後一次提問的 LLM 服務識別碼
    /// </summary>
    public string LlmServiceId { get; set; } = string.Empty;

    /// <summary>
    /// 最後一次提問使用者選取的工具
    /// </summary>
    public string ToolSelected { get; set; } = "{}";

    // Reverse navigation
    public ICollection<ChatMessage> ChatMessages { get; set; } = new List<ChatMessage>();
}