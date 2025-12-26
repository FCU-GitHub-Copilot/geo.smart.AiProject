namespace Geo.Smart.AiAgentHub.AiKernel.Models.Vms;

/// <summary>
/// 聊天室清單 ViewModel
/// </summary>
public class ChatRoomVm
{
    /// <summary>
    /// 聊天室 ID
    /// </summary>
    public Guid RoomId { get; set; } = Guid.NewGuid();

    /// <summary>
    /// 聊天室名稱
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// 建立時間
    /// </summary>
    public DateTime CreatedDate { get; set; }

    /// <summary>
    /// 對話則數
    /// </summary>
    public int MessagesCount { get; set; } = 0;
}