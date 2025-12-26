using Geo.Smart.AiAgentHub.KmRag.Models;
using Geo.Smart.AiAgentHub.KmRag.Services.Contracts;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.KernelMemory;
using Microsoft.KernelMemory.Configuration;
using Microsoft.KernelMemory.Context;
using Microsoft.KernelMemory.Diagnostics;
using Microsoft.KernelMemory.DocumentStorage;
using Microsoft.KernelMemory.HTTP;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using System.Text.Json;

namespace Geo.Smart.AiAgentHub.KmRag;

public static class WebAPIEndpoints
{
    public static IEndpointRouteBuilder AddKernelMemoryEndpoints(
        this IEndpointRouteBuilder builder,
        string apiPrefix = "/",
        KernelMemoryConfig? kmConfig = null,
        IEndpointFilter[]? filters = null)
    {
        builder.AddPostUploadEndpoint(apiPrefix, kmConfig?.Service.GetMaxUploadSizeInBytes()).AddFilters(filters);
        builder.AddUploadStatusEndpoint(apiPrefix).AddFilters(filters);
        builder.AddAskEndpoint(apiPrefix, kmConfig?.Service.SendSSEDoneMessage ?? true).AddFilters(filters);
        builder.AddSearchEndpoint(apiPrefix).AddFilters(filters);
        builder.AddGetDownloadEndpoint(apiPrefix).AddFilters(filters);
        builder.AddListIndexesEndpoint(apiPrefix).AddFilters(filters);
        builder.AddDeleteIndexEndpoint(apiPrefix).AddFilters(filters);
        builder.AddDeleteDocumentEndpoint(apiPrefix).AddFilters(filters);
        return builder;
    }

    public static RouteHandlerBuilder AddPostUploadEndpoint(
        this IEndpointRouteBuilder builder,
        string apiPrefix = "/",
        long? maxUploadSizeInBytes = null)
    {
        RouteGroupBuilder group = builder.MapGroup(apiPrefix);

        // File upload endpoint
        var route = group.MapPost(Constants.HttpUploadEndpoint, async Task<IResult> (
                HttpRequest request,
                IIngestionService uploadService,
                ILogger<KernelMemoryWebAPI> log,
                IContextProvider contextProvider,
                CancellationToken cancellationToken) =>
            {
                if (maxUploadSizeInBytes.HasValue && request.HttpContext.Features.Get<IHttpMaxRequestBodySizeFeature>() is { } feature)
                {
                    log.LogTrace("Max upload request body size set to {MaxSize} bytes", maxUploadSizeInBytes.Value);
                    feature.MaxRequestBodySize = maxUploadSizeInBytes;
                }

                log.LogTrace("New upload HTTP request, content length {ContentLength}", request.ContentLength);

                // Note: .NET doesn't yet support binding multipart forms including data and files
                (HttpDocumentUploadRequest input, bool isValid, string errMsg)
                    = await HttpDocumentUploadRequest.BindHttpRequestAsync(request, cancellationToken)
                        .ConfigureAwait(false);

                // Allow internal classes to access custom arguments via IContextProvider
                contextProvider.InitContextArgs(input.ContextArguments);

                if (!isValid)
                {
                    log.LogError(errMsg);
                    return Results.Problem(detail: errMsg, statusCode: 400);
                }

                try
                {
                    var result = await uploadService
                        .ProcessUploadAsync(input, contextProvider.GetContext(), cancellationToken)
                        .ConfigureAwait(false);

                    var url = Constants.HttpUploadStatusEndpointWithParams
                        .Replace(Constants.HttpIndexPlaceholder, input.Index, StringComparison.Ordinal)
                        .Replace(Constants.HttpDocumentIdPlaceholder, result.DocumentId, StringComparison.Ordinal);

                    return Results.Accepted(url, result);
                }
                catch (Exception e)
                {
                    return Results.Problem(title: "Document upload failed", detail: e.Message, statusCode: 503);
                }
            })
            .WithName("UploadDocument")
            .WithDisplayName("UploadDocument")
            .WithOpenApi(operation =>
                {
                    operation.Summary = "Upload a new document to the knowledge base";
                    operation.Description = "Upload a document consisting of one or more files to extract memories from. The extraction process happens asynchronously. If a document with the same ID already exists, it will be overwritten and the memories previously extracted will be updated.";
                    operation.RequestBody = new OpenApiRequestBody
                    {
                        Content =
                        {
                            ["multipart/form-data"] = new OpenApiMediaType
                            {
                                Schema = new OpenApiSchema
                                {
                                    Type = "object",
                                    Properties =
                                    {
                                        ["index"] = new OpenApiSchema
                                        {
                                            Type = "string",
                                            Description = "Name of the index where to store memories generated by the files."
                                        },
                                        ["documentId"] = new OpenApiSchema
                                        {
                                            Type = "string",
                                            Description = "Unique ID used for import pipeline and document ID."
                                        },
                                        ["tags"] = new OpenApiSchema
                                        {
                                            Type = "array",
                                            Items = new OpenApiSchema { Type = "string" },
                                            Description = "Tags to apply to the memories extracted from the files.",
                                            Example = new OpenApiArray
                                            {
                                                new OpenApiString("group:abc123"),
                                                new OpenApiString("user:xyz")
                                            }
                                        },
                                        ["steps"] = new OpenApiSchema
                                        {
                                            Type = "array",
                                            Items = new OpenApiSchema { Type = "string" },
                                            Description = "How to process the files, e.g. how to extract/chunk etc.",
                                            Example = new OpenApiArray
                                            {
                                                new OpenApiString("extract"),
                                                new OpenApiString("partition"),
                                                new OpenApiString("gen_embeddings"),
                                                new OpenApiString("save_records"),
                                            }
                                        },
                                        ["files"] = new OpenApiSchema
                                        {
                                            Type = "array",
                                            Items = new OpenApiSchema
                                            {
                                                Type = "string",
                                                Format = "binary"
                                            },
                                            Description = "Files to process and extract memories from."
                                        }
                                    }
                                },
                                Encoding =
                                {
                                    { "tags", new OpenApiEncoding { Explode = true } },
                                    { "steps", new OpenApiEncoding { Explode = true } },
                                }
                            }
                        },
                        Description = "Document to upload and extract memories from"
                    };
                    return operation;
                }
            )
            .Produces<UploadAccepted>(StatusCodes.Status202Accepted)
            .Produces<ProblemDetails>(StatusCodes.Status400BadRequest)
            .Produces<ProblemDetails>(StatusCodes.Status401Unauthorized)
            .Produces<ProblemDetails>(StatusCodes.Status403Forbidden)
            .Produces<ProblemDetails>(StatusCodes.Status503ServiceUnavailable);

        return route;
    }

    public static RouteHandlerBuilder AddListIndexesEndpoint(
        this IEndpointRouteBuilder builder, string apiPrefix = "/")
    {
        RouteGroupBuilder group = builder.MapGroup(apiPrefix);

        // List of indexes endpoint
        var route = group.MapGet(Constants.HttpIndexesEndpoint,
                async Task<IResult> (
                    IIngestionService ingestionService,
                    ILogger<KernelMemoryWebAPI> log,
                    CancellationToken cancellationToken) =>
                {
                    log.LogTrace("New index list HTTP request");

                    IndexCollection result = await ingestionService
                        .ListIndexesAsync(cancellationToken)
                        .ConfigureAwait(false);

                    return Results.Ok(result);
                })
            .WithName("ListIndexes")
            .WithDisplayName("ListIndexes")
            .WithDescription("Get the list of containers (aka 'indexes') from the knowledge base.")
            .WithSummary("Get the list of containers (aka 'indexes') from the knowledge base. Each index has a unique name. Indexes are collections of memories extracted from the documents uploaded.")
            .Produces<IndexCollection>(StatusCodes.Status200OK)
            .Produces<ProblemDetails>(StatusCodes.Status401Unauthorized)
            .Produces<ProblemDetails>(StatusCodes.Status403Forbidden);

        return route;
    }

    public static RouteHandlerBuilder AddDeleteIndexEndpoint(
        this IEndpointRouteBuilder builder, string apiPrefix = "/", IEndpointFilter[]? filters = null)
    {
        RouteGroupBuilder group = builder.MapGroup(apiPrefix);

        // Delete index endpoint
        var route = group.MapDelete(Constants.HttpIndexesEndpoint,
                async Task<IResult> (
                    [FromQuery(Name = Constants.WebService.IndexField)]
                    string? index,
                    IIngestionService ingestionService,
                    ILogger<KernelMemoryWebAPI> log,
                    CancellationToken cancellationToken) =>
                {
                    log.LogTrace("New delete index HTTP request, index '{IndexName}'", index ?? "default");

                    DeleteAccepted result = await ingestionService
                        .DeleteIndexAsync(index: index, cancellationToken)
                        .ConfigureAwait(false);

                    var url = string.Empty;
                    return Results.Accepted(url, result);
                })
            .WithName("DeleteIndexByName")
            .WithDisplayName("DeleteIndexByName")
            .WithDescription("Delete a container of documents (aka 'index') from the knowledge base.")
            .WithSummary("Delete a container of documents (aka 'index') from the knowledge base. Indexes are collections of memories extracted from the documents uploaded.")
            .Produces<DeleteAccepted>(StatusCodes.Status202Accepted)
            .Produces<ProblemDetails>(StatusCodes.Status401Unauthorized)
            .Produces<ProblemDetails>(StatusCodes.Status403Forbidden);

        return route;
    }

    public static RouteHandlerBuilder AddDeleteDocumentEndpoint(
        this IEndpointRouteBuilder builder, string apiPrefix = "/", IEndpointFilter[]? filters = null)
    {
        RouteGroupBuilder group = builder.MapGroup(apiPrefix);

        // Delete document endpoint
        var route = group.MapDelete(Constants.HttpDocumentsEndpoint,
                async Task<IResult> (
                    [FromQuery(Name = Constants.WebService.IndexField)]
                    string? index,
                    [FromQuery(Name = Constants.WebService.DocumentIdField)]
                    string documentId,
                    IIngestionService ingestionService,
                    ILogger<KernelMemoryWebAPI> log,
                    CancellationToken cancellationToken) =>
                {
                    log.LogTrace("New delete document HTTP request, index '{IndexName}'", index ?? "default");

                    try
                    {
                        DeleteAccepted result = await ingestionService
                            .DeleteDocumentAsync(documentId: documentId, index: index, cancellationToken)
                            .ConfigureAwait(false);

                        var url = Constants.HttpUploadStatusEndpointWithParams
                            .Replace(Constants.HttpIndexPlaceholder, index, StringComparison.Ordinal)
                            .Replace(Constants.HttpDocumentIdPlaceholder, documentId, StringComparison.Ordinal);

                        return Results.Accepted(url, result);
                    }
                    catch (ArgumentException ex)
                    {
                        return Results.Problem(detail: ex.Message, statusCode: 400);
                    }
                })
            .WithName("DeleteDocumentById")
            .WithDisplayName("DeleteDocumentById")
            .WithDescription("Delete a document from the knowledge base.")
            .WithSummary("Delete a document from the knowledge base. When deleting a document, which can consist of multiple files, all the memories previously extracted are deleted too.")
            .Produces<DeleteAccepted>(StatusCodes.Status202Accepted)
            .Produces<ProblemDetails>(StatusCodes.Status400BadRequest)
            .Produces<ProblemDetails>(StatusCodes.Status401Unauthorized)
            .Produces<ProblemDetails>(StatusCodes.Status403Forbidden);

        return route;
    }

    public static RouteHandlerBuilder AddAskEndpoint(
        this IEndpointRouteBuilder builder, string apiPrefix = "/", bool sseSendDoneMessage = true, IEndpointFilter[]? filters = null)
    {
        RouteGroupBuilder group = builder.MapGroup(apiPrefix);

        // Ask endpoint
        var route = group.MapPost(Constants.HttpAskEndpoint,
                async Task (
                    HttpContext httpContext,
                    MemoryQuery query,
                    IRetrievalService retrievalService,
                    ILogger<KernelMemoryWebAPI> log,
                    IContextProvider contextProvider,
                    CancellationToken cancellationToken) =>
                {
                    contextProvider.InitContextArgs(query.ContextArguments);

                    log.LogTrace("New ask request, index '{IndexName}', minRelevance {MinRelevance}",
                        query.Index ?? "default", query.MinRelevance);

                    IAsyncEnumerable<MemoryAnswer> answerStream = retrievalService.AskStreamingAsync(
                        query: query,
                        context: contextProvider.GetContext(),
                        cancellationToken: cancellationToken);

                    httpContext.Response.StatusCode = StatusCodes.Status200OK;

                    try
                    {
                        if (query.Stream)
                        {
                            httpContext.Response.ContentType = "text/event-stream; charset=utf-8";
                            await foreach (var answer in answerStream.ConfigureAwait(false))
                            {
                                string json = answer.ToJson(true);
                                await httpContext.Response.WriteAsync($"{SSE.DataPrefix}{json}\n\n", cancellationToken).ConfigureAwait(false);
                                await httpContext.Response.Body.FlushAsync(cancellationToken).ConfigureAwait(false);
                            }
                        }
                        else
                        {
                            httpContext.Response.ContentType = "application/json; charset=utf-8";
                            MemoryAnswer answer = await answerStream.FirstAsync(cancellationToken: cancellationToken).ConfigureAwait(false);
                            string json = answer.ToJson(false);
                            await httpContext.Response.WriteAsync(json, cancellationToken).ConfigureAwait(false);
                        }
                    }
                    catch (Exception e)
                    {
                        log.LogError(e, "An error occurred while preparing the response");

                        httpContext.Response.StatusCode = StatusCodes.Status503ServiceUnavailable;

                        var json = query.Stream
                            ? JsonSerializer.Serialize(new MemoryAnswer
                            {
                                StreamState = StreamStates.Error,
                                Question = query.Question,
                                NoResult = true,
                                NoResultReason = $"Error: {e.Message} [{e.GetType().FullName}]"
                            })
                            : JsonSerializer.Serialize(new ProblemDetails
                            {
                                Status = StatusCodes.Status503ServiceUnavailable,
                                Title = "Service Unavailable",
                                Detail = $"{e.Message} [{e.GetType().FullName}]"
                            });

                        await httpContext.Response.WriteAsync(query.Stream ? $"{SSE.DataPrefix}{json}\n\n" : json, cancellationToken).ConfigureAwait(false);
                    }

                    if (query.Stream && sseSendDoneMessage)
                    {
                        await httpContext.Response.WriteAsync($"{SSE.DoneMessage}\n\n", cancellationToken: cancellationToken).ConfigureAwait(false);
                    }

                    await httpContext.Response.Body.FlushAsync(cancellationToken).ConfigureAwait(false);
                })
            .WithName("AnswerQuestion")
            .WithDisplayName("AnswerQuestion")
            .WithDescription("Answer a user question using the internal knowledge base.")
            .WithSummary("Use the memories extracted from the files uploaded to generate an answer. The query can include filters to use only a subset of the memories available.")
            .Produces<MemoryAnswer>(StatusCodes.Status200OK)
            .Produces<ProblemDetails>(StatusCodes.Status401Unauthorized)
            .Produces<ProblemDetails>(StatusCodes.Status403Forbidden)
            .Produces<ProblemDetails>(StatusCodes.Status503ServiceUnavailable);

        return route;
    }

    public static RouteHandlerBuilder AddSearchEndpoint(
        this IEndpointRouteBuilder builder, string apiPrefix = "/", IEndpointFilter[]? filters = null)
    {
        RouteGroupBuilder group = builder.MapGroup(apiPrefix);

        // Search endpoint
        var route = group.MapPost(Constants.HttpSearchEndpoint,
                async Task<IResult> (
                    SearchQuery query,
                    IRetrievalService retrievalService,
                    ILogger<KernelMemoryWebAPI> log,
                    IContextProvider contextProvider,
                    CancellationToken cancellationToken) =>
                {
                    contextProvider.InitContextArgs(query.ContextArguments);

                    log.LogTrace("New search HTTP request, index '{IndexName}', minRelevance {MinRelevance}",
                        query.Index ?? "default", query.MinRelevance);

                    SearchResult result = await retrievalService
                        .SearchAsync(query: query, context: contextProvider.GetContext(), cancellationToken: cancellationToken)
                        .ConfigureAwait(false);

                    return Results.Ok(result);
                })
            .WithName("SearchDocumentSnippets")
            .WithDisplayName("SearchDocumentSnippets")
            .WithDescription("Search the knowledge base for relevant snippets of text.")
            .WithSummary("Search the knowledge base for relevant snippets of text. The search can include filters to use only a subset of the knowledge base.")
            .Produces<SearchResult>(StatusCodes.Status200OK)
            .Produces<ProblemDetails>(StatusCodes.Status401Unauthorized)
            .Produces<ProblemDetails>(StatusCodes.Status403Forbidden);

        return route;
    }

    public static RouteHandlerBuilder AddUploadStatusEndpoint(
        this IEndpointRouteBuilder builder, string apiPrefix = "/", IEndpointFilter[]? filters = null)
    {
        RouteGroupBuilder group = builder.MapGroup(apiPrefix);

        // Document status endpoint
        var route = group.MapGet(Constants.HttpUploadStatusEndpoint,
                async Task<IResult> (
                    [FromQuery(Name = Constants.WebService.IndexField)]
                    string? index,
                    [FromQuery(Name = Constants.WebService.DocumentIdField)]
                    string documentId,
                    IIngestionService ingestionService,
                    ILogger<KernelMemoryWebAPI> log,
                    CancellationToken cancellationToken) =>
                {
                    log.LogTrace("New document status HTTP request");

                    try
                    {
                        DataPipelineStatus? pipeline = await ingestionService
                            .GetDocumentStatusAsync(documentId: documentId, index: index, cancellationToken)
                            .ConfigureAwait(false);

                        if (pipeline == null)
                        {
                            return Results.Problem(detail: "Document not found or empty pipeline", statusCode: 404);
                        }

                        return Results.Ok(pipeline);
                    }
                    catch (ArgumentException ex)
                    {
                        return Results.Problem(detail: ex.Message, statusCode: 400);
                    }
                })
            .WithName("CheckDocumentStatus")
            .WithDisplayName("CheckDocumentStatus")
            .WithDescription("Check the status of a file upload in progress.")
            .WithSummary("Check the status of a file upload in progress. When uploading a document, which can consist of multiple files, each file goes through multiple steps. The status include details about which steps are completed.")
            .Produces<DataPipelineStatus>(StatusCodes.Status200OK)
            .Produces<ProblemDetails>(StatusCodes.Status400BadRequest)
            .Produces<ProblemDetails>(StatusCodes.Status401Unauthorized)
            .Produces<ProblemDetails>(StatusCodes.Status403Forbidden)
            .Produces<ProblemDetails>(StatusCodes.Status404NotFound)
            .Produces<ProblemDetails>(StatusCodes.Status413PayloadTooLarge);

        return route;
    }

    public static RouteHandlerBuilder AddGetDownloadEndpoint(
        this IEndpointRouteBuilder builder, string apiPrefix = "/", IEndpointFilter[]? filters = null)
    {
        RouteGroupBuilder group = builder.MapGroup(apiPrefix);

        // File download endpoint
        var route = group.MapGet(Constants.HttpDownloadEndpoint, async Task<IResult> (
                [FromQuery(Name = Constants.WebService.IndexField)]
                string? index,
                [FromQuery(Name = Constants.WebService.DocumentIdField)]
                string documentId,
                [FromQuery(Name = Constants.WebService.FilenameField)]
                string filename,
                HttpContext httpContext,
                IRetrievalService retrievalService,
                ILogger<KernelMemoryWebAPI> log,
                CancellationToken cancellationToken) =>
            {
                log.LogTrace("New download file HTTP request, index {IndexName}, documentId {DocumentId}, fileName {FileName}",
                    index ?? "default", documentId, filename);

                try
                {
                    StreamableFileContent? file = await retrievalService
                        .ExportFileAsync(documentId: documentId, filename: filename, index: index, cancellationToken: cancellationToken)
                        .ConfigureAwait(false);

                    if (file == null)
                    {
                        return Results.Problem(title: "File not found", statusCode: 404);
                    }

                    log.LogTrace("Downloading file '{FileName}', size '{FileSize}', type '{FileType}'",
                        filename, file.FileSize, file.FileType);

                    Stream resultingFileStream = await file.GetStreamAsync().WaitAsync(cancellationToken).ConfigureAwait(false);
                    var response = Results.Stream(
                        resultingFileStream,
                        contentType: file.FileType,
                        fileDownloadName: filename,
                        lastModified: file.LastWrite,
                        enableRangeProcessing: true);

                    if (response is FileStreamHttpResult { FileLength: null or 0 })
                    {
                        httpContext.Response.Headers.ContentLength = file.FileSize;
                    }

                    return response;
                }
                catch (ArgumentException ex)
                {
                    return Results.Problem(detail: ex.Message, statusCode: 400);
                }
                catch (DocumentStorageFileNotFoundException ex)
                {
                    return Results.Problem(title: "File not found", detail: ex.Message, statusCode: 404);
                }
                catch (Exception ex)
                {
                    return Results.Problem(title: "File download failed", detail: ex.Message, statusCode: 503);
                }
            })
            .WithName("DownloadFile")
            .WithDisplayName("DownloadFile")
            .WithDescription("Download a file referenced by a previous answer or search result.")
            .WithSummary("Download a file referenced by a previous answer or search result. The file is returned as the original copy, retrieved from the document storage.")
            .WithGroupName("Search")
            .Produces<StreamableFileContent>(StatusCodes.Status200OK)
            .Produces<ProblemDetails>(StatusCodes.Status400BadRequest)
            .Produces<ProblemDetails>(StatusCodes.Status404NotFound)
            .Produces<ProblemDetails>(StatusCodes.Status401Unauthorized)
            .Produces<ProblemDetails>(StatusCodes.Status403Forbidden)
            .Produces<ProblemDetails>(StatusCodes.Status503ServiceUnavailable);

        return route;
    }

#pragma warning disable CA1812 // used by logger, can't be static

    // Class used to tag log entries and allow log filtering
    // ReSharper disable once ClassNeverInstantiated.Local
    private sealed class KernelMemoryWebAPI;

#pragma warning restore CA1812
}

internal static class EndpointConventionBuilderExtensions
{
    internal static void AddFilters(this IEndpointConventionBuilder route, IEndpointFilter[]? filters = null)
    {
        if (filters == null || filters.Length == 0) { return; }

        foreach (var filter in filters)
        {
            route.AddEndpointFilter(filter);
        }
    }
}