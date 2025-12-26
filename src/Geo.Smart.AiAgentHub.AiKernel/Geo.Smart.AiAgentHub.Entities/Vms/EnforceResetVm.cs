namespace Geo.Smart.AiAgentHub.Entities.Vms;

/// <summary>
/// 產製出的 captcha 資訊
/// </summary>
public class EnforceResetVm
{
    /// <summary>
    /// 強制變更密碼Id
    /// </summary>
    public string EnforceId { get; set; } = string.Empty;

    /// <summary>
    /// 新密碼
    /// </summary>
    public string NewQoo { get; set; } = string.Empty;

    /// <summary>
    /// 確認新密碼
    /// </summary>
    public string ConfirmQoo { get; set; } = string.Empty;
}