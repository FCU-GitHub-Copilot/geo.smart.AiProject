namespace Geo.Smart.AiAgentHub.Entities.Vms.UserManage;

/// <summary>
/// 樹狀組織物件
/// </summary>
public class OrgTreeVm
{
    /// <summary>
    /// 組織 Id
    /// </summary>
    public Guid OrgId { get; set; }

    /// <summary>
    /// 組織名稱
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// 子組織
    /// </summary>
    public List<OrgTreeVm>? SubOrg { get; set; }
}