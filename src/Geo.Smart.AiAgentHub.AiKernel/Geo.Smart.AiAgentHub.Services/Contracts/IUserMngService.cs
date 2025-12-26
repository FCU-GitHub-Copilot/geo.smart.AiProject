namespace Geo.Smart.AiAgentHub.Services.Contracts;

/// <summary>
/// 使用者管理
/// </summary>
public interface IUserMngService
{
    /// <summary>
    /// 取得使用者列表
    /// </summary>
    /// <param name="param"></param>
    /// <returns></returns>
    Task<PaginationResult<UserListVm>> Query(QueryBase param);

    /// <summary>
    /// 使用者詳細資料
    /// </summary>
    /// <param name="userId"></param>
    /// <returns></returns>
    Task<Result<UserDetailVm>> Detail(string userId);

    /// <summary>
    /// 新增使用者
    /// </summary>
    /// <param name="param"></param>
    /// <returns></returns>
    Task<Result<string>> Create(UserCreateVm param);

    /// <summary>
    /// 編輯使用者資料
    /// </summary>
    /// <param name="param"></param>
    /// <returns></returns>
    Task<Result<string>> Update(UserUpdateVm param);

    /// <summary>
    /// 系統管理 - 帳號管理 - 刪除帳號
    /// </summary>
    /// <param name="userId">要刪除的使用者 Id</param>
    /// <returns>刪除結果</returns>
    Task<Result<string>> Delete(string userId);

    /// <summary>
    /// 角色清單
    /// </summary>
    /// <returns></returns>
    Task<Result<List<CodeName>>> Role();
}