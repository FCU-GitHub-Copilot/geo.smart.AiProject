namespace Geo.Smart.AiAgentHub.Entities.Ldap;

/// <summary>
/// LDAP 連線設定
/// </summary>
public class LdapSetting
{
    /// <summary>
    /// LDAP 服務位置
    /// </summary>
    public string Server { get; set; } = string.Empty;

    /// <summary>
    /// LDAP Port
    /// </summary>
    public int Port { get; set; }

    /// <summary>
    /// LDAP Sync 帳號
    /// </summary>
    public string User { get; set; } = string.Empty;

    /// <summary>
    /// Qoo
    /// </summary>
    public string Qoo { get; set; } = string.Empty;
}