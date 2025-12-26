using Geo.Smart.AiAgentHub.Entities.Vms.Profile;
using Geo.Smart.AiAgentHub.Services.Common;
using Geo.Smart.AiAgentHub.Services.Contracts;
using Geo.Smart.AiAgentHub.Services.Helpers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Geo.Smart.AiAgentHub.Services;

/// <summary>
/// 個人設定管理
/// </summary>
public class ProfileService(GdbContext dbModel,
    ILogger<CommonService> _logger,
    IHttpContextAccessor _httpContextAccessor,
    IHostEnvironment _hostEnvironment,
    CaptchaHelper _captchaHelper,
    EmailHelper _emailHelper,
    UserManager<ApplicationUser> _userManager
    ) : BaseService(dbModel, _logger), IProfileService
{
    /// <summary>
    /// 取得目前登入者個人資料
    /// </summary>
    /// <param name="userId">使用者Id</param>
    /// <returns></returns>
    public async Task<Result<MeVm>> Me(string userId)
    {
        try
        {
            var user = await DbModel.ApplicationUsers
                .Include(x => x.Organization)
                .AsNoTracking()
                .Where(x => x.Id == userId && x.IsEnabled && !x.IsDelete)
                .Select(x => new MeVm
                {
                    UserId = x.Id,
                    UserName = x.UserName,
                    Email = x.Email,
                    FullName = x.FullName,
                    JobTitle = x.JobTitle,
                    OrgName = x.Organization != null ? x.Organization.NameShort : string.Empty,
                    Tel = x.Tel,
                    IsRegisterVerify = x.IsRegisterVerify
                })
                .FirstOrDefaultAsync();

            if (user == null)
                return ResultHelper.Failure<MeVm>("查無資料");
            if (!user.IsRegisterVerify)
                ResultHelper.Failure<UserVm>("User Not Verify Yet");

            return ResultHelper.Success(user);
        }
        catch (Exception ex)
        {
            LogError(ex, nameof(Me));
            return ResultHelper.Failure<MeVm>(ex.Message);
        }
    }

    /// <summary>
    /// 更新個人資料
    /// </summary>
    /// <param name="userId">使用者Id</param>
    /// <param name="vm">更新內容</param>
    /// <returns></returns>
    public async Task<Result<MeVm>> Update(string userId, UpdateVm vm)
    {
        try
        {
            var errors = vm.Validate().ToList();
            if (errors.Count != 0)
            {
                return ResultHelper.Failure<MeVm>(string.Join(",", errors));
            }

            var user = await DbModel.ApplicationUsers
                .Include(x => x.Organization)
                .FirstOrDefaultAsync(x => x.Id == userId);

            if (user == null)
            {
                return ResultHelper.Failure<MeVm>("User Not Found");
            }

            if (!user.IsRegisterVerify)
            {
                return ResultHelper.Failure<MeVm>("User Not Verify Yet");
            }

            user.UserName = vm.UserName;
            user.Email = vm.Email;
            user.FullName = vm.FullName;
            user.JobTitle = vm.JobTitle;
            user.Tel = vm.Tel;

            await DbModel.SaveChangesAsync();

            var me = new MeVm
            {
                UserId = user.Id,
                UserName = user.UserName,
                Email = user.Email,
                FullName = user.FullName,
                JobTitle = user.JobTitle,
                Tel = user.Tel,
                OrgName = user.Organization != null ? user.Organization.NameShort : string.Empty,
                IsRegisterVerify = user.IsRegisterVerify
            };

            return ResultHelper.Success(me);
        }
        catch (Exception ex)
        {
            LogError(ex, nameof(Update));
            return ResultHelper.Failure<MeVm>("更新個人資料發生例外");
        }
    }

    /// <summary>
    /// 變更密碼
    /// </summary>
    /// <param name="userId">使用者Id</param>
    /// <param name="vm">變更密碼內容</param>
    /// <returns></returns>
    public async Task<Result<string>> UpdateQoo(string userId, ChangeQooVm vm)
    {
        var (success, errors) = vm.Validate();
        if (!success)
        {
            return ResultHelper.Failure<string>(string.Join(",", errors));
        }

        try
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return ResultHelper.Failure<string>("查無使用者");

            if (!user.IsRegisterVerify)
            {
                return ResultHelper.Failure<string>("帳號尚未驗證");
            }

            if (!await _userManager.CheckPasswordAsync(user, vm.OldQoo))
            {
                return ResultHelper.Failure<string>("原始密碼錯誤");
            }

            // 密碼複雜度檢查
            foreach (var validator in _userManager.PasswordValidators)
            {
                var result = await validator.ValidateAsync(_userManager, user, vm.NewQoo);
                if (!result.Succeeded)
                {
                    return ResultHelper.Failure<string>(string.Join(",", result.Errors.Select(e => e.Description)));
                }
            }

            // 新密碼不能與前三次相同
            var qooHash = _userManager.PasswordHasher.HashPassword(user, vm.NewQoo);
            var theSameThreeTimes = await CheckQooTheSame(user, qooHash);
            if (theSameThreeTimes)
                return ResultHelper.Failure<string>("新密碼不能與前三次相同!");

            // 變更密碼後一小時內不能再變更
            var now = DateTime.UtcNow;
            if (user.LastChangeQoo > DateTime.MinValue && (now - user.LastChangeQoo.ToUniversalTime()).TotalMinutes < 60)
                return ResultHelper.Failure<string>("變更密碼後，一小時之內不能再次變更!");

            // 變更密碼
            var changeResult = await _userManager.ChangePasswordAsync(user, vm.OldQoo, vm.NewQoo);
            if (!changeResult.Succeeded)
                return ResultHelper.Failure<string>(string.Join(",", changeResult.Errors.Select(e => e.Description)));

            user.LastChangeQoo = now;
            user.MustChangeQoo = false;
            await DbModel.SaveChangesAsync();
            await AddUserQooHistoryAsync(user);

            return ResultHelper.Success("密碼變更成功");
        }
        catch (Exception ex)
        {
            LogError(ex, nameof(UpdateQoo));
            return ResultHelper.Failure<string>("變更密碼發生例外");
        }
    }

    /// <summary>
    /// 系統管理 - 帳號管理 - 取得所有啟用的組織清單
    /// </summary>
    /// <returns></returns>
    public async Task<Result<List<OrgIdName>>> Org()
    {
        try
        {
            var orgList = await DbModel.Organizations.AsNoTracking()
                .Where(x => x.IsEnabled)
                .Select(x => new OrgIdName
                {
                    OrgId = x.OrgId,
                    Name = x.Name,
                    Upper = x.Upper
                }).ToListAsync();

            return ResultHelper.Success(orgList);
        }
        catch (Exception ex)
        {
            LogError(ex, nameof(Org));
            return ResultHelper.Failure<List<OrgIdName>>(ex.Message);
        }
    }

    /// <summary>
    /// 取得使用者的第一個角色 RoleId
    /// </summary>
    /// <param name="userId">使用者ID（GUID）</param>
    /// <returns></returns>
    public async Task<Result<string>> GetUserRoleId(string userId)
    {
        if (string.IsNullOrEmpty(userId))
        {
            return ResultHelper.Failure<string>("未授權存取");
        }

        try
        {
            var userRole = await DbModel.UserRoles
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.UserId == userId);

            if (userRole == null)
            {
                return ResultHelper.Failure<string>("查無角色");
            }

            return ResultHelper.Success(userRole.RoleId);
        }
        catch (Exception ex)
        {
            LogError(ex, nameof(GetUserRoleId));
            return ResultHelper.Failure<string>("取得角色時發生例外");
        }
    }

    /// <summary>
    /// 新增使用者的密碼歷程
    /// </summary>
    /// <param name="user"></param>
    private async Task AddUserQooHistoryAsync(ApplicationUser user)
    {
        DbModel.UserQooHistories.Add(new UserQooHistory
        {
            LoginId = user.Id,
            UserId = user.Id,
            QooHash = user.PasswordHash,
        });
        await DbModel.SaveChangesAsync();
    }

    /// <summary>
    /// 檢查新密碼是否與使用者前三次的密碼相同
    /// </summary>
    /// <param name="user">使用者物件</param>
    /// <param name="qooHash">新密碼的雜湊值</param>
    /// <returns></returns>

    private async Task<bool> CheckQooTheSame(ApplicationUser user, string qooHash)
    {
        var his = await DbModel.UserQooHistories.AsNoTracking()
            .Where(x => x.UserId == user.Id)
            .OrderByDescending(x => x.CreatedDate)
            .Select(x => x.QooHash)
            .Take(3)
            .ToListAsync();
        return his.Exists(x =>
        {
            var res = _userManager.PasswordHasher.VerifyHashedPassword(user, x!, qooHash);
            return res == PasswordVerificationResult.Success;
        });
    }

    /// <summary>
    /// 忘記密碼
    /// </summary>
    /// <param name="vm"></param>
    /// <returns></returns>
    public async Task<Result<object>> ForgotQoo(ForgotQooVm vm)
    {
        try
        {
            if (!(await _captchaHelper.Validate(vm.CaptchaId, vm.Captcha)))
            {
                return ReturnErrorResult(
                    AccountFailStatus.驗證碼不符.ToString(),
                    (int)AccountFailStatus.驗證碼不符
                );
            }

            var (success, errors) = vm.Validate();
            if (!success)
            {
                return ReturnErrorResult(
                    errors[0].ToString(),
                    (int)errors[0]
                );
            }

            var user = await _userManager.FindByNameAsync(vm.UserName);
            if (user == null || !user.IsRegisterVerify)
            {
                return ReturnErrorResult(
                    AccountFailStatus.帳號不存在.ToString(),
                    (int)AccountFailStatus.帳號不存在
                );
            }

            await VerifyCodeFlow(user.Id, VerifyType.忘記密碼);
            return ResultHelper.Success<object>(string.Empty);
        }
        catch (Exception e)
        {
            Logger?.LogError(e, e.Message);
            return ReturnErrorResult(
                e.Message,
                (int)AccountFailStatus.忘記密碼失敗
            );
        }
    }

    /// <summary>
    /// 返回錯誤結果
    /// </summary>
    /// <param name="message">錯誤資訊</param>
    /// <param name="errorCode">錯誤碼</param>
    /// <returns></returns>
    private static Result<object> ReturnErrorResult(string message, int errorCode)
    {
        var result = ResultHelper.Failure<object>(message);
        result.ErrorCode = errorCode;
        return result;
    }

    /// <summary>
    /// 忘記密碼 - 修改密碼
    /// </summary>
    /// <param name="vm"></param>
    /// <returns></returns>
    public async Task<Result<object>> ForgotQooUpdate(ForgotUpdateQooVm vm)
    {
        try
        {
            var (success, errors) = vm.Validate();
            if (!success)
            {
                return ResultHelper.Failure<object>(string.Join(",", errors));
            }
            var checkResult = await CheckVerifyCode(Guid.Parse(vm.VeriyCodeId));
            if (checkResult != null && checkResult.Success)
            {
                var user = await _userManager.FindByIdAsync(checkResult.Data);
                if (user == null || !user.IsRegisterVerify)
                {
                    return ResultHelper.Failure<object>("The email you entered does belong to any account.");
                }

                user.PasswordHash = _userManager.PasswordHasher.HashPassword(user, vm.NewQoo);
                user.ForgotPwdVerifyCode = null;
                user.LastChangeQoo = DateTime.Now;
                user.MustChangeQoo = false;
                var updateResult = await _userManager.UpdateAsync(user);
                if (!updateResult.Succeeded)
                {
                    return ResultHelper.Failure<object>(string.Join(",", updateResult.Errors.Select(r => r.Description)));
                }

                await AddUserQooHistoryAsync(user);
                return ResultHelper.Success<object>(string.Empty);
            }
            else
            {
                return ResultHelper.Failure<object>(checkResult?.Message);
            }
        }
        catch (Exception e)
        {
            Logger?.LogError(e, e.Message);
            return ResultHelper.Failure<object>(e.Message);
        }
    }

    /// <summary>
    /// 強制更新密碼
    /// </summary>
    /// <param name="vm"></param>
    /// <returns></returns>
    public async Task<Result<string>> EnforceResetPassword(EnforceResetVm vm)
    {
        var res = new Result<string>();
        var isPass = true;
        try
        {
            // 改寫只檢查是否可以強制更新密碼 然後用原本的UpdatePassword去更新
            if (string.IsNullOrEmpty(vm.EnforceId))
            {
                res.Message = AccountFailStatus.密碼更新失敗.ToString();
                isPass = false;
            }
            if (string.IsNullOrEmpty(vm.NewQoo) && isPass)
            {
                res.Message = AccountFailStatus.新密碼未輸入.ToString();
                isPass = false;
            }
            if (!vm.NewQoo.Equals(vm.ConfirmQoo) && isPass)
            {
                res.Message = AccountFailStatus.新密碼與確認新密碼不相同.ToString();
            }
            // 將UserId替換成真正UserId
            var tryStatus = Guid.TryParse(vm.EnforceId, out _);
            if (!string.IsNullOrEmpty(vm.EnforceId) && tryStatus)
            {
                var checkResult = await CheckVerifyCode(Guid.Parse(vm.EnforceId));
                if (checkResult != null && checkResult.Success)
                {
                    vm.EnforceId = checkResult.Data;
                    res = await EnforceChangePassword(vm);
                }
                else
                {
                    res.Message = "密碼更新失敗";
                }
            }
            else
            {
                res.Message = "密碼更新失敗";
            }
        }
        catch (Exception e)
        {
            Logger?.LogError(e, "Error {MethodName}：{Message}", nameof(EnforceResetPassword), e.Message);
            res.Message = e.Message;
        }

        return res;
    }

    /// <summary>
    /// 改密碼
    /// </summary>
    /// <param name="vm"></param>
    /// <returns></returns>
    private async Task<Result<string>> EnforceChangePassword(EnforceResetVm vm)
    {
        var res = new Result<string>();

        var user = await _userManager.FindByIdAsync(vm.EnforceId);

        if (user == null || !user.IsEnabled)
        {
            res.Data = AccountFailStatus.帳號不存在.ToString();
            return res;
        }
        // 檢查新密碼不能與前三次相同
        var qooHash = _userManager.PasswordHasher.HashPassword(user, vm.NewQoo);
        var theSameThreeTimes = await CheckQooTheSame(user, qooHash);
        if (theSameThreeTimes)
        {
            res.Data = AccountFailStatus.前三次密碼相同.ToString();
            return res;
        }
        var hashPassword = _userManager.PasswordHasher.HashPassword(user, vm.NewQoo);
        try
        {
            user.PasswordHash = hashPassword;
            user.LastChangeQoo = DateTime.Now;
            user.MustChangeQoo = false;
            await using (var tran = await DbModel.Database.BeginTransactionAsync())
            {
                await _userManager.UpdateAsync(user);
                await AddUserQooHistoryAsync(user);

                await tran.CommitAsync();
            }
            res.Success = true;
        }
        catch (Exception ex)
        {
            Logger?.LogError(ex, "密碼更新失敗");
            res.Data = AccountFailStatus.密碼更新失敗.ToString();
        }

        return res;
    }

    /// <summary>
    /// CheckVerifyCode
    /// </summary>
    /// <param name="verifyCodeId"></param>
    /// <returns></returns>
    public async Task<Result<string>> CheckVerifyCode(Guid verifyCodeId)
    {
        var res = ResultHelper.Success<string>("");
        try
        {
            var verifyCode = await DbModel.VerifyCodes.FirstOrDefaultAsync(x => x.Id == verifyCodeId);
            if (verifyCode == null)
            {
                res = ResultHelper.Failure<string>(AccountFailStatus.無驗證碼紀錄.ToString());
                res.ErrorCode = (int)AccountFailStatus.無驗證碼紀錄;
                return res;
            }
            var now = DateTime.Now;
            if (verifyCode.StartTime <= now && now <= verifyCode.EndTime)
            {
                res.Data = verifyCode.UserId;
                res.Success = true;
            }
            else
            {
                res = ResultHelper.Failure<string>(AccountFailStatus.超過有效時間.ToString());
                res.ErrorCode = (int)AccountFailStatus.超過有效時間;
                return res;
            }
        }
        catch (Exception ex)
        {
            Logger?.LogError(ex, "驗證碼檢核失敗");
            res = ResultHelper.Failure<string>(AccountFailStatus.驗證碼檢核失敗.ToString());
            res.ErrorCode = (int)AccountFailStatus.驗證碼檢核失敗;
        }
        return res;
    }

    /// <summary>
    /// 寄發驗證信
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="type"></param>
    /// <returns></returns>
    public async Task<Result<string>> VerifyCodeFlow(string userId, VerifyType type)
    {
        var res = ResultHelper.Success<string>("");
        try
        {
            if (_httpContextAccessor.HttpContext == null)
            {
                Logger?.LogError("_httpContextAccessor.HttpContext is NULL：{MethodName}", nameof(VerifyCodeFlow));
                res = ResultHelper.Failure<string>(AccountFailStatus.寄發驗證信失敗.ToString());
                res.ErrorCode = (int)AccountFailStatus.寄發驗證信失敗;
                return res;
            }
            var request = _httpContextAccessor.HttpContext.Request;
            var path = "";
            var dictionary = new Dictionary<string, string>();
            var subject = "";
            var userInfo = await _userManager.FindByIdAsync(userId);
            var verifyCodeId = await AddVerifyCode(userId, type);
            switch (type)
            {
                case VerifyType.信箱註冊:
                    path = $"{_hostEnvironment.ContentRootPath}/MailTemplates/AppMemberConfirm.html";
                    dictionary.Add("urlBase", $"{request.Scheme}://{request.Host}/registerVerify?token={verifyCodeId}");
                    subject = "【系統名稱】信箱註冊";
                    break;

                case VerifyType.第一次登入:
                    path = $"{_hostEnvironment.ContentRootPath}/MailTemplates/FirstLogin.html";
                    dictionary.Add("urlBase", $"{request.Scheme}://{request.Host}/changeMyword?token={verifyCodeId}");
                    subject = "【系統名稱】第一次登入更改密碼";
                    break;

                case VerifyType.三個月強制變更密碼:
                    path = $"{_hostEnvironment.ContentRootPath}/MailTemplates/ThreeMonth.html";
                    dictionary.Add("urlBase", $"{request.Scheme}://{request.Host}/changeMyword?token={verifyCodeId}");
                    subject = "【系統名稱】超過三個月更改密碼";
                    break;

                case VerifyType.忘記密碼:
                    path = $"{_hostEnvironment.ContentRootPath}/MailTemplates/ForgotPwd.html";
                    dictionary.Add("urlBase", $"{request.Scheme}://{request.Host}/changeMyword?token={verifyCodeId}");
                    subject = "【系統名稱】忘記密碼通知";
                    break;

                default:
                    break;
            }

            await _emailHelper.SendTemplateEmail(userInfo!.Email!, path, dictionary, subject);

            res.Message = verifyCodeId.ToString();
        }
        catch (Exception e)
        {
            Logger?.LogError(e, "Error {MethodName}：{Message}", nameof(VerifyCodeFlow), e.Message);
            res = ResultHelper.Failure<string>(AccountFailStatus.寄發驗證信失敗.ToString());
            res.ErrorCode = (int)AccountFailStatus.寄發驗證信失敗;
        }
        return res;
    }

    private async Task<Guid> AddVerifyCode(string userId, VerifyType type, int afterHours = 1)
    {
        var now = DateTime.Now;
        var verifyCode = new VerifyCode()
        {
            UserId = userId,
            StartTime = now,
            EndTime = now.AddHours(afterHours),
            VerifyType = type
        };
        await DbModel.VerifyCodes.AddAsync(verifyCode);
        await DbModel.SaveChangesAsync();
        return verifyCode.Id;
    }
}