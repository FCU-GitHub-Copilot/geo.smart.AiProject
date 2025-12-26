using Geo.Smart.AiAgentHub.KmRag.Models;
using Geo.Smart.AiAgentHub.KmRag.Services.Contracts;
using Microsoft.Extensions.Logging;
using Microsoft.KernelMemory;
using Microsoft.KernelMemory.Context;
using Microsoft.KernelMemory.DocumentStorage;

namespace Geo.Smart.AiAgentHub.KmRag.Services;

/// <summary>
/// 檢索服務實作
/// </summary>
public class RetrievalService : IRetrievalService
{
    private readonly IKernelMemory _kernelMemory;
    private readonly ILogger<RetrievalService> _logger;

    /// <summary>
    /// 建構函式
    /// </summary>
    /// <param name="kernelMemory">Kernel Memory 服務</param>
    /// <param name="logger">日誌記錄器</param>
    public RetrievalService(
        IKernelMemory kernelMemory,
        ILogger<RetrievalService> logger)
    {
        _kernelMemory = kernelMemory;
        _logger = logger;
    }

    /// <summary>
    /// 處理問答請求
    /// </summary>
    /// <param name="query">記憶體查詢</param>
    /// <param name="context">內容物件</param>
    /// <param name="cancellationToken">取消標記</param>
    /// <returns>記憶體答案串流</returns>
    public IAsyncEnumerable<MemoryAnswer> AskStreamingAsync(
        MemoryQuery query,
        IContext context,
        CancellationToken cancellationToken = default)
    {
        _logger.LogTrace("Asking question in index '{IndexName}', minRelevance {MinRelevance}",
            query.Index ?? "default", query.MinRelevance);

        return _kernelMemory.AskStreamingAsync(
            question: query.Question,
            index: query.Index,
            filters: query.Filters,
            minRelevance: query.MinRelevance,
            options: new SearchOptions { Stream = query.Stream },
            context: context,
            cancellationToken: cancellationToken);
    }

    /// <summary>
    /// 處理搜尋請求
    /// </summary>
    /// <param name="query">搜尋查詢</param>
    /// <param name="context">內容物件</param>
    /// <param name="cancellationToken">取消標記</param>
    /// <returns>搜尋結果</returns>
    public async Task<SearchResult> SearchAsync(
        SearchQuery query,
        IContext context,
        CancellationToken cancellationToken = default)
    {
        _logger.LogTrace("Searching in index '{IndexName}', minRelevance {MinRelevance}",
            query.Index ?? "default", query.MinRelevance);

        var result = await _kernelMemory.SearchAsync(
                query: query.Query,
                index: query.Index,
                filters: query.Filters,
                minRelevance: query.MinRelevance,
                limit: query.Limit,
                context: context,
                cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        return result;
    }

    /// <summary>
    /// 匯出檔案
    /// </summary>
    /// <param name="documentId">文件識別碼</param>
    /// <param name="filename">檔案名稱</param>
    /// <param name="index">索引名稱</param>
    /// <param name="cancellationToken">取消標記</param>
    /// <returns>可串流檔案內容</returns>
    public async Task<StreamableFileContent?> ExportFileAsync(
        string documentId,
        string filename,
        string? index = null,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(documentId))
        {
            throw new ArgumentException("文件識別碼不可為空", nameof(documentId));
        }

        if (string.IsNullOrWhiteSpace(filename))
        {
            throw new ArgumentException("檔案名稱不可為空", nameof(filename));
        }

        _logger.LogTrace("Exporting file '{FileName}' from document '{DocumentId}' in index '{IndexName}'",
            filename, documentId, index ?? "default");

        try
        {
            var file = await _kernelMemory.ExportFileAsync(
                    documentId: documentId,
                    fileName: filename,
                    index: index,
                    cancellationToken: cancellationToken)
                .ConfigureAwait(false);

            if (file == null)
            {
                _logger.LogWarning("File not found: '{FileName}' in document '{DocumentId}'", filename, documentId);
                return null;
            }

            _logger.LogTrace("File '{FileName}' found, size '{FileSize}', type '{FileType}'",
                filename, file.FileSize, file.FileType);

            return file;
        }
        catch (DocumentStorageFileNotFoundException ex)
        {
            _logger.LogWarning(ex, "File not found: '{FileName}' in document '{DocumentId}'", filename, documentId);
            throw;
        }
    }
}
