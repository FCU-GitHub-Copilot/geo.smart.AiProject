using Geo.Smart.AiAgentHub.Services.Common;
using Geo.Smart.AiAgentHub.Entities.Identity.Token;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using JsonWebToken = Microsoft.IdentityModel.JsonWebTokens;

namespace Geo.Smart.AiAgentHub.Services.Helpers;
/// <summary>
/// Bearer Token Base 驗證小幫手
/// </summary>
[DiLifetime(ServiceLifetime.Singleton)]
public class JwtHelper(IConfiguration _configuration)
{
    /// <summary>
    /// 產製 Token
    /// </summary>
    /// <param name="user">使用者</param>
    /// <param name="expireMinutes"></param>
    /// <returns></returns>
    public TokenVm GenerateToken(ApplicationUser user, int expireMinutes = 60)
    {
        var jwtSetting = _configuration.GetSection("JwtSettings").Get<JwtSetting>();
        var claims = new List<Claim>
        {
            new (ClaimTypes.NameIdentifier, user.Id),
            new (ClaimTypes.Name, user.UserName ?? string.Empty),
            new (ClaimTypes.Role, user.ApplicationRoles?.FirstOrDefault()?.Id ?? string.Empty),
            new (ClaimTypes.GroupSid, user.OrgId?.ToString() ?? string.Empty),

            new (JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new (JwtRegisteredClaimNames.Name, user.FullName)
        };

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Issuer = jwtSetting!.Issuer,
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.Now.AddMinutes(expireMinutes),
            SigningCredentials = new SigningCredentials(
                new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSetting.SignKey)),
                SecurityAlgorithms.HmacSha512Signature
            )
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        var serializeToken = tokenHandler.WriteToken(tokenHandler.CreateToken(tokenDescriptor));

        return new TokenVm
        {
            AccessToken = serializeToken,
            RefreshToken = GenerateRefreshToken(),
        };
    }

    /// <summary>
    /// 取得 JWT 的驗證參數
    /// </summary>
    /// <param name="jwtSetting">JWT 設定</param>
    /// <returns></returns>
    public static TokenValidationParameters GetTokenValidationParameters(JwtSetting jwtSetting)
    {
        return new TokenValidationParameters
        {
            // 透過這項宣告，就可以從 "sub" 取值並設定給 User.Identity.Name
            NameClaimType = ClaimTypes.NameIdentifier,
            // 透過這項宣告，就可以從 "roles" 取值，並可讓 [Authorize] 判斷角色
            RoleClaimType = ClaimTypes.Role,

            // 一般我們都會驗證 Issuer
            ValidateIssuer = true,
            ValidIssuer = jwtSetting.Issuer,
            // 通常不太需要驗證 Audience
            ValidateAudience = false,
            // 一般我們都會驗證 Token 的有效期間
            ValidateLifetime = true,
            // 如果 Token 中包含 key 才需要驗證，一般都只有簽章而已
            ValidateIssuerSigningKey = true,
            RequireSignedTokens = true,
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(jwtSetting.SignKey)),

            // 通常不太需要驗證 Audience
            // token 時效應該大於五分鐘，TimeSpan FromMinutes(5)
            ClockSkew = TimeSpan.Zero
        };
    }

    /// <summary>
    /// 取得 JWT 的驗證參數
    /// </summary>
    /// <returns></returns>
    public TokenValidationParameters GetTokenValidationParameters()
    {
        var jwtSetting = _configuration.GetSection("JwtSettings").Get<JwtSetting>();
        return GetTokenValidationParameters(jwtSetting!);
    }

    /// <summary>
    /// 驗證 JWT TOKEN
    /// </summary>
    /// <param name="token"></param>
    /// <returns></returns>
    public async Task<Result<UserInfo>> ValidateJwt(string token)
    {
        try
        {
            var validationParameters = GetTokenValidationParameters();

            var jsonWebTokenHandler = new JsonWebToken.JsonWebTokenHandler()
            {
                MapInboundClaims = true
            };
            var tokenResult = await jsonWebTokenHandler.ValidateTokenAsync(token, validationParameters);
            var claimsPrincipal = new ClaimsPrincipal(tokenResult.ClaimsIdentity);

            return ResultHelper.Success(UserHelper.GetUserInfo(claimsPrincipal));
        }
        catch
        {
            // 當 TOKEN 不合規時轉型失敗會拋出例外
            return ResultHelper.Failure<UserInfo>("Token 不合規!");
        }
    }

    /// <summary>
    /// 產生 refresh TOKEN
    /// </summary>
    /// <param name="length"></param>
    /// <returns></returns>
    public static string GenerateRefreshToken(int length = 32)
    {
        var chars = "ABCDEFGHIJKLMNPQRSTUVWXYZ123456789";
        return new string(Enumerable.Repeat(chars, length)
            .Select(x => x[RandomHelper.GetInt(x.Length)]).ToArray());
    }

    /// <summary>
    /// 取得 JWT 驗證後的 ClaimsPrincipal USER 資訊
    /// 此方法不驗 過期時間
    /// </summary>
    /// <param name="jwtToken"></param>
    /// <returns></returns>
    public Result<ClaimsPrincipal> GetJwtUser(string jwtToken)
    {
        try
        {
            var validationParameters = GetTokenValidationParameters();
            validationParameters.ValidateLifetime = false;
            var jwtHandler = new JwtSecurityTokenHandler();
            var user = jwtHandler.ValidateToken(jwtToken, validationParameters, out _);
            return ResultHelper.Success(user);
        }
        catch
        {
            // 當 TOKEN 不合規時轉型失敗會拋出例外
            return ResultHelper.Failure<ClaimsPrincipal>("JwtToken 不合規!");
        }
    }
}