using Geo.Smart.AiAgentHub.Entities.Vms.Filex;
using Geo.Smart.AiAgentHub.Infras;
using Geo.Smart.AiAgentHub.Services.Common;
using Geo.Smart.AiAgentHub.Services.Contracts;
using Geo.Smart.AiAgentHub.Services.Helpers;
using Geo.Smart.FileManagerCore;
using Geo.Smart.FileManagerCore.Models.Common;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Geo.Smart.AiAgentHub.Services;

/// <summary>
/// 空間資料服務
/// </summary>
public class FileService(GdbContext dbModel,
    ILogger<CommonService> _logger,
    FilesManager _filesManager
    ) : BaseService(dbModel, _logger), IFileService
{
    /// <summary>
    /// 允許上傳的副檔名清單
    /// </summary>
    private static readonly string[] _allows = [
        ".doc", ".docx", ".xls", ".xlsx", ".ppt", ".pptx",
        ".odt", ".ods", ".odp", ".pdf",
        ".jpg", ".jpeg", ".png", ".gif",".zip", ".7z",
        ".tiff", ".tif"
    ];

    /// <summary>
    /// 檔案上傳方法
    /// </summary>
    /// <param name="file">實體檔案</param>
    /// <param name="note">檔案說明</param>
    /// <returns></returns>
    public Result<string> Upload(IFormFile file, string? note)
    {
        try
        {
            if (file == null || file.Length == 0)
            {
                return ResultHelper.Failure<string>("File Empty");
            }
            if (!_allows.Contains(Path.GetExtension(file.FileName.ToLower())))
            {
                return ResultHelper.Failure<string>("上傳的檔案不是允許的檔案格式");
            }
            // 檢查檔案表頭
            var headerChecker = FileHeaderHelper.Validate(file);
            if (!headerChecker.Success)
            {
                return headerChecker;
            }
            var result = _filesManager.Upload(file, note);
            if (result == null)
            {
                return ResultHelper.Failure<string>("檔案上傳失敗");
            }
            return ResultHelper.Success(result.FileId.ToString());
        }
        catch (Exception ex)
        {
            return LogAndReturn<string>(ex, nameof(Upload));
        }
    }

    /// <summary>
    /// 更新檔案的說明
    /// </summary>
    /// <param name="vm">更新檔案說明 ViewModel</param>
    /// <returns></returns>
    public ActionResult<Result<string>> UpdateNote(UpdateNoteVm vm)
    {
        try
        {
            var success = _filesManager.UpdateNote(vm.FileId, vm.Note);
            if (!success)
            {
                return ResultHelper.Failure<string>("更新檔案說明失敗！");
            }
            return ResultHelper.Success(vm.FileId.ToString());
        }
        catch (Exception ex)
        {
            return LogAndReturn<string>(ex, nameof(UpdateNote));
        }
    }

    /// <summary>
    /// 依照檔案識別碼取得檔案內容
    /// </summary>
    /// <param name="fileId">檔案 ID</param>
    /// <returns></returns>
    public Result<FileModel> GetFileById(Guid fileId)
    {
        try
        {
            var file = _filesManager.GetFileContent(fileId);
            return ResultHelper.Success(file);
        }
        catch (Exception ex)
        {
            return LogAndReturn<FileModel>(ex, nameof(GetFileById));
        }
    }

    /// <summary>
    /// 刪除一般檔案(只能由上傳者修改、刪除。如果是 系統管理者 業務單位主管 則不限制。)
    /// </summary>
    /// <param name="vm">檔案資料</param>
    /// <param name="userInfo">使用者資訊</param>
    public async Task<Result<string>> Delete(DeleteFileVm vm, UserInfo userInfo)
    {
        try
        {
            if (!await CheckCanEdit(vm.FileId, userInfo))
            {
                return ResultHelper.Failure<string>("您沒有權限刪除此資料");
            }
            var success = _filesManager.Delete(vm.FileId);
            return success
                ? ResultHelper.Success(string.Empty)
                : ResultHelper.Failure<string>("刪除失敗！");
        }
        catch (Exception e)
        {
            return LogAndReturn<string>(e, nameof(Delete));
        }
    }

    /// <summary>
    /// 檢查是否可刪除 和 編輯。(只能由上傳者修改、刪除。如果是 系統管理者 局內主管 則不限制。)
    /// </summary>
    /// <param name="Field">檔案唯一Id</param>
    /// <param name="userInfo">使用者資訊</param>
    /// <returns></returns>
    private async Task<bool> CheckCanEdit(Guid Field, UserInfo userInfo)
    {
        var createdBy = await DbModel.Filexes.AsNoTracking()
            .Where(x => x.FileId == Field && x.IsEnabled)
            .Select(x => x.CreatedBy)
            .SingleOrDefaultAsync();
        if (string.IsNullOrEmpty(createdBy))
        {
            return false;
        }
        return ConstantData.FileAdminRoles.Contains(userInfo.RoleId)
            || userInfo.UserId.Equals(createdBy, StringComparison.OrdinalIgnoreCase);
    }
}