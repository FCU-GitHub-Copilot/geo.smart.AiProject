using Geo.Smart.AiAgentHub.Entities.Vms.Photo;
using Geo.Smart.FileManagerCore.Models.Common;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Geo.Smart.AiAgentHub.Services.Contracts;

/// <summary>
/// 照片上傳服務介面
/// </summary>
public interface IPhotoService
{
    /// <summary>
    /// 圖片上傳
    /// </summary>
    /// <param name="file">上傳的檔案</param>
    /// <param name="note">檔案說明</param>
    /// <returns></returns>
    Result<string> Upload(IFormFile file, [FromForm] string? note);

    /// <summary>
    /// 更新圖片的說明
    /// </summary>
    /// <param name="photoId">圖片 ID</param>
    /// <param name="note">圖片說明</param>
    /// <returns></returns>
    Result<string> UpdateNote(Guid photoId, string note);

    /// <summary>
    /// 圖片：ori、lg、md、sm、xs
    /// </summary>
    /// <param name="id"></param>
    /// <param name="size"></param>
    /// <returns></returns>
    Result<FileModel> GetPhoto(Guid id, string size);

    /// <summary>
    /// 刪除照片
    /// </summary>
    /// <param name="vm">照片資料</param>
    /// <param name="userInfo">使用者資訊</param>
    /// <returns></returns>
    Task<Result<string>> Delete(DeletePhotoVm vm, UserInfo userInfo);
}