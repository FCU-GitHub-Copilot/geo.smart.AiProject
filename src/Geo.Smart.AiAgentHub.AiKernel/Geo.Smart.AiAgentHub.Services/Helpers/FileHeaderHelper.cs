using Microsoft.AspNetCore.Http;

namespace Geo.Smart.AiAgentHub.Services.Helpers;
/// <summary>
/// 檢查上傳檔案的表頭
/// </summary>
public static class FileHeaderHelper
{
    private static readonly byte[] _msOffice2003 = [0xD0, 0xCF, 0x11, 0xE0, 0xA1, 0xB1, 0x1A, 0xE1];
    private static readonly byte[] _zips = [0x50, 0x4B, 0x03, 0x04];
    private static readonly byte[] _jpg = [0xFF, 0xD8, 0xFF];

    /// <summary>
    /// 所有的檔案表頭字典檔
    /// https://en.wikipedia.org/wiki/List_of_file_signatures
    /// </summary>
    private static readonly Dictionary<string, byte[]> _fileHeaders = new()
    {
            { ".doc", _msOffice2003 },
            { ".xls", _msOffice2003 },
            { ".ppt", _msOffice2003 },
            { ".docx", _zips },
            { ".xlsx", _zips },
            { ".pptx", _zips },
            { ".odt", _zips },
            { ".ods", _zips },
            { ".odp", _zips },
            { ".zip", _zips },
            { ".7z", _zips },
            { ".pdf", "%PDF-"u8.ToArray() },
            { ".jpg", _jpg },
            { ".jpeg", _jpg },
            { ".png", new byte[] { 0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A }},
            { ".gif", "GIF8"u8.ToArray()},
            { ".tiff", new byte[] { 0x49, 0x49, 0x2A, 0x00 } },
            { ".tiff_be", new byte[] { 0x4D, 0x4D, 0x00, 0x2A } },
            { ".tif", new byte[] { 0x49, 0x49, 0x2A, 0x00 } },
            { ".tif_be", new byte[] { 0x4D, 0x4D, 0x00, 0x2A } }
        };

    /// <summary>
    /// 驗證上傳的檔案表頭與副檔名是否相同
    /// </summary>
    /// <param name="formFile"></param>
    /// <returns></returns>
    public static Result<string> Validate(IFormFile formFile)
    {
        var fileExt = Path.GetExtension(formFile.FileName).ToLower();
        if (!_fileHeaders.TryGetValue(fileExt, out byte[]? checker))
        {
            return ResultHelper.Failure<string>("上傳檔案的附檔名未在允許清單中！");
        }

        var header = new byte[checker.Length];
        using var stream = formFile.OpenReadStream();
        stream.ReadExactly(header, 0, checker.Length);
        stream.Position = 0;
        stream.Close();

        if (Compare(checker, header))
        {
            return ResultHelper.Success(string.Empty);
        }
        else
        {
            return ResultHelper.Failure<string>("上傳檔案的表頭未合規！");
        }
    }

    /// <summary>
    /// 比較兩個陣列是否相同
    /// </summary>
    /// <param name="a1"></param>
    /// <param name="a2"></param>
    /// <returns></returns>
    private static bool Compare(byte[] a1, byte[] a2)
    {
        if (a1.Length != a2.Length)
        {
            return false;
        }

        for (int i = 0; i < a1.Length; i++)
        {
            if (a1[i] != a2[i])
                return false;
        }

        return true;
    }
}