namespace Geo.Smart.AiAgentHub.AiKernel.Vm;

/// <summary>
/// 使用者提問內容
/// </summary>
public class AskVm
{
    /// <summary>
    /// 聊天室 ID，沒有的話要自動建立一個聊天室
    /// </summary>
    public Guid? RoomId { get; set; }

    /// <summary>
    /// 取得或設定提問內容
    /// </summary>
    public required string Message { get; set; }

    /// <summary>
    /// 服務識別碼，必要，建議使用服務來源與 ModelId 組合
    /// </summary>
    public string ServiceId { get; set; } = string.Empty;

    /// <summary>
    /// 使用者挑選的工具清單
    /// </summary>
    public Dictionary<string, List<string>> ToolSelected { get; set; } = [];

    /// <summary>
    /// SingleR 的連線 ID
    /// </summary>
    public string ConnectionId { get; set; } = string.Empty;

    /// <summary>
    /// 參考圖片位置清單
    /// </summary>
    public List<string> Images { get; set; } = [];
}