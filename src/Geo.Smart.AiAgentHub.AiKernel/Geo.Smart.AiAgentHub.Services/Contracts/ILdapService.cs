namespace Geo.Smart.AiAgentHub.Services.Contracts;

/// <summary>
/// 提供 LDAP 相關服務的介面
/// </summary>
public interface ILdapService
{
    /// <summary>
    /// 執行 LDAP 同步作業
    /// </summary>
    /// <returns>回傳同步是否成功</returns>
    Task<bool> LdapSync();
}