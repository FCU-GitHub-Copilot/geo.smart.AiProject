using Geo.Smart.AiAgentHub.Services.Common;
using Geo.Smart.AiAgentHub.Services.Contracts;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Geo.Smart.AiAgentHub.Services;

/// <summary>
/// 使用者管理
/// </summary>
public class UserMngService(GdbContext dbModel,
    ILogger<CommonService> _logger,
    UserManager<ApplicationUser> _userManager
    ) : BaseService(dbModel, _logger), IUserMngService
{
    /// <summary>
    /// 取得使用者列表
    /// </summary>
    /// <param name="param">查詢條件</param>
    /// <returns>分頁的使用者列表</returns>
    public async Task<PaginationResult<UserListVm>> Query(QueryBase param)
    {
        try
        {
            var query = DbModel.ApplicationUsers.AsNoTracking()
                .Include(x => x.ApplicationRoles)
                .Include(x => x.Organization)
                .Where(x => !x.IsDelete)
                .Select(x => new UserListVm
                {
                    UserId = x.Id,
                    UserName = x.UserName,
                    FullName = x.FullName,
                    Email = x.Email,
                    RoleName = x.ApplicationRoles.Any() ? x.ApplicationRoles.First().Name : "",
                    OrgId = x.OrgId,
                    OrgName = x.Organization.NameShort,
                    LastLogin = x.LastLogin,
                    IsEnabled = x.IsEnabled,
                })
                .WhereIf(!string.IsNullOrEmpty(param.Keyword), x => x.FullName.Contains(param.Keyword)
                    || (x.UserName != null && x.UserName.Contains(param.Keyword)));

            return await ResultHelper.PaginationSuccessAsync(query, param);
        }
        catch (Exception ex)
        {
            LogError(ex, nameof(Query));
            return ResultHelper.PaginationFailure<UserListVm>(ex.Message);
        }
    }

    /// <summary>
    /// 使用者詳細資料
    /// </summary>
    /// <param name="userId">使用者ID</param>
    /// <returns></returns>
    public async Task<Result<UserDetailVm>> Detail(string userId)
    {
        try
        {
            var user = await DbModel.ApplicationUsers.AsNoTracking()
                .Include(x => x.Organization)
                .Include(x => x.ApplicationRoles)
                .Where(x => x.Id == userId)
                .Select(x => new UserDetailVm
                {
                    UserId = x.Id,
                    UserName = x.UserName,
                    Email = x.Email,
                    FullName = x.FullName,
                    Tel = x.Tel,
                    TelExt = x.TelExt,
                    JobTitle = x.JobTitle,
                    RoleId = x.ApplicationRoles.Select(r => r.Id).FirstOrDefault(),
                    RoleName = x.ApplicationRoles.Select(r => r.Name).FirstOrDefault(),
                    OrgId = x.OrgId,
                    OrgName = x.Organization.NameShort,
                    Gender = x.Gender,
                    IsEnabled = x.IsEnabled,
                    LastChangeQoo = x.LastChangeQoo
                }).FirstOrDefaultAsync();

            if (user == null)
            {
                return ResultHelper.Failure<UserDetailVm>("查無資料或無權限");
            }

            // 密碼是否需要變更提醒
            var lastChange = user.LastChangeQoo;
            var threeMonthsAgo = DateTime.Now.AddMonths(-3).Date;
            user.IsNeedQooChange = lastChange.Date <= threeMonthsAgo;
            user.RemainQooChange = (int)(lastChange.Date.AddMonths(3) - DateTime.Now.Date).TotalDays;

            return ResultHelper.Success(user);
        }
        catch (Exception ex)
        {
            LogError(ex, nameof(Detail));
            return ResultHelper.Failure<UserDetailVm>(ex.Message);
        }
    }

    /// <summary>
    /// 新增帳號
    /// </summary>
    /// <param name="param">建立使用者VM</param>
    /// <returns></returns>
    public async Task<Result<string>> Create(UserCreateVm param)
    {
        try
        {
            // 帳號唯一性驗證
            if (await DbModel.ApplicationUsers.AnyAsync(x => x.IsEnabled && x.UserName == param.UserName))
            {
                return ResultHelper.Failure<string>("使用者帳號已經被使用，請填寫其他帳號註冊");
            }
            // Email唯一性驗證
            if (await DbModel.ApplicationUsers.AnyAsync(x => !x.IsDelete && x.Email == param.Email))
            {
                return ResultHelper.Failure<string>("E-mail與其他帳號重複，請確認登打是否有誤");
            }

            if (string.IsNullOrWhiteSpace(param.UserName)
              || string.IsNullOrWhiteSpace(param.Email)
              || string.IsNullOrWhiteSpace(param.FullName)
              || string.IsNullOrWhiteSpace(param.Tel)
              || string.IsNullOrWhiteSpace(param.JobTitle)
              || string.IsNullOrWhiteSpace(param.RoleId)
              || string.IsNullOrWhiteSpace(param.Qoo))
            {
                return ResultHelper.Failure<string>("帳號、信箱、姓名、電話、職稱、角色、密碼皆為必填");
            }
            if (param.Qoo != param.ConfirmQoo)
            {
                return ResultHelper.Failure<string>("密碼與確認密碼不一致");
            }
            // 取得角色名稱
            var role = await DbModel.ApplicationRoles.FirstOrDefaultAsync(x => x.Id == param.RoleId);
            if ((role == null) || (string.IsNullOrWhiteSpace(role?.Name)))
            {
                return ResultHelper.Failure<string>("找不到對應的角色");
            }
            if (string.IsNullOrWhiteSpace(role.Name))
            {
                return ResultHelper.Failure<string>("角色名稱為空，無法加入角色");
            }
            var newUser = new ApplicationUser
            {
                Id = Guid.NewGuid().ToString(),
                UserName = param.UserName,
                NormalizedUserName = param.UserName.ToUpper(),
                Email = param.Email,
                NormalizedEmail = param.Email.ToUpper(),
                FullName = param.FullName,
                Gender = param.Gender,
                OrgId = param.OrgId,
                JobTitle = param.JobTitle,
                Tel = param.Tel,
                TelExt = param.TelExt,
                IsEnabled = param.IsEnabled,
                LastChangeQoo = param.LastChangeQoo,
                IsRegisterVerify = true,
                EmailConfirmed = true
            };

            var createResult = await _userManager.CreateAsync(newUser, param.Qoo);
            if (!createResult.Succeeded)
            {
                return ResultHelper.Failure<string>(string.Join(",",
                    createResult.Errors.Select(r => r.Description)));
            }

            // 將使用者加入角色（寫入 AspNetUserRoles）
            var addRoleResult = await _userManager.AddToRoleAsync(newUser, role.Name);
            if (!addRoleResult.Succeeded)
            {
                return ResultHelper.Failure<string>(string.Join(",", addRoleResult.Errors.Select(r => r.Description)));
            }
            return ResultHelper.Success(newUser.Id);
        }
        catch (Exception ex)
        {
            LogError(ex, nameof(Create));
            return ResultHelper.Failure<string>(ex.Message);
        }
    }

    /// <summary>
    /// 編輯帳號
    /// </summary>
    /// <param name="param"></param>
    /// <returns></returns>
    public async Task<Result<string>> Update(UserUpdateVm param)
    {
        try
        {
            Dictionary<string, string> requiredFields = CheckRequiredFields(param);

            //必填檢查
            var emptyField = requiredFields.FirstOrDefault(f => string.IsNullOrWhiteSpace(f.Value));
            if (!string.IsNullOrEmpty(emptyField.Key))
            {
                return ResultHelper.Failure<string>($"{emptyField.Key}為必填");
            }

            // 帳號/Email唯一性檢查
            var uniqueCheck = await CheckUserUniqueAsync(param);
            if (uniqueCheck != null)
            {
                return uniqueCheck;
            }

            var user = await DbModel.ApplicationUsers
                .Include(x => x.ApplicationRoles)
                .Where(x => x.Id == param.UserId)
                .FirstOrDefaultAsync();

            if (user == null)
            {
                return ResultHelper.Failure<string>("查無資料或無權限");
            }

            var role = await DbModel.ApplicationRoles.FirstOrDefaultAsync(x => x.Id == param.RoleId);
            if (role == null)
            {
                return ResultHelper.Failure<string>("找不到對應的角色");
            }

            UpdateUserFields(param, user);

            var currentRoles = await _userManager.GetRolesAsync(user);
            if (currentRoles.Any())
            {
                var removeResult = await _userManager.RemoveFromRolesAsync(user, currentRoles);
                if (!removeResult.Succeeded)
                {
                    return ResultHelper.Failure<string>(string.Join(",", removeResult.Errors.Select(r => r.Description)));
                }
            }

            var addRoleResult = await _userManager.AddToRoleAsync(user, role.Name!);
            if (!addRoleResult.Succeeded)
            {
                return ResultHelper.Failure<string>(string.Join(",", addRoleResult.Errors.Select(r => r.Description)));
            }

            var updateResult = await _userManager.UpdateAsync(user);
            if (!updateResult.Succeeded)
            {
                return ResultHelper.Failure<string>(string.Join(",",
                    updateResult.Errors.Select(r => r.Description)));
            }

            return ResultHelper.Success(user.Id);
        }
        catch (Exception ex)
        {
            LogError(ex, nameof(Update));
            return ResultHelper.Failure<string>(ex.Message);
        }
    }

    /// <summary>
    /// 刪除帳號
    /// </summary>
    /// <param name="userId">要刪除的使用者 Id</param>
    /// <returns>刪除結果</returns>
    public async Task<Result<string>> Delete(string userId)
    {
        try
        {
            // 僅允許刪除本組織或下層組織的使用者
            var user = await DbModel.ApplicationUsers
                .FirstOrDefaultAsync(x => x.Id == userId);
            if (user == null)
            {
                return ResultHelper.Failure<string>("查無此使用者或無權限");
            }

            user.UserName = $"del_{user.UserName}_{Guid.NewGuid()}";
            user.IsEnabled = false;
            user.IsDelete = true;

            await _userManager.UpdateAsync(user);
            await DbModel.SaveChangesAsync();

            return ResultHelper.Success(user.Id);
        }
        catch (Exception ex)
        {
            LogError(ex, nameof(Delete));
            return ResultHelper.Failure<string>(ex.Message);
        }
    }

    /// <summary>
    /// 角色清單
    /// </summary>
    /// <returns></returns>
    public async Task<Result<List<CodeName>>> Role()
    {
        try
        {
            var result = await DbModel.ApplicationRoles.AsNoTracking()
                .Select(x => new CodeName
                {
                    Code = x.Id,
                    Name = x.Name
                })
                .OrderBy(x => x.Name).ToListAsync();
            return ResultHelper.Success(result);
        }
        catch (Exception ex)
        {
            LogError(ex, nameof(Role));
            return ResultHelper.Failure<List<CodeName>>(ex.Message);
        }
    }

    private static void UpdateUserFields(UserUpdateVm param, ApplicationUser user)
    {
        user.UserName = param.UserName;
        user.Email = param.Email;
        user.NormalizedEmail = param.Email.ToUpper();
        user.FullName = param.FullName;
        user.Gender = param.Gender;
        user.OrgId = param.OrgId;
        user.JobTitle = param.JobTitle;
        user.Tel = param.Tel;
        user.TelExt = param.TelExt;
        user.IsEnabled = param.IsEnabled;
    }

    private static Dictionary<string, string> CheckRequiredFields(UserUpdateVm param)
    {
        // 必填欄位檢查
        return new Dictionary<string, string>
        {
            { nameof(param.UserName), param.UserName },
            { nameof(param.FullName), param.FullName },
            { nameof(param.Tel), param.Tel },
            { nameof(param.Email), param.Email },
            { nameof(param.JobTitle), param.JobTitle },
            { nameof(param.RoleId), param.RoleId }
        };
    }

    private async Task<Result<string>?> CheckUserUniqueAsync(UserUpdateVm param)
    {
        var existUser = await DbModel.ApplicationUsers
            .Where(x => !x.IsDelete && x.Id != param.UserId)
            .Where(x => x.UserName == param.UserName || x.Email == param.Email)
            .FirstOrDefaultAsync();

        if (existUser != null)
        {
            if (existUser.UserName == param.UserName)
            {
                return ResultHelper.Failure<string>("使用者帳號已經被使用，請填寫其他帳號註冊");
            }
            if (existUser.Email == param.Email)
            {
                return ResultHelper.Failure<string>("E-mail與其他帳號重複，請確認登打是否有誤");
            }
        }
        return null;
    }

    #region Private Function

    /// <summary>
    /// 遞迴取得樹狀組織清單
    /// </summary>
    /// <param name="allOrgList"></param>
    /// <param name="currentOrgId"></param>
    /// <returns></returns>
    private static List<OrgTreeVm> GetOrgTree(List<OrgIdName> allOrgList, Guid? currentOrgId)
    {
        return allOrgList.Where(x => x.Upper == currentOrgId)
            .OrderBy(x => x.Name)
            .Select(x => new OrgTreeVm
            {
                OrgId = x.OrgId,
                Name = x.Name,
                SubOrg = GetOrgTree(allOrgList, x.OrgId)
            }).ToList();
    }

    #endregion Private Function
}