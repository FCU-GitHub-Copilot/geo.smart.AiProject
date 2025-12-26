using System.Security.Cryptography;
using System.Text;

namespace Geo.Smart.AiAgentHub.Services.Helpers;
/// <summary>
/// 隨機亂數產生Helper
/// </summary>
public static class RandomHelper
{
    /// <summary>
    /// 取得固定長度的數字字串
    /// </summary>
    /// <param name="length"></param>
    /// <returns></returns>
    public static string GetString(int length)
    {
        var bytes = new byte[8];
        using (var crypto = RandomNumberGenerator.Create())
        {
            crypto.GetBytes(bytes);
        }
        return (BitConverter.ToUInt64(bytes) % Math.Pow(10, length)).ToString().PadLeft(length, '0');
    }

    /// <summary>
    /// 取得隨機數字
    /// </summary>
    /// <param name="max"></param>
    /// <returns></returns>
    public static int GetInt(int? max = null)
    {
        var bytes = new byte[4];
        using (var crypto = RandomNumberGenerator.Create())
        {
            crypto.GetBytes(bytes);
        }
        var rand = Math.Abs(BitConverter.ToInt32(bytes));
        if (!max.HasValue)
        {
            return rand;
        }

        return rand % (max.Value);
    }

    internal static readonly char[] chars =
        "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890".ToCharArray();

    /// <summary>
    /// 取出指定長度的隨機英數字
    /// </summary>
    /// <param name="size"></param>
    /// <returns></returns>
    public static string GetUniqueKey(int size)
    {
        byte[] data = new byte[4 * size];
        using (var crypto = RandomNumberGenerator.Create())
        {
            crypto.GetBytes(data);
        }
        StringBuilder result = new StringBuilder(size);
        for (int i = 0; i < size; i++)
        {
            var rnd = BitConverter.ToUInt32(data, i * 4);
            var idx = rnd % chars.Length;

            result.Append(chars[idx]);
        }

        return result.ToString();
    }
}