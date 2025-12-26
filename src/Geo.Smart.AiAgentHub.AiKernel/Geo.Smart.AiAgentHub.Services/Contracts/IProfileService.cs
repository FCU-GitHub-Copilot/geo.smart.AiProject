using Geo.Smart.AiAgentHub.Entities.Vms.Profile;

namespace Geo.Smart.AiAgentHub.Services.Contracts;

/// <summary>
/// 個人資訊相關服務
/// </summary>
public interface IProfileService
{
    /// <summary>
    /// 取得目前登入者個人資料
    /// </summary>
    /// <param name="userId">使用者Id</param>
    /// <returns>個人資料</returns>
    Task<Result<MeVm>> Me(string userId);

    /// <summary>
    /// 更新個人資料
    /// </summary>
    /// <param name="userId">使用者Id</param>
    /// <param name="vm">更新內容</param>
    /// <returns>更新後的個人資料</returns>
    Task<Result<MeVm>> Update(string userId, UpdateVm vm);

    /// <summary>
    /// 變更密碼
    /// </summary>
    /// <param name="userId">使用者Id</param>
    /// <param name="vm">變更密碼內容</param>
    /// <returns>變更結果訊息</returns>
    Task<Result<string>> UpdateQoo(string userId, ChangeQooVm vm);

    /// <summary>
    /// 取得所有啟用的組織清單
    /// </summary>
    /// <returns>組織清單</returns>
    Task<Result<List<OrgIdName>>> Org();

    /// <summary>
    /// 取得使用者的角色 RoleId
    /// </summary>
    /// <param name="userId">使用者ID（GUID）</param>
    /// <returns>角色 RoleId，若無則回傳空字串</returns>
    Task<Result<string>> GetUserRoleId(string userId);

    /// <summary>
    /// 忘記密碼
    /// </summary>
    /// <param name="vm"></param>
    /// <returns></returns>
    Task<Result<object>> ForgotQoo(ForgotQooVm vm);

    /// <summary>
    /// 忘記密碼 - 修改密碼
    /// </summary>
    /// <param name="vm"></param>
    /// <returns></returns>
    Task<Result<object>> ForgotQooUpdate(ForgotUpdateQooVm vm);

    /// <summary>
    /// 強制更新密碼
    /// </summary>
    /// <param name="vm"></param>
    /// <returns></returns>
    Task<Result<string>> EnforceResetPassword(EnforceResetVm vm);

    /// <summary>
    /// 寄信通知
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="type">VerifyType</param>
    /// <returns></returns>
    Task<Result<string>> VerifyCodeFlow(string userId, VerifyType type);
}