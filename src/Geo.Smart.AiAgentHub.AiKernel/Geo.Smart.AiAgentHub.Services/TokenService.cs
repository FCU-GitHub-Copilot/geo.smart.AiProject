using Geo.Smart.AiAgentHub.Entities.Identity.Token;
using Geo.Smart.AiAgentHub.Entities.Ldap;
using Geo.Smart.AiAgentHub.Infras;
using Geo.Smart.AiAgentHub.Services.Common;
using Geo.Smart.AiAgentHub.Services.Contracts;
using Geo.Smart.AiAgentHub.Services.Helpers;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;

namespace Geo.Smart.AiAgentHub.Services;

/// <summary>
/// Token 服務，負責驗證碼產生、登入、權杖刷新等相關功能
/// </summary>
public class TokenService(GdbContext dbModel,
    ILogger<CommonService> _logger,
    IConfiguration _configuration,
    SignInManager<ApplicationUser> _signInManager,
    UserManager<ApplicationUser> _userManager,
    JwtHelper _jwtHelper,
    CaptchaHelper _captchaHelper,
    VerifyHelper _verifyHelper
    ) : BaseService(dbModel, _logger), ITokenService
{
    /// <summary>
    /// 取得驗證碼
    /// </summary>
    /// <returns>驗證碼結果物件</returns>
    public async Task<Result<CaptchaVm>> Captcha()
    {
        try
        {
            return await _captchaHelper.Captcha();
        }
        catch (Exception e)
        {
            Logger?.LogError(e, e.Message);
            return ResultHelper.Failure<CaptchaVm>(e.Message);
        }
    }

    /// <summary>
    /// 使用者登入
    /// </summary>
    /// <param name="login">登入資訊 ViewModel</param>
    /// <returns>登入結果物件</returns>
    public async Task<Result<LoginResultVm>> Login(LoginViewModel login)
    {
        var (status, user) = await ValidateUser(login);
        if (status == null && user != null)
        {
            var token = _jwtHelper.GenerateToken(user);
            // 紀錄 RefreshToken
            await SaveRefreshToken(user, token);
            // 紀錄登入次數
            await SaveLoginTimes(user);
            return ResultHelper.Success(new LoginResultVm
            {
                AccessToken = token.AccessToken,
                RefreshToken = token.RefreshToken,
                Status = ConstantData.CommonConst.LoginSuccess,
            });
        }
        else if (status == LoginFailStatus.需強制修改密碼 && user != null)
        {
            var verifyType = user.MustChangeQoo
                ? VerifyType.第一次登入
                : VerifyType.三個月強制變更密碼;
            var verifyId = await _verifyHelper.GenerateCode(
                user.Id, verifyType
            );

            var result = ReturnLoginFalse(status.Value);
            result.Data.AccessToken = verifyId.ToString();
            return result;
        }
        else
        {
            return ReturnLoginFalse(status!.Value);
        }
    }

    /// <summary>
    /// 驗證使用者帳號密碼
    /// </summary>
    /// <param name="login">登入資訊 ViewModel</param>
    /// <returns>登入失敗狀態與使用者物件</returns>
    private async Task<(LoginFailStatus? status, ApplicationUser? user)> ValidateUser(LoginViewModel login)
    {
        // 驗證碼驗證
        if (!login.UseOtp
            && !(await _captchaHelper.Validate(login.CaptchaId, login.Captcha)))
        {
            return (LoginFailStatus.驗證碼不符, null);
        }
        var user = await _userManager.FindByNameAsync(login.UserName);
        if (user == null)
        {
            return (LoginFailStatus.登入失敗, null);
        }

        var result = await _signInManager.CheckPasswordSignInAsync(
            user, login.Qoo, true
        );

        if (result.Succeeded)
        {
            if (!user.IsEnabled)
            {
                return (LoginFailStatus.帳號已停用, null);
            }

            if (user.MustChangeQoo)
            {
                return (LoginFailStatus.需強制修改密碼, user);
            }
            if (user.LastChangeQoo < DateTime.Now.AddMonths(-3))
            {
                return (LoginFailStatus.需強制修改密碼, user);
            }
            user = await GetUserByAccount(login.UserName);
            return (null, user);
        }
        else if (result.RequiresTwoFactor)
        {
            return (LoginFailStatus.未通過二階段驗證, null);
        }
        else if (result.IsLockedOut)
        {
            Logger?.LogWarning("User account locked out.");
            return (LoginFailStatus.帳號已鎖定, null);
        }
        else if (result.IsNotAllowed)
        {
            return (LoginFailStatus.不允許登入, null);
        }
        else
        {
            return (LoginFailStatus.登入失敗, null);
        }
    }

    /// <summary>
    /// 使用 AD 帳密登入，取得 TOKEN
    /// </summary>
    /// <param name="login">登入 VM</param>
    /// <returns></returns>
    public async Task<Result<LoginResultVm>> Ldap(LoginViewModel login)
    {
        if (!(await _captchaHelper.Validate(login.CaptchaId, login.Captcha)))
        {
            return ReturnLoginFalse(LoginFailStatus.驗證碼不符);
        }

        var user = await GetUserByAccount(login.UserName);
        if (user == null
            || user.LoginType != LoginType.LDAP
            || string.IsNullOrEmpty(user.DistinguishedName))
        {
            return ReturnLoginFalse(LoginFailStatus.登入失敗);
        }

        var loginSuccess = await GeoLdapHelper.Login(new LdapSetting
        {
            Qoo = login.Qoo,
            //改用【專用名稱】登入
            User = user.DistinguishedName,
            Server = _configuration.GetSection("LDAP").Get<LdapSetting>()?.Server
                ?? string.Empty
        });

        if (loginSuccess && user.IsEnabled)
        {
            var token = _jwtHelper.GenerateToken(user);
            // 紀錄 RefreshToken
            await SaveRefreshToken(user, token);
            // 紀錄登入次數
            await SaveLoginTimes(user);
            return ResultHelper.Success(new LoginResultVm
            {
                AccessToken = token.AccessToken,
                RefreshToken = token.RefreshToken,
                Status = ConstantData.CommonConst.LoginSuccess,
            });
        }
        else
        {
            return ReturnLoginFalse(LoginFailStatus.登入失敗);
        }
    }

    /// <summary>
    /// 刷新權杖
    /// </summary>
    /// <param name="vm">Token ViewModel</param>
    /// <returns>登入結果物件</returns>
    public async Task<Result<LoginResultVm>> Refresh(TokenVm vm)
    {
        var userRefreshToken = await GetUserRefreshToken(vm);
        if (userRefreshToken != null)
        {
            var user = await GetUserById(userRefreshToken.UserId);
            if (user != null)
            {
                var tokenVm = _jwtHelper.GenerateToken(user);
                await SaveRefreshToken(user, tokenVm);
                return ResultHelper.Success(new LoginResultVm
                {
                    AccessToken = tokenVm.AccessToken,
                    RefreshToken = tokenVm.RefreshToken,
                    Status = ConstantData.CommonConst.LoginSuccess,
                });
            }
        }
        return ReturnLoginFalse(LoginFailStatus.更新權杖已過期);
    }

    /// <summary>
    /// 檢查 Email 是否可用
    /// </summary>
    /// <param name="email">電子郵件地址</param>
    /// <returns>結果物件，Data 為 true 表示可用</returns>
    public async Task<Result<object>> IsEmailCanUse(string email)
    {
        try
        {
            //會員已經驗證 或是 驗證中(未驗證且資料庫有驗證碼) -> Email 無法使用
            var user = await DbModel.Users.AsNoTracking().FirstOrDefaultAsync(r =>
                r.Email == email
                && (r.IsRegisterVerify
                    || (!r.IsRegisterVerify && !string.IsNullOrEmpty(r.RegisterVerifyCode))
                ));

            return new Result<object>(true) { Data = user == null };
        }
        catch (Exception e)
        {
            Logger?.LogError(e, e.Message);
            return new Result<object>(e.Message);
        }
    }

    /// <summary>
    /// 驗證 JWT 權杖是否合法
    /// </summary>
    /// <param name="token">JWT 權杖字串</param>
    /// <returns>是否驗證通過</returns>
    public bool ValidateJwt(string token)
    {
        var validationParameters = _jwtHelper.GetTokenValidationParameters();

        try
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            tokenHandler.ValidateToken(token, validationParameters,
                out SecurityToken validatedToken);
            // 當 validatedToken 轉換成 JwtSecurityToken 失敗時，表示 token 不合規
            var jwtToken = (JwtSecurityToken)validatedToken;
            return jwtToken != null;
        }
        catch
        {
            return false;
        }
    }

    /// <summary>
    /// 儲存 RefreshToken
    /// </summary>
    /// <param name="user"></param>
    /// <param name="vm"></param>
    /// <returns></returns>
    private async Task<int> SaveRefreshToken(ApplicationUser user, TokenVm vm)
    {
        DbModel.UserTokenxes.Add(new UserTokenx
        {
            TokenId = Guid.NewGuid(),
            AccessToken = vm.AccessToken,
            ReFreshToken = vm.RefreshToken,
            UserId = user.Id,
            UserName = user.UserName!,
            IsEnabled = true,
            CreatedDate = DateTime.Now,
            ExpiredDate = DateTime.Today.AddDays(1),
        });
        return await DbModel.SaveChangesAsync();
    }

    /// <summary>
    /// 儲存 登入次數
    /// </summary>
    /// <param name="user"></param>
    /// <returns></returns>
    private async Task<int> SaveLoginTimes(ApplicationUser user)
    {
        var userInfo = await DbModel.ApplicationUsers.FirstOrDefaultAsync(x => x.Id == user.Id);
        if (userInfo != null)
        {
            userInfo.LoginTimes++;
            userInfo.LastLogin = DateTime.Now;
        }
        return await DbModel.SaveChangesAsync();
    }

    /// <summary>
    /// 取得 User RefreshToken 紀錄
    /// </summary>
    /// <param name="vm"></param>
    /// <returns></returns>
    private async Task<UserTokenx?> GetUserRefreshToken(TokenVm vm)
    {
        var result = _jwtHelper.GetJwtUser(vm.AccessToken);
        if (!result.Success)
        {
            return null;
        }

        var userInfo = UserHelper.GetUserInfo(result.Data);
        if (userInfo == null)
        {
            return null;
        }

        var userToken = await DbModel.UserTokenxes.FirstOrDefaultAsync(x =>
            x.IsEnabled
            && x.ExpiredDate > DateTime.Now
            && x.ReFreshToken == vm.RefreshToken
            && x.AccessToken == vm.AccessToken
            && x.UserId == userInfo.UserId);
        if (userToken != null)
        {
            userToken.IsEnabled = false;
            await DbModel.SaveChangesAsync();
        }
        return userToken;
    }

    /// <summary>
    /// 取得User
    /// </summary>
    /// <param name="userId"></param>
    /// <returns></returns>
    private async Task<ApplicationUser?> GetUserById(string userId)
    {
        return await DbModel.Users.Include(x => x.ApplicationRoles).AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == userId && x.IsEnabled && x.IsRegisterVerify);
    }

    /// <summary>
    /// 由帳號取得User
    /// </summary>
    /// <param name="userName"></param>
    /// <returns></returns>
    private async Task<ApplicationUser?> GetUserByAccount(string userName)
    {
        return await DbModel.Users
            .Include(x => x.ApplicationRoles)
            .AsNoTracking()
            .FirstOrDefaultAsync(r => r.UserName == userName && r.IsEnabled);
    }

    /// <summary>
    /// 回傳登入失敗資訊
    /// </summary>
    /// <param name="status">登入失敗狀態</param>
    /// <returns>登入失敗結果物件</returns>
    private static Result<LoginResultVm> ReturnLoginFalse(LoginFailStatus status)
    {
        return new Result<LoginResultVm>(false)
        {
            Data = new LoginResultVm
            {
                AccessToken = null,
                Status = status.ToString()
            },
            Message = status.ToString(),
            ErrorCode = (int)status
        };
    }
}