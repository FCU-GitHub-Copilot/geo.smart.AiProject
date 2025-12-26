namespace Geo.Smart.AiAgentHub.AiKernel.Models.Vms;

/// <summary>
/// 聊天室詳細資訊的檢視模型
/// </summary>
public class ChatRoomDetailVm : ChatRoomVm
{
    /// <summary>
    /// 最後一次提問的 LLM 服務識別碼
    /// </summary>
    public string LlmServiceId { get; set; } = string.Empty;

    /// <summary>
    /// 使用者挑選的工具清單
    /// {"server1":["toolA","toolB"],"server2":["toolC"]}
    /// </summary>
    public Dictionary<string, List<string>> ToolSelected { get; set; } = [];

    /// <summary>
    /// 聊天室訊息清單
    /// </summary>
    public List<ChatMessageVm> ChatMessages { get; set; } = [];
}