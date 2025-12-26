using Geo.Smart.AiAgentHub.Entities.Vms.Filex;
using Geo.Smart.AiAgentHub.WebApi.Common;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Geo.Smart.AiAgentHub.WebApi.Controllers;

/// <summary>
/// 檔案 API 控制器，提供檔案上傳與下載功能
/// </summary>
[ApiController]
[Route("[controller]/[action]")]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
[ApiExplorerSettings(IgnoreApi = true)]
public class FileController(IFileService _fileService) : SmartController
{
    /// <summary>
    /// 檔案上傳
    /// </summary>
    /// <param name="file">上傳的檔案</param>
    /// <param name="note">檔案說明</param>
    /// <returns>上傳結果，包含檔案識別碼或錯誤訊息</returns>
    [HttpPost]
    public ActionResult<Result<string>> Upload(IFormFile file, [FromForm] string? note)
    {
        return Ok(_fileService.Upload(file, note));
    }

    /// <summary>
    /// 更新檔案的說明
    /// </summary>
    /// <param name="vm">更新檔案說明 ViewModel</param>
    /// <returns></returns>
    [HttpPost]
    public ActionResult<Result<string>> UpdateNote(UpdateNoteVm vm)
    {
        return Ok(_fileService.UpdateNote(vm));
    }

    /// <summary>
    /// 依檔案識別碼下載檔案
    /// </summary>
    /// <param name="id">檔案識別碼</param>
    /// <returns>檔案內容或空結果</returns>
    [HttpGet("/[controller]/Get/{id}")]
    public IActionResult File(Guid id)
    {
        var result = _fileService.GetFileById(id);
        if (!result.Success)
        {
            return new EmptyResult();
        }

        return File(result.Data.FileContent, result.Data.ContentType, result.Data.FileName);
    }

    /// <summary>
    /// 刪除一般檔案(只能由上傳者修改、刪除。如果是 系統管理者 局內主管 則不限制。)
    /// </summary>
    /// <param name="vm">檔案資料</param>
    [HttpPost]
    public async Task<ActionResult<Result<string>>> Delete([FromBody] DeleteFileVm vm)
    {
        var result = await _fileService.Delete(vm, UserInfo);
        return Ok(result);
    }
}