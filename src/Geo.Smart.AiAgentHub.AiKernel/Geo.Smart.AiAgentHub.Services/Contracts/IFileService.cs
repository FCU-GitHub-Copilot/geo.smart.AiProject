using Geo.Smart.AiAgentHub.Entities.Vms.Filex;
using Geo.Smart.FileManagerCore.Models.Common;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Geo.Smart.AiAgentHub.Services.Contracts;

/// <summary>
/// 檔案服務介面
/// </summary>
public interface IFileService
{
    /// <summary>
    /// 檔案上傳方法
    /// </summary>
    /// <param name="file">實體檔案</param>
    /// <param name="note">檔案說明</param>
    /// <returns></returns>
    Result<string> Upload(IFormFile file, string? note);

    /// <summary>
    /// 更新檔案的說明
    /// </summary>
    /// <param name="vm">更新檔案說明 ViewModel</param>
    /// <returns></returns>
    ActionResult<Result<string>> UpdateNote(UpdateNoteVm vm);

    /// <summary>
    /// 依照檔案識別碼取得檔案內容
    /// </summary>
    /// <param name="fileId">檔案 ID</param>
    /// <returns></returns>
    Result<FileModel> GetFileById(Guid fileId);

    /// <summary>
    /// 刪除一般檔案(只能由上傳者修改、刪除。如果是 系統管理者 局內主管 則不限制。)
    /// </summary>
    /// <param name="vm">檔案資料</param>
    /// <param name="userInfo">使用者資訊</param>
    Task<Result<string>> Delete(DeleteFileVm vm, UserInfo userInfo);
}