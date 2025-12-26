using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace Geo.Smart.AiAgentHub.Services.Helpers;
/// <summary>
/// 使用者小助手
/// </summary>
public static class UserHelper
{
    /// <summary>
    /// 取得 JWT Token 中的使用者資訊
    /// </summary>
    /// <param name="claimsPrincipal"></param>
    /// <returns></returns>
    public static UserInfo GetUserInfo(ClaimsPrincipal claimsPrincipal)
    {
        return new UserInfo
        {
            UserId = claimsPrincipal.FindFirstValue(ClaimTypes.NameIdentifier) ?? string.Empty,
            Account = claimsPrincipal.FindFirstValue(ClaimTypes.Name) ?? string.Empty,
            RoleId = claimsPrincipal.FindFirstValue(ClaimTypes.Role) ?? string.Empty,
            FullName = claimsPrincipal.FindFirstValue(JwtRegisteredClaimNames.Name) ?? string.Empty
        };
    }
}