namespace Geo.Smart.AiAgentHub.Entities.Vms.Filex;

/// <summary>
/// 檔案資訊
/// </summary>
public partial class FilexVm
{
    /// <summary>
    /// 檔案唯一Id
    /// </summary>
    public Guid FileId { get; set; } = Guid.NewGuid();

    /// <summary>
    /// 檔案名稱
    /// </summary>
    public string FileName { get; set; } = string.Empty;

    /// <summary>
    /// 副檔名
    /// </summary>
    public string Extension { get; set; } = string.Empty;

    /// <summary>
    /// 檔案大小
    /// </summary>
    public long Size { get; set; }

    /// <summary>
    /// 檔案類型
    /// </summary>
    public string ContentType { get; set; } = string.Empty;

    /// <summary>
    /// 備註
    /// </summary>
    public string? Note { get; set; }

    /// <summary>
    /// 流水號
    /// </summary>
    public int FileSeq { get; set; }

    /// <summary>
    /// 建立者
    /// </summary>
    public string CreatedBy { get; set; } = string.Empty;

    /// <summary>
    /// 新增時間
    /// </summary>
    public DateTime CreatedDate { get; set; }

    /// <summary>
    /// 修改者
    /// </summary>
    public string UpdatedBy { get; set; } = string.Empty;

    /// <summary>
    /// 修改時間
    /// </summary>
    public DateTime UpdatedDate { get; set; }
}