using Geo.Smart.AiAgentHub.Entities.Vms.Photo;
using Geo.Smart.AiAgentHub.Infras;
using Geo.Smart.AiAgentHub.Services.Common;
using Geo.Smart.AiAgentHub.Services.Contracts;
using Geo.Smart.AiAgentHub.Services.Helpers;
using Geo.Smart.FileManagerCore;
using Geo.Smart.FileManagerCore.Models.Common;
using Geo.Smart.FileManagerCore.Models.Enum;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Geo.Smart.AiAgentHub.Services;

/// <summary>
/// 空間資料服務
/// </summary>
public class PhotoService(GdbContext dbModel,
    ILogger<CommonService> _logger,
    PhotoManager _photoManager)
    : BaseService(dbModel, _logger), IPhotoService
{
    private static readonly string[] _allows = [
        ".jpg", ".jpeg", ".png", ".gif"
    ];

    /// <summary>
    /// 圖片上傳
    /// </summary>
    /// <param name="file">上傳的檔案</param>
    /// <param name="note">檔案說明</param>
    /// <returns></returns>
    public Result<string> Upload(IFormFile file, [FromForm] string? note)
    {
        try
        {
            var validResult = ValidPhoto(file);
            if (!validResult.Success)
            {
                return validResult;
            }
            var result = _photoManager.Upload(file, note);
            if (result == null)
            {
                return ResultHelper.Failure<string>("圖片上傳失敗");
            }
            return ResultHelper.Success(result.PhotoId.ToString());
        }
        catch (Exception e)
        {
            return LogAndReturn<string>(e, nameof(Upload));
        }
    }

    /// <summary>
    /// 更新圖片的說明
    /// </summary>
    /// <param name="photoId">圖片 ID</param>
    /// <param name="note">圖片說明</param>
    /// <returns></returns>
    public Result<string> UpdateNote(Guid photoId, string note)
    {
        try
        {
            var success = _photoManager.UpdateNote(photoId, note);
            if (!success)
            {
                return ResultHelper.Failure<string>("更新檔案說明失敗！");
            }
            return ResultHelper.Success(photoId.ToString());
        }
        catch (Exception e)
        {
            return LogAndReturn<string>(e, nameof(UpdateNote));
        }
    }

    /// <summary>
    /// 圖片：ori、lg、md、sm、xs
    /// </summary>
    /// <param name="id"></param>
    /// <param name="size"></param>
    /// <returns></returns>
    public Result<FileModel> GetPhoto(Guid id, string size)
    {
        try
        {
            if (!Enum.TryParse<PhotoSize>(size.ToLower(), out var photoSize))
            {
                photoSize = PhotoSize.ori;
            }
            var fileModel = _photoManager.GetContent(id, photoSize);
            return ResultHelper.Success(fileModel);
        }
        catch (Exception e)
        {
            return LogAndReturn<FileModel>(e, nameof(GetPhoto));
        }
    }

    /// <summary>
    /// 刪除照片
    /// </summary>
    /// <param name="vm">照片資料</param>
    /// <param name="userInfo">使用者資訊</param>
    /// <returns></returns>
    public async Task<Result<string>> Delete(DeletePhotoVm vm, UserInfo userInfo)
    {
        try
        {
            if (!await CheckCanEdit(vm.PhotoId, userInfo))
            {
                return ResultHelper.Failure<string>("您沒有權限刪除此資料");
            }
            var success = _photoManager.Delete(vm.PhotoId);
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
    /// 驗證傳入的圖片是否合規
    /// </summary>
    /// <param name="file">上傳的圖片</param>
    /// <returns></returns>
    private static Result<string> ValidPhoto(IFormFile file)
    {
        if (file == null || file.Length == 0)
        {
            return ResultHelper.Failure<string>("File Empty");
        }
        if (!_allows.Contains(Path.GetExtension(file.FileName.ToLower())))
        {
            return ResultHelper.Failure<string>("上傳的檔案不是允許的圖片格式");
        }
        // 檢查檔案表頭
        return FileHeaderHelper.Validate(file);
    }

    /// <summary>
    /// 依照片ID取得照片
    /// </summary>
    /// <param name="photoId">照片ID</param>
    /// <returns>照片物件</returns>
    private async Task<Photo?> GetPhoto(Guid photoId)
    {
        return await DbModel.Photos.SingleOrDefaultAsync(x => x.PhotoId == photoId);
    }

    /// <summary>
    /// 判斷是否能修改刪除。 (只能由上傳者修改、刪除。如果是 系統管理者、局內主管 則不限制。)
    /// </summary>
    /// <param name="photoId">照片 ID</param>
    /// <param name="userInfo">使用者資訊</param>
    /// <returns></returns>
    private async Task<bool> CheckCanEdit(Guid photoId, UserInfo userInfo)
    {
        var createdBy = await DbModel.Photos.AsNoTracking()
            .Where(x => x.PhotoId == photoId && x.IsEnabled)
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