using Geo.Smart.AiAgentHub.KmRag.Models;
using Microsoft.KernelMemory;
using Microsoft.KernelMemory.Context;

namespace Geo.Smart.AiAgentHub.KmRag.Services.Contracts;

/// <summary>
/// 檢索服務介面
/// </summary>
public interface IRetrievalService
{
    /// <summary>
    /// 處理問答請求
    /// </summary>
    /// <param name="query">記憶體查詢</param>
    /// <param name="context">內容物件</param>
    /// <param name="cancellationToken">取消標記</param>
    /// <returns>記憶體答案串流</returns>
    IAsyncEnumerable<MemoryAnswer> AskStreamingAsync(
        MemoryQuery query,
        IContext context,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 處理搜尋請求
    /// </summary>
    /// <param name="query">搜尋查詢</param>
    /// <param name="context">內容物件</param>
    /// <param name="cancellationToken">取消標記</param>
    /// <returns>搜尋結果</returns>
    Task<SearchResult> SearchAsync(
        SearchQuery query,
        IContext context,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 匯出檔案
    /// </summary>
    /// <param name="documentId">文件識別碼</param>
    /// <param name="filename">檔案名稱</param>
    /// <param name="index">索引名稱</param>
    /// <param name="cancellationToken">取消標記</param>
    /// <returns>可串流檔案內容</returns>
    Task<StreamableFileContent?> ExportFileAsync(
        string documentId,
        string filename,
        string? index = null,
        CancellationToken cancellationToken = default);
}
