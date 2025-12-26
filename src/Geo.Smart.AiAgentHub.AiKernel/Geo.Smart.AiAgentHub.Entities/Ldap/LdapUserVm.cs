namespace Geo.Smart.AiAgentHub.Entities.Ldap;

/// <summary>
/// 透過LDAP取得的使用者資訊
/// </summary>
public class LdapUserVm
{
    /// <summary>
    /// CN - 完成姓名加上AD帳號
    /// </summary>
    public string Cn { get; set; } = string.Empty;

    /// <summary>
    /// AD 上的GUID，Object GUID
    /// </summary>
    public string UserId { get; set; } = string.Empty;

    /// <summary>
    /// AD 帳號
    /// </summary>
    public string Account { get; set; } = string.Empty;

    /// <summary>
    /// 電子郵件
    /// </summary>
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// 姓氏
    /// </summary>
    public string FamilyName { get; set; } = string.Empty;

    /// <summary>
    /// 名字
    /// </summary>
    public string GivenName { get; set; } = string.Empty;

    /// <summary>
    /// 完整姓名
    /// </summary>
    public string FullName
    {
        get
        {
            return $"{FamilyName}{GivenName}";
        }
    }

    /// <summary>
    /// 顯示用的英文名字
    /// </summary>
    public string ShowName
    {
        get
        {
            var cnSplit = Cn.Split(" ");
            if (cnSplit.Length == 2)
            {
                return cnSplit[1];
            }
            else
            {
                return Account;
            }
        }
    }

    /// <summary>
    /// 帳號控制，用來判斷是否已離職
    /// </summary>
    public int? AccountControl { get; set; }

    /// <summary>
    /// DN
    /// </summary>
    public string DistinguishedName { get; set; } = string.Empty;

    /// <summary>
    /// OU - LDAP 組織名稱
    /// </summary>
    public string Ou { get; set; } = string.Empty;
}