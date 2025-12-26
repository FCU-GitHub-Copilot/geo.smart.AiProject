#pragma warning disable CS1591 // 遺漏公用可見類型或成員的 XML 註解

namespace Geo.Smart.AiAgentHub.Infras.Enums;

/// <summary>
/// 登入方式，0:帳密登入,1:LDAP,2:SSO
/// </summary>
public enum LoginType
{
    帳密登入,
    LDAP,
    SSO
}