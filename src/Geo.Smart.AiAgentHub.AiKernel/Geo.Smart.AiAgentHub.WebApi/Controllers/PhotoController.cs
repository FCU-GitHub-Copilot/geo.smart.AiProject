using Geo.Smart.AiAgentHub.Entities.Vms.Photo;
using Geo.Smart.AiAgentHub.WebApi.Common;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Geo.Smart.AiAgentHub.WebApi.Controllers;

/// <summary>
/// 圖片 API Controller
/// </summary>
[ApiController]
[Route("[controller]/[action]")]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
[ApiExplorerSettings(IgnoreApi = true)]
public class PhotoController(IPhotoService _photoService)
    : SmartController
{
    /// <summary>
    /// 圖片上傳
    /// </summary>
    /// <param name="file"></param>
    /// <param name="note">檔案說明</param>
    /// <returns></returns>
    [HttpPost]
    public ActionResult<Result<string>> Upload(IFormFile file, [FromForm] string? note)
    {
        return Ok(_photoService.Upload(file, note));
    }

    /// <summary>
    /// 更新圖片的說明
    /// </summary>
    /// <param name="photoId">圖片 ID</param>
    /// <param name="note">圖片說明</param>
    /// <returns>上傳結果，包含圖片識別碼或錯誤訊息</returns>
    [HttpPost]
    public ActionResult<Result<string>> UpdateNote(Guid photoId, string note)
    {
        return Ok(_photoService.UpdateNote(photoId, note));
    }

    /// <summary>
    /// 圖片：ori、lg、md、sm、xs
    /// </summary>
    /// <param name="id"></param>
    /// <param name="size"></param>
    /// <returns></returns>
    [HttpGet("/[controller]/Get/{size}/{id}")]
    public IActionResult Photo(Guid id, string size = "ori")
    {
        var result = _photoService.GetPhoto(id, size);
        if (!result.Success || result.Data == null)
        {
            return new EmptyResult();
        }

        return File(result.Data.FileContent, result.Data.ContentType);
    }

    /// <summary>
    /// 刪除照片
    /// </summary>
    /// <param name="vm">照片資料</param>
    [HttpPost]
    public async Task<ActionResult<Result<string>>> Delete([FromBody] DeletePhotoVm vm)
    {
        return Ok(await _photoService.Delete(vm, UserInfo));
    }
}