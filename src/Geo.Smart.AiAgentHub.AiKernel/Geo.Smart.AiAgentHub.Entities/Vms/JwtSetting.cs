namespace Geo.Smart.AiAgentHub.Entities.Vms;

/// <summary>
/// JWT Token 發行設定參數
/// </summary>
public class JwtSetting
{
    /// <summary>
    /// 建構式
    /// </summary>
    public JwtSetting()
    {
        Issuer = string.Empty;
        SignKey = string.Empty;
    }

    /// <summary>
    /// jwt 發者
    /// </summary>
    public string Issuer { get; set; }

    /// <summary>
    /// 發行 jwt 簽章的金鑰
    /// </summary>
    public string SignKey { get; set; }
}