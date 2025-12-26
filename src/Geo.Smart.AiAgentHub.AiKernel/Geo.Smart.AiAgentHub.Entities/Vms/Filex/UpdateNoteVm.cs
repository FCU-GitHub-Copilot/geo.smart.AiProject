namespace Geo.Smart.AiAgentHub.Entities.Vms.Filex;

/// <summary>
/// 更新檔案的說明
/// </summary>
public class UpdateNoteVm
{
    /// <summary>
    /// 檔案 ID
    /// </summary>
    public Guid FileId { get; set; }

    /// <summary>
    /// 檔案說明
    /// </summary>
    public string Note { get; set; } = string.Empty;
}