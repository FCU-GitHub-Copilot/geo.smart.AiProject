using Geo.Smart.AiAgentHub.Entities.Ldap;
using Novell.Directory.Ldap;

namespace Geo.Smart.AiAgentHub.Services.Helpers;

/// <summary>
/// GEO LDAP 登入與取得使用者清單
/// </summary>
public static class GeoLdapHelper
{
    /// <summary>
    /// 登入 GEO LDAP
    /// </summary>
    /// <param name="ldap">LDAP 資訊</param>
    /// <returns>是否登入成功</returns>
    public static async Task<bool> Login(LdapSetting ldap)
    {
        try
        {
            using (var cn = new LdapConnection())
            {
                await cn.ConnectAsync(ldap.Server, 389);
                await cn.BindAsync(ldap.User, ldap.Qoo);
            }
            return true;
        }
        catch (Exception)
        {
            // LDAP 登入失敗，靜態類別無法注入 Logger
            // 建議將此方法移至非靜態的 Helper 類別以支援日誌記錄
            return false;
        }
    }

    /// <summary>
    /// 取得組織內的使用者
    /// </summary>
    /// <param name="ldap">LDAP 資訊</param>
    /// <returns>使用者清單</returns>
    public static async Task<List<LdapUserVm>> GetLdapUsers(LdapSetting ldap)
    {
        var orgs = new List<string> {
            "GEO_SMART"
        };
        var users = new List<LdapUserVm>();

        foreach (var org in orgs)
        {
            var dc = $"OU={org},DC=geo,DC=local";
            users.AddRange(await GetUsersByDc(dc, org, ldap));
        }
        return users;
    }

    /// <summary>
    /// 連入 LDAP 取得使用者資訊
    /// </summary>
    /// <param name="dc">DC 資訊</param>
    /// <param name="ou">OU 資訊</param>
    /// <param name="ldap">LDAP 資訊</param>
    /// <returns>使用者清單</returns>
    private static async Task<List<LdapUserVm>> GetUsersByDc(string dc, string ou, LdapSetting ldap)
    {
        using (var cn = new LdapConnection())
        {
            await cn.ConnectAsync(ldap.Server, ldap.Port);

            await cn.BindAsync(ldap.User, ldap.Qoo);
            var searchFilter = "(objectClass=person)";
            var attributes = new string[]
            {
                "cn", "userAccountControl", "displayname", "mail",
                "givenName","sn", "sAMAccountName", "objectGUID",
                "distinguishedName"
            };
            var result = await cn.SearchAsync(
               dc,
               LdapConnection.ScopeSub,
               searchFilter,
               attributes,
               false
            );

            var users = new List<LdapUserVm>();
            while (await result.HasMoreAsync())
            {
                var entry = await result.NextAsync();
                users.Add(new LdapUserVm
                {
                    Cn = GetAttributeString(entry, "cn"),
                    UserId = GetAttributeGuidString(entry, "objectGUID"),
                    Account = GetAttributeString(entry, "sAMAccountName"),
                    Email = GetAttributeString(entry, "mail"),
                    FamilyName = GetAttributeString(entry, "sn"),
                    GivenName = GetAttributeString(entry, "givenName"),
                    DistinguishedName = GetAttributeString(entry, "distinguishedName"),
                    AccountControl = NullableTryParseInt32(GetAttributeString(entry, "userAccountControl")),
                    Ou = ou
                });
            }
            return users;
        }
    }

    /// <summary>
    /// 從 LDAP 取得的 Attribute 字串
    /// </summary>
    /// <param name="le">LDAP Entry</param>
    /// <param name="key">屬性名稱</param>
    /// <returns>屬性值</returns>
    private static string GetAttributeString(LdapEntry le, string key)
    {
        // 防呆：先檢查 Attribute 是否存在
        if (!le.GetAttributeSet().ContainsKey(key))
        {
            return string.Empty;
        }

        var attr = le.Get(key);
        if (attr != null)
        {
            return attr.StringValue;
        }
        else
        {
            return string.Empty;
        }
    }

    /// <summary>
    /// 從 LDAP 取得的 Attribute Byte，轉GUID
    /// </summary>
    /// <param name="le">LDAP Entry</param>
    /// <param name="key">屬性名稱</param>
    /// <returns>GUID 字串</returns>
    private static string GetAttributeGuidString(LdapEntry le, string key)
    {
        var attr = le.Get(key);
        if (attr != null)
        {
            return new Guid(attr.ByteValue).ToString();
        }
        else
        {
            return string.Empty;
        }
    }

    /// <summary>
    /// 解析 INT
    /// </summary>
    /// <param name="text">輸入值</param>
    /// <returns>解析後的整數</returns>
    public static int? NullableTryParseInt32(string text)
    {
        return int.TryParse(text, out int value) ? value : null;
    }
}