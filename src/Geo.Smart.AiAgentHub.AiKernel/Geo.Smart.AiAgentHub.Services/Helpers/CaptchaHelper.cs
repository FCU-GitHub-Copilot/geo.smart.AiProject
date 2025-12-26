using Geo.Smart.AiAgentHub.AiKernel.Models;
using Geo.Smart.AiAgentHub.Infras;
using Geo.Smart.AiAgentHub.Services.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using SixLabors.ImageSharp;
using SixLaborsCaptcha.Core;

namespace Geo.Smart.AiAgentHub.Services.Helpers;

/// <summary>
/// 負責產生與驗證驗證碼的輔助類別
/// </summary>
[DiLifetime(ServiceLifetime.Scoped)]
public class CaptchaHelper(GdbContext _dbModel,
    ILogger<CaptchaHelper> _logger,
    IConfiguration _configuration,
    IHostEnvironment _hostEnvironment)
{
    /// <summary>
    /// 驗證碼可用字元集
    /// </summary>
    private static readonly char[] _chars = "0123456789".ToCharArray();

    /// <summary>
    /// 取得驗證碼
    /// </summary>
    /// <returns>回傳包含驗證碼資訊的結果物件</returns>
    public async Task<Result<CaptchaVm>> Captcha()
    {
        try
        {
            var code = _hostEnvironment.IsDevelopment()
                ? ConstantData.CommonConst.Code9527
                : Extensions.GetUniqueKey(6, _chars);

            var captcha = new Captcha
            {
                Code = code
            };

            await _dbModel.AddAsync(captcha);
            await _dbModel.SaveChangesAsync();

            var base64 = Convert.ToBase64String(GetImageBytes(code));

            return ResultHelper.Success(new CaptchaVm
            {
                CaptchaId = captcha.Id,
                CaptchaBase64 = $"data:image/png;base64,{base64}"
            });
        }
        catch (Exception e)
        {
            _logger.LogError(e, $"Error {nameof(Captcha)}：發生錯誤");
            return ResultHelper.Failure<CaptchaVm>(e.Message);
        }
    }

    /// <summary>
    /// 產生驗證碼圖片並回傳 byte 陣列
    /// </summary>
    /// <param name="code">驗證碼字串</param>
    /// <returns>驗證碼圖片的 byte 陣列</returns>
    /// <exception cref="NotImplementedException">尚未實作例外</exception>
    private static byte[] GetImageBytes(string code)
    {
        var slc = new SixLaborsCaptchaModule(new SixLaborsCaptchaOptions
        {
            // 圖片長寬預設為 180 x 50
            FontFamilies = ["Arial"],
            DrawLines = 4,
            FontStyle = SixLabors.Fonts.FontStyle.BoldItalic,
            TextColor = [
                    Color.Blue, Color.Red, Color.Black,
                    Color.Brown, Color.Green
                ],
            NoiseRate = 1500,
            MaxLineThickness = 1,
        });

        return slc.Generate(code);
    }

    /// <summary>
    /// 驗證驗證碼是否正確
    /// </summary>
    /// <param name="captchaId">驗證碼唯一識別碼</param>
    /// <param name="code">使用者輸入的驗證碼</param>
    /// <returns>驗證結果，true 表示驗證通過</returns>
    public async Task<bool> Validate(Guid captchaId, string code)
    {
        if (_hostEnvironment.IsDevelopment() && code == ConstantData.CommonConst.Code9527)
        {
            return true;
        }

        try
        {
            var timingMin = _configuration.GetValue("Captcha:TimingMin", 5);
            var captcha = await _dbModel.Captchas
                .Where(x => x.IsEnabled)
                .Where(x => x.Id == captchaId)
                .FirstOrDefaultAsync();
            if (captcha == null)
            {
                return false;
            }

            // 使用過一次就失效
            captcha.IsEnabled = false;
            await _dbModel.SaveChangesAsync();

            // 忽略大小寫
            return (captcha.Code.Equals(code, StringComparison.CurrentCultureIgnoreCase)
                && captcha.CreatedDate.AddMinutes(timingMin) >= DateTime.Now);
        }
        catch (Exception e)
        {
            _logger.LogError(e, $"Error {nameof(Captcha)}：發生錯誤");
            return false;
        }
    }
}