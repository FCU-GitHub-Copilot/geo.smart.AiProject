namespace Geo.Smart.AiAgentHub.Entities.Vms.UserManage;

/// <summary>
/// 使用者登入紀錄資料 Vm
/// </summary>
public class UserLogVm
{
    /// <summary>
    /// 帳號
    /// </summary>
    public string? UserName { get; set; }

    /// <summary>
    /// 中文名稱
    /// </summary>
    public string FullName { get; set; } = string.Empty;

    /// <summary>
    /// IP
    /// </summary>
    public string Ip { get; set; } = "-";

    /// <summary>
    /// 操作時間
    /// </summary>
    public DateTime RequestTime { get; set; } = DateTime.Now;

    /// <summary>
    /// 是否登入成功
    /// </summary>
    public bool IsRequestResult { get; set; } = false;

    /// <summary>
    /// 登入結果文字（"成功" 或 "失敗"）
    /// </summary>
    public string LoginResultText => IsRequestResult ? "成功" : "失敗";
}