namespace Geo.Smart.AiAgentHub.Entities.Vms;

/// <summary>
/// 使用者代理資訊 ViewModel
/// </summary>
public class UserAgentVm
{
    /// <summary>
    /// 原始代理資訊
    /// </summary>
    public string UserAgent { get; set; } = string.Empty;

    /// <summary>
    /// IP
    /// </summary>
    public string Ip { get; set; } = string.Empty;

    /// <summary>
    /// 瀏覽器類型
    /// </summary>
    public string Browser { get; set; } = string.Empty;

    /// <summary>
    /// 作業平台
    /// </summary>
    public string Os { get; set; } = string.Empty;
}