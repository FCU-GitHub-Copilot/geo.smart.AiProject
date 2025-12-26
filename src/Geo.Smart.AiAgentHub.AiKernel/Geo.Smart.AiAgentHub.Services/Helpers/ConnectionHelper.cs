namespace Geo.Smart.AiAgentHub.Services.Helpers;

/// <summary>
/// 資料庫連線小幫手
/// </summary>
public static class ConnectionHelper
{
    /// <summary>
    /// 資料庫連線字串解密
    /// </summary>
    /// <param name="cs">已加密的連線字串</param>
    /// <returns></returns>
    public static string GetDecrypt(string cs)
    {
        if (cs.StartsWith("Data Source=", StringComparison.OrdinalIgnoreCase))
        {
            return cs;
        }
        return SimpAesHelper.Decrypt(cs);
    }
}