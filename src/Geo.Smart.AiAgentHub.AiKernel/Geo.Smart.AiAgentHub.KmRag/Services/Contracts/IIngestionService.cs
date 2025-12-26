using Geo.Smart.AiAgentHub.KmRag.Models;
using Microsoft.KernelMemory;
using Microsoft.KernelMemory.Context;

namespace Geo.Smart.AiAgentHub.KmRag.Services.Contracts;

/// <summary>
/// 文件擷取服務介面
/// </summary>
public interface IIngestionService
{
    /// <summary>
    /// 處理文件上傳請求
    /// </summary>
    /// <param name="input">HTTP 文件上傳請求</param>
    /// <param name="context">內容物件</param>
    /// <param name="cancellationToken">取消標記</param>
    /// <returns>上傳接受回應</returns>
    Task<UploadAccepted> ProcessUploadAsync(
        HttpDocumentUploadRequest input,
        IContext context,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 取得文件處理狀態
    /// </summary>
    /// <param name="documentId">文件識別碼</param>
    /// <param name="index">索引名稱</param>
    /// <param name="cancellationToken">取消標記</param>
    /// <returns>資料管線狀態</returns>
    Task<DataPipelineStatus?> GetDocumentStatusAsync(
        string documentId,
        string? index = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 取得索引列表
    /// </summary>
    /// <param name="cancellationToken">取消標記</param>
    /// <returns>索引集合</returns>
    Task<IndexCollection> ListIndexesAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// 刪除索引
    /// </summary>
    /// <param name="index">索引名稱</param>
    /// <param name="cancellationToken">取消標記</param>
    /// <returns>刪除接受回應</returns>
    Task<DeleteAccepted> DeleteIndexAsync(string? index, CancellationToken cancellationToken = default);

    /// <summary>
    /// 刪除文件
    /// </summary>
    /// <param name="documentId">文件識別碼</param>
    /// <param name="index">索引名稱</param>
    /// <param name="cancellationToken">取消標記</param>
    /// <returns>刪除接受回應</returns>
    Task<DeleteAccepted> DeleteDocumentAsync(string documentId, string? index = null, CancellationToken cancellationToken = default);
}