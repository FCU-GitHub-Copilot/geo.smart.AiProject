using Geo.Smart.AiAgentHub.Services.Common;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System.Security.Cryptography;
using System.Text;

namespace Geo.Smart.AiAgentHub.Services.Helpers;
/// <summary>
/// 提供 AES 加解密與 SHA256 雜湊功能的輔助類別
/// </summary>
[DiLifetime(ServiceLifetime.Singleton)]
public class AesHelper
{
    private readonly string _k;
    private readonly string _v;

    /// <summary>
    /// 建構式
    /// </summary>
    /// <param name="configuration"></param>
    public AesHelper(IConfiguration configuration)
    {
        _k = configuration["SimpAes:Key"]!;
        _v = configuration["SimpAes:IV"]!;
    }

    /// <summary>
    /// 加密
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    public string Encrypt(string input)
    {
        return SimpAesHelper.Encrypt(input, _k, _v);
    }

    /// <summary>
    /// 解密
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    public string Decrypt(string input)
    {
        return SimpAesHelper.Decrypt(input, _k, _v);
    }

    /// <summary>
    /// 取得 SHA256 Hash 值
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    public static string Sha256Hash(string input)
    {
        var bytes = Encoding.UTF8.GetBytes(input);
        var hashBytes = SHA256.HashData(bytes);
        return Base64UrlEncoder.Encode(hashBytes);
    }
}