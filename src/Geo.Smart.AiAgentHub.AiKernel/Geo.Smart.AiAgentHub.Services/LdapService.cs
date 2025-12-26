using Geo.Smart.AiAgentHub.Entities.Ldap;
using Geo.Smart.AiAgentHub.Services.Common;
using Geo.Smart.AiAgentHub.Services.Contracts;
using Geo.Smart.AiAgentHub.Services.Helpers;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Geo.Smart.AiAgentHub.Services;

/// <summary>
/// 提供 LDAP 同步與管理功能的服務類別
/// </summary>
/// <param name="dbModel">資料庫內容物件</param>
/// <param name="_configuration">組態設定物件</param>
public class LdapService(GdbContext dbModel,
    ILogger<CommonService> _logger,
    IConfiguration _configuration
    ) : BaseService(dbModel, _logger), ILdapService
{
    /// <summary>
    /// 執行 LDAP 使用者同步作業
    /// </summary>
    /// <returns>回傳同步是否成功</returns>
    public async Task<bool> LdapSync()
    {
        var ldap = _configuration.GetSection("LDAP").Get<LdapSetting>();
        if (ldap == null)
        {
            return false;
        }
        var ldapUsers = await GeoLdapHelper.GetLdapUsers(ldap);
        var dbUsers = await DbModel.ApplicationUsers
            .Include(x => x.Organization)
            .Where(x => x.IsEnabled)
            .Where(x => x.PasswordHash == null)
            .ToListAsync();

        // dbUsers 有，ldapUsers 沒有，則刪除
        var deleteUsers = dbUsers.Where(x =>
        {
            return !ldapUsers.Any(y => y.UserId == x.Id);
        }).ToList();

        deleteUsers.ForEach(x =>
        {
            try
            {
                x.IsEnabled = false;
                DbModel.SaveChanges();
            }
            catch (Exception ex)
            {
                LogError(ex, $"{nameof(LdapSync)}.DeleteUsers");
            }
        });

        // dbUsers 沒有，ldapUsers 有，則新增
        var addUsers = ldapUsers.Where(x => !dbUsers.Any(y => y.Id == x.UserId)).ToList();
        addUsers.ForEach(x => CreateUserFromLdap(x));

        // dbUsers 有，ldapUsers 有，則更新
        var updateUsers = dbUsers.Join(ldapUsers, x => x.Id, y => y.UserId, (dbUser, ldapUser) => new { dbUser, ldapUser }).ToList();
        updateUsers.ForEach(x => UpdateUserFromLdap(x.dbUser, x.ldapUser));

        return true;
    }

    /// <summary>
    /// 新增從 LDAP 同步過來的使用者
    /// </summary>
    /// <param name="ldapUser">LDAP 使用者物件</param>
    private void CreateUserFromLdap(LdapUserVm ldapUser)
    {
        try
        {
            // 如果 LDAP 回傳的 AccountControl 為 null, 則視為停用, 即是 2
            var accountControl = ldapUser.AccountControl ?? 2;
            // 旗標內有 2 的話, 表示此帳號已停用
            if ((accountControl & 2) == 2)
            {
                return;
            }

            var org = DbModel.Organizations
                .Where(x => x.IsEnabled)
                .Where(x => x.Name == ldapUser.Ou)
                .FirstOrDefault();
            if (org == null)
            {
                Logger?.LogError("Error {MethodName}：查無此 {OuName} 組織", nameof(CreateUserFromLdap), ldapUser.Ou);
                return;
            }

            var dbUser = DbModel.ApplicationUsers
                .FirstOrDefault(x => x.Id == ldapUser.UserId);
            if (dbUser != null)
            {
                // 如果已有舊資料(曾經 IsEnabled=false)，即更新
                UpdateUserFromLdap(dbUser, ldapUser);
            }
            else
            {
                var user = new ApplicationUser
                {
                    Id = ldapUser.UserId,
                    UserName = ldapUser.Account.ToLower(),
                    NormalizedUserName = ldapUser.Account.ToUpper(),
                    Email = ldapUser.Email.ToLower(),
                    NormalizedEmail = ldapUser.Email.ToUpper(),
                    EmailConfirmed = true,
                    FullName = ldapUser.Cn,
                    DistinguishedName = ldapUser.DistinguishedName,
                    CreatedDate = DateTime.Now,
                    LoginType = LoginType.LDAP,
                    OrgId = org.OrgId,
                    IsRegisterVerify = true,
                };
                DbModel.Users.Add(user);

                DbModel.SaveChanges();
            }

            DbModel.SaveChanges();
        }
        catch (Exception ex)
        {
            LogError(ex, nameof(CreateUserFromLdap));
        }
    }

    /// <summary>
    /// 更新 LDAP 使用者資訊
    /// </summary>
    /// <param name="user">資料庫使用者物件</param>
    /// <param name="ldapUser">LDAP 使用者物件</param>
    private void UpdateUserFromLdap(ApplicationUser user, LdapUserVm ldapUser)
    {
        try
        {
            // 不更新組織
            user.UserName = ldapUser.Account.ToLower();
            user.NormalizedUserName = ldapUser.Account.ToUpper();
            user.Email = ldapUser.Email.ToLower();
            user.NormalizedEmail = ldapUser.Email.ToUpper();
            user.FullName = ldapUser.Cn;
            user.DistinguishedName = ldapUser.DistinguishedName;
            user.IsRegisterVerify = true;

            // 如果 LDAP 回傳的 AccountControl 為 null, 則視為停用, 即是 2
            var accountControl = ldapUser.AccountControl ?? 2;
            // 旗標內有 2 的話, 表示此帳號已停用
            user.IsEnabled = ((accountControl & 2) != 2);

            DbModel.SaveChanges();
        }
        catch (Exception ex)
        {
            LogError(ex, nameof(UpdateUserFromLdap));
        }
    }
}