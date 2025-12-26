using Geo.Smart.AiAgentHub.Services.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Geo.Smart.AiAgentHub.Services.Helpers;

/// <summary>
/// 驗證碼 Helper
/// </summary>
[DiLifetime(ServiceLifetime.Scoped)]
public class VerifyHelper(GdbContext _dbContext)
{
    /// <summary>
    /// 產生驗證碼
    /// </summary>
    /// <param name="identityCode">識別碼</param>
    /// <param name="verifyType">驗證類型</param>
    /// <param name="afterMinutes">驗證碼過期時間</param>
    /// <returns></returns>
    public async Task<Guid> GenerateCode(string identityCode,
        VerifyType verifyType, int afterMinutes = 30)
    {
        var now = DateTime.Now;
        // 先刪除舊的驗證碼
        var codes = await _dbContext.VerifyCodes.Where(x =>
            x.UserId == identityCode && x.VerifyType == verifyType
        ).ToListAsync();
        foreach (var code in codes)
        {
            code.EndTime = now;
        }
        var verifyCode = new VerifyCode()
        {
            UserId = identityCode,
            VerifyType = verifyType,
            StartTime = now,
            EndTime = now.AddMinutes(afterMinutes),
        };

        await _dbContext.VerifyCodes.AddAsync(verifyCode);
        _dbContext.VerifyCodes.Add(verifyCode);
        await _dbContext.SaveChangesAsync();

        return verifyCode.Id;
    }

    /// <summary>
    /// 驗證驗證碼
    /// </summary>
    /// <param name="code">驗證碼</param>
    /// <param name="verifyType">驗證類型</param>
    /// <param name="identityCode">識別碼(可選，如果非NULL需額外判斷識別碼相同)</param>
    /// <param name="disabled">單次驗證(預設True)</param>
    /// <returns></returns>
    public async Task<bool> CheckVerifyCode(Guid code, VerifyType verifyType, string? identityCode = null, bool disabled = true)
    {
        var now = DateTime.Now;

        var verifyCode = await _dbContext.VerifyCodes
            .Where(r => r.Id == code && r.VerifyType == verifyType && r.StartTime <= now && r.EndTime >= now)
            .WhereIf(!string.IsNullOrEmpty(identityCode), r => r.UserId == identityCode)
            .FirstOrDefaultAsync();

        if (verifyCode == null)
        {
            return false;
        }

        if (disabled)
        {
            verifyCode.EndTime = now;
            await _dbContext.SaveChangesAsync();
        }

        return true;
    }
}