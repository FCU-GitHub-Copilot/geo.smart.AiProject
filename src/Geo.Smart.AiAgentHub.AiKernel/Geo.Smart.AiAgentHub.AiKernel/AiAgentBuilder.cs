using Geo.Smart.AiAgentHub.AiKernel.Vm;
using Geo.Smart.AiAgentHub.Infras.Enums;
using Microsoft.SemanticKernel;
using ModelContextProtocol.Client;

namespace Geo.Smart.AiAgentHub.AiKernel;

/// <summary>
/// 負責建立 AI Agent 及註冊 MCP 工具的建構器類別
/// </summary>
public static class AiAgentBuilder
{
    /// <summary>
    /// 依據 LLM 設定清單建立 Kernel 實例
    /// </summary>
    /// <param name="llmSetup">LLM 設定</param>
    /// <returns>已註冊 LLM 的 Kernel 實例</returns>
    public static Kernel GetKernel(LlmSetupVm llmSetup)
    {
        var builder = Kernel.CreateBuilder();
        AddChatCompletion(llmSetup, builder);
        return builder.Build();
    }

    /// <summary>
    /// 根據 LLM 設定將對應的 Chat Completion 註冊到 Kernel Builder
    /// </summary>
    /// <param name="llmSetup">LLM 設定資訊</param>
    /// <param name="builder">Kernel Builder 實例</param>
    private static void AddChatCompletion(LlmSetupVm llmSetup, IKernelBuilder builder)
    {
        switch (llmSetup.LlmSourceType)
        {
            case LlmSourceType.OpenAi:
                builder.AddOpenAIChatCompletion(
                    modelId: llmSetup.ModelId,
                    apiKey: GetApiKey(llmSetup.ApiKey),
                    serviceId: llmSetup.ServiceId
                );
                break;

            case LlmSourceType.AzureOpenAi:
                builder.AddAzureOpenAIChatCompletion(
                    modelId: llmSetup.ModelId,
                    apiKey: GetApiKey(llmSetup.ApiKey),
                    endpoint: llmSetup.Endpoint!,
                    deploymentName: llmSetup.DeploymentName!,
                    serviceId: llmSetup.ServiceId
                );
                break;

            case LlmSourceType.Ollama:
                builder.AddOllamaChatCompletion(
                    modelId: llmSetup.ModelId,
                    endpoint: new Uri(llmSetup.Endpoint!),
                    serviceId: llmSetup.ServiceId
                );
                break;

            case LlmSourceType.Gemini:
                builder.AddGoogleAIGeminiChatCompletion(
                    modelId: llmSetup.ModelId,
                    apiKey: GetApiKey(llmSetup.ApiKey),
                    serviceId: llmSetup.ServiceId
                );
                break;

            case LlmSourceType.Afs:
                builder.AddOpenAIChatCompletion(
                    modelId: llmSetup.ModelId,
                    apiKey: GetApiKey(llmSetup.ApiKey),
                    endpoint: new Uri(llmSetup.Endpoint!),
                    serviceId: llmSetup.ServiceId
                );
                break;

            default:
                break;
        }
    }

    private static string GetApiKey(string? key)
    {
        if (string.IsNullOrEmpty(key))
        {
            return string.Empty;
        }
        if (key.StartsWith("ENV:") && key.Length > 4)
        {
            var envId = key[4..].Trim();
            return Environment.GetEnvironmentVariable(envId) ?? envId;
        }
        return key;
    }

    /// <summary>
    /// 註冊 MCP 工具至 Kernel
    /// </summary>
    /// <param name="mcpServers">MCP 伺服器設定清單</param>
    /// <param name="kernel">Kernel 實例</param>
    /// <param name="ToolSelected">使用者挑選要使用的工具</param>
    public static async Task SetMcpTools(List<McpServerVm> mcpServers, Kernel kernel,
        Dictionary<string, List<string>> ToolSelected)
    {
        kernel.Plugins.Clear();
        foreach (var mcpServer in mcpServers)
        {
            var tools = await GetMcpClientToolsAsync(mcpServer).ConfigureAwait(false);

            // 篩選使用者選取的工具清單
            if (ToolSelected.TryGetValue(mcpServer.Name, out List<string>? selected))
            {
                tools = [.. tools.Where(tool => selected.Contains(tool.Name))];
            }

            kernel.Plugins.AddFromFunctions(
                mcpServer.Name,
                tools.Select(aiFunction => aiFunction.AsKernelFunction())
            );
        }
    }

    /// <summary>
    /// 取得 MCP Client 工具清單
    /// </summary>
    /// <param name="mcpServer">MCP SSE 伺服器設定</param>
    /// <returns>工具清單</returns>
    public static async Task<IList<McpClientTool>> GetMcpClientToolsAsync(McpServerVm mcpServer)
    {
        var clientTransport = CreateClientTransport(mcpServer);

        var mcpClient = await McpClientFactory.CreateAsync(clientTransport);
        var tools = await mcpClient.ListToolsAsync().ConfigureAwait(false);
        return tools;
    }

    /// <summary>
    /// 檢查並建立 MCP Client 的通訊物件
    /// </summary>
    /// <param name="mcpServer">MCP SSE 伺服器設定</param>
    /// <returns></returns>
    private static IClientTransport CreateClientTransport(McpServerVm mcpServer)
    {
        IClientTransport clientTransport;
        if (mcpServer.McpServerType == McpServerType.Stdio)
        {
            clientTransport = new StdioClientTransport(
                new StdioClientTransportOptions
                {
                    Name = mcpServer.Name,
                    Command = mcpServer.StdioCommand!,
                    Arguments = mcpServer.StdioArgs,
                    EnvironmentVariables = mcpServer.StdioEnv,
                }
            );
        }
        else if (mcpServer.McpServerType == McpServerType.Sse || mcpServer.McpServerType == McpServerType.Streamable)
        {
            clientTransport = new SseClientTransport(new SseClientTransportOptions
            {
                Name = mcpServer.Name,
                Endpoint = new Uri(mcpServer.SseUrl),
            });
        }
        else
        {
            throw new ArgumentException("不支援的 MCP 伺服器型別", nameof(mcpServer));
        }

        return clientTransport;
    }
}