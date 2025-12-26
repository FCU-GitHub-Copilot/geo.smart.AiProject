using Geo.Smart.AiAgentHub.Infras;
using Microsoft.Extensions.Logging;

namespace Geo.Smart.AiAgentHub.Services.Common;

/// <summary>
/// 提供具備資料庫存取與日誌功能的服務基底類別
/// </summary>
/// <remarks>
/// 此抽象類別作為需要存取資料庫與日誌功能之服務的基礎，
/// 實作 <see cref="IDisposable"/> 介面以確保資源正確釋放
/// 派生類別可覆寫 <see cref="Dispose(bool)"/> 方法以擴充額外釋放行為
/// </remarks>
public abstract class BaseService : IDisposable
{
    /// <summary>
    /// 取得或設定資料庫內容物件
    /// </summary>
    protected GdbContext DbModel { get; set; }

    /// <summary>
    /// 取得日誌記錄器
    /// </summary>
    protected ILogger? Logger { get; }

    /// <summary>
    /// 建構函式 (接受 GdbContext 和 ILogger)
    /// </summary>
    /// <param name="dbModel">資料庫內容物件</param>
    /// <param name="logger">日誌記錄器</param>
    protected BaseService(GdbContext dbModel, ILogger logger)
    {
        DbModel = dbModel;
        Logger = logger;
    }

    /// <summary>
    /// 記錄錯誤日誌
    /// </summary>
    /// <param name="e">例外狀況物件</param>
    /// <param name="methodName">發生錯誤的方法名稱</param>
    protected void LogError(Exception e, string methodName)
    {
        Logger?.LogError(e, "Error {MethodName}：{Message}", methodName, e.Message);
    }

    /// <summary>
    /// 記錄錯誤日誌並回傳失敗結果
    /// </summary>
    /// <typeparam name="T">回傳型別</typeparam>
    /// <param name="e">例外狀況物件</param>
    /// <param name="methodName">發生錯誤的方法名稱</param>
    /// <returns>失敗的結果物件</returns>
    protected Result<T> LogAndReturn<T>(Exception e, string methodName) where T : class
    {
        LogError(e, methodName);
        return ResultHelper.Failure<T>(ConstantData.Error.Exception);
    }

    /// <summary>
    /// 記錄錯誤日誌並回傳分頁失敗結果
    /// </summary>
    /// <typeparam name="T">回傳型別</typeparam>
    /// <param name="e">例外狀況物件</param>
    /// <param name="methodName">發生錯誤的方法名稱</param>
    /// <returns>分頁失敗的結果物件</returns>
    protected PaginationResult<T> LogAndReturnByPage<T>(Exception e, string methodName) where T : class
    {
        LogError(e, methodName);
        return ResultHelper.PaginationFailure<T>(ConstantData.Error.Exception);
    }

    /// <summary>
    /// 釋放資源
    /// </summary>
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// 釋放資源
    /// </summary>
    /// <param name="disposing">是否釋放受控資源</param>
    protected virtual void Dispose(bool disposing)
    {
        if (!disposing || DbModel == null)
        {
            return;
        }
        DbModel.Dispose();
    }
}