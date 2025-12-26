using Geo.Smart.AiAgentHub.KmRag.Models;
using Geo.Smart.AiAgentHub.KmRag.Services.Contracts;
using Microsoft.KernelMemory;
using Microsoft.KernelMemory.Context;

namespace Geo.Smart.AiAgentHub.KmRag.Services;

/// <summary>
/// 文件擷取服務介面
/// </summary>
public class IngestionService : IIngestionService
{
    private readonly IKernelMemory _kernelMemory;
    private readonly ILogger<IngestionService> _logger;

    /// <summary>
    /// 建構函式
    /// </summary>
    /// <param name="kernelMemory">Kernel Memory 服務</param>
    /// <param name="logger">日誌記錄器</param>
    public IngestionService(
        IKernelMemory kernelMemory,
        ILogger<IngestionService> logger)
    {
        _kernelMemory = kernelMemory;
        _logger = logger;
    }

    /// <summary>
    /// 處理文件上傳請求
    /// </summary>
    /// <param name="input">HTTP 文件上傳請求</param>
    /// <param name="context">內容物件</param>
    /// <param name="cancellationToken">取消標記</param>
    /// <returns>上傳接受回應</returns>
    public async Task<UploadAccepted> ProcessUploadAsync(
        HttpDocumentUploadRequest input,
        IContext context,
        CancellationToken cancellationToken = default)
    {
        _logger.LogTrace("Index '{IndexName}'", input.Index);

        var documentId = await _kernelMemory
            .ImportDocumentAsync(input.ToDocumentUploadRequest(), context, cancellationToken)
            .ConfigureAwait(false);

        _logger.LogTrace("Doc Id '{DocumentId}'", documentId);

        return new UploadAccepted
        {
            DocumentId = documentId,
            Index = input.Index,
            Message = "Document upload completed, ingestion pipeline started"
        };
    }

    /// <summary>
    /// 取得文件處理狀態
    /// </summary>
    /// <param name="documentId">文件識別碼</param>
    /// <param name="index">索引名稱</param>
    /// <param name="cancellationToken">取消標記</param>
    /// <returns>資料管線狀態</returns>
    public async Task<DataPipelineStatus?> GetDocumentStatusAsync(
        string documentId,
        string? index = null,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrEmpty(documentId))
        {
            throw new ArgumentException("文件識別碼不可為空", nameof(documentId));
        }

        _logger.LogTrace("Getting document status for documentId '{DocumentId}', index '{IndexName}'",
            documentId, index ?? "default");

        var pipeline = await _kernelMemory
            .GetDocumentStatusAsync(documentId: documentId, index: index, cancellationToken)
            .ConfigureAwait(false);

        if (pipeline == null)
        {
            _logger.LogWarning("Document not found: '{DocumentId}'", documentId);
            return null;
        }

        if (pipeline.Empty)
        {
            _logger.LogWarning("Empty pipeline for document: '{DocumentId}'", documentId);
            return null;
        }

        return pipeline;
    }

    /// <summary>
    /// 取得索引列表
    /// </summary>
    /// <param name="cancellationToken">取消標記</param>
    /// <returns>索引集合</returns>
    public async Task<IndexCollection> ListIndexesAsync(CancellationToken cancellationToken = default)
    {
        _logger.LogTrace("Listing indexes");

        var result = new IndexCollection();
        IEnumerable<IndexDetails> list = await _kernelMemory
            .ListIndexesAsync(cancellationToken)
            .ConfigureAwait(false);

        foreach (IndexDetails index in list)
        {
            result.Results.Add(index);
        }

        _logger.LogTrace("Found {Count} indexes", result.Results.Count);

        return result;
    }

    /// <summary>
    /// 刪除索引
    /// </summary>
    /// <param name="index">索引名稱</param>
    /// <param name="cancellationToken">取消標記</param>
    /// <returns>刪除接受回應</returns>
    public async Task<DeleteAccepted> DeleteIndexAsync(string? index, CancellationToken cancellationToken = default)
    {
        _logger.LogTrace("Deleting index '{IndexName}'", index ?? "default");

        await _kernelMemory
            .DeleteIndexAsync(index: index, cancellationToken)
            .ConfigureAwait(false);

        return new DeleteAccepted
        {
            Index = index ?? string.Empty,
            Message = "Index deletion request received, pipeline started"
        };
    }

    /// <summary>
    /// 刪除文件
    /// </summary>
    /// <param name="documentId">文件識別碼</param>
    /// <param name="index">索引名稱</param>
    /// <param name="cancellationToken">取消標記</param>
    /// <returns>刪除接受回應</returns>
    public async Task<DeleteAccepted> DeleteDocumentAsync(string documentId, string? index = null, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrEmpty(documentId))
        {
            throw new ArgumentException("文件識別碼不可為空", nameof(documentId));
        }

        _logger.LogTrace("Deleting document '{DocumentId}' from index '{IndexName}'",
            documentId, index ?? "default");

        await _kernelMemory
            .DeleteDocumentAsync(documentId: documentId, index: index, cancellationToken)
            .ConfigureAwait(false);

        return new DeleteAccepted
        {
            DocumentId = documentId,
            Index = index ?? string.Empty,
            Message = "Document deletion request received, pipeline started"
        };
    }
}