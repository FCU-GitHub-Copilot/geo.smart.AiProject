namespace Geo.Smart.AiAgentHub.Entities.Ldap;

/// <summary>
/// LDAP 回傳個人資料
/// </summary>
public class LdapPersonOri
{
    /// <summary>
    /// 唯一辨識名稱
    /// </summary>
    public string DistinguishedName { get; set; } = string.Empty;

    /// <summary>
    /// 使用者屬性資料
    /// </summary>
    public PersonAttributes Attributes { get; set; } = new PersonAttributes();

    /// <summary>
    /// LDAP 控制項集合
    /// </summary>
    public object[] Controls { get; set; } = [];
}

/// <summary>
/// LDAP 使用者資料 ViewModel
/// </summary>
public class PersonAttributes
{
    /// <summary>
    /// 顯示名稱
    /// </summary>
    public string[] DisplayName { get; set; } = [];

    /// <summary>
    /// 帳號控制屬性
    /// </summary>
    public string[] UserAccountControl { get; set; } = [];

    /// <summary>
    /// 使用者 CN 屬性
    /// </summary>
    public string[] CN { get; set; } = [];

    /// <summary>
    /// 使用者信箱
    /// </summary>
    public string[] Mail { get; set; } = [];

    /// <summary>
    /// 姓氏
    /// </summary>
    public string[] Sn { get; set; } = [];

    /// <summary>
    /// 名字
    /// </summary>
    public string[] GivenName { get; set; } = [];

    /// <summary>
    /// SAM 帳號名稱
    /// </summary>
    public string[] SAMAccountName { get; set; } = [];

    /// <summary>
    /// 物件 GUID 集合
    /// </summary>
    public List<byte[]> ObjectGUID { get; set; } = new List<byte[]>();
}