using Geo.Smart.AiAgentHub.Services.Extension;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using System.Security.Claims;

namespace Geo.Smart.AiAgentHub.WebApi.Middlewares;
/// <summary>
/// 擴充使用者宣告
/// https://www.reflections-ibs.com/blog/articles/how-to-extend-asp-net-core-3-0-and-3-1-identity-user
/// </summary>
public class QooUserClaimsPrincipalFactory
    : UserClaimsPrincipalFactory<ApplicationUser, ApplicationRole>
{
    /// <summary>
    /// 建構式
    /// </summary>
    /// <param name="userManager"></param>
    /// <param name="roleManager"></param>
    /// <param name="options"></param>
    public QooUserClaimsPrincipalFactory(
        UserManager<ApplicationUser> userManager,
        RoleManager<ApplicationRole> roleManager,
        IOptions<IdentityOptions> options)
        : base(userManager, roleManager, options)
    {
    }

    /// <summary>
    /// 覆寫使用者宣告
    /// </summary>
    /// <param name="user"></param>
    /// <returns></returns>
    protected override async Task<ClaimsIdentity> GenerateClaimsAsync(ApplicationUser user)
    {
        var identity = await base.GenerateClaimsAsync(user);
        identity.AddClaim(new Claim(ClaimTypes.GroupSid, user.OrgId?.ToString() ?? string.Empty));
        identity.AddClaim(new Claim(ClaimTypesExt.MustChangeQooClaimType, user.MustChangeQoo.ToString()));
        return identity;
    }
}