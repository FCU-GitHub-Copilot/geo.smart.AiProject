namespace Geo.Smart.AiAgentHub.Entities.Vms.Photo;

/// <summary>
/// 檔案資訊
/// </summary>
public partial class PhotoVm
{
    /// <summary>
    /// 照片唯一ID
    /// </summary>
    public Guid PhotoId { get; set; } = Guid.NewGuid();

    /// <summary>
    /// 圖片名稱
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
    /// 緯度
    /// </summary>
    public double? Lat { get; set; }

    /// <summary>
    /// 經度
    /// </summary>
    public double? Lon { get; set; }

    /// <summary>
    /// 流水號
    /// </summary>
    public int PhotoSeq { get; set; }

    /// <summary>
    /// 上傳者
    /// </summary>
    public string CreatedBy { get; set; } = string.Empty;

    /// <summary>
    /// 上傳日期
    /// </summary>
    public DateTime CreatedDate { get; set; } = DateTime.Now;
}