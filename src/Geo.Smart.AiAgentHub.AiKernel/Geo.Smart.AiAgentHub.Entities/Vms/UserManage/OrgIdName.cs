namespace Geo.Smart.AiAgentHub.Entities.Vms.UserManage;

/// <summary>
/// 組織 Id 名稱
/// </summary>
public class OrgIdName
{
    /// <summary>
    ///  組織 Id
    /// </summary>
    public Guid OrgId { get; set; }

    /// <summary>
    /// 名稱
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// 上層組織 Id
    /// </summary>
    public Guid? Upper { get; set; }
}