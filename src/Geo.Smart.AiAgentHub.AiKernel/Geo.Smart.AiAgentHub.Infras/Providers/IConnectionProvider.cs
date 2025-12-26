namespace Geo.Smart.AiAgentHub.Infras.Providers;

/// <summary>
/// 提供資料庫連線字串的服務類別
/// </summary>
public interface IConnectionProvider
{
    /// <summary>
    /// 取得 GDB 資料庫的連線字串
    /// </summary>
    string GdbConnectionString { get; }

    /// <summary>
    /// 取得連線資訊中的應用程式名稱
    /// </summary>
    string ConnectionAppName { get; }
}