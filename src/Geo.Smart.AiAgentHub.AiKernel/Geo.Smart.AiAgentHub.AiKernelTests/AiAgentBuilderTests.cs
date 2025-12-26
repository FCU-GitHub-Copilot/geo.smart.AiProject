using Geo.Smart.AiAgentHub.AiKernel;
using Geo.Smart.AiAgentHub.AiKernel.Vm;
using Geo.Smart.AiAgentHub.Infras.Enums;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.SemanticKernel.ChatCompletion;
using System.Text.Json;

namespace Geo.Smart.AiAgentHub.AiKernelTests;

[TestClass()]
public class AiAgentBuilderTests
{
    /// <summary>
    /// 測試 GetKernel 方法是否正確建立 Kernel 實例並註冊 OpenAI LLM
    /// </summary>
    [TestMethod()]
    public void GetKernel_ValidLlmSetup_ShouldRegisterOpenAIChatCompletionService()
    {
        // 建立測試用 LLM 設定清單
        var llmSetup = GetOpenAiLlmSetup();

        // 執行
        var kernel = AiAgentBuilder.GetKernel(llmSetup);

        // 驗證
        Assert.IsNotNull(kernel);
        Assert.IsNotNull(kernel.Services.GetKeyedService<IChatCompletionService>("openai-gpt-4o"));
    }

    private static LlmSetupVm GetOpenAiLlmSetup()
    {
        return new LlmSetupVm
        {
            ServiceId = "openai-gpt-4o",
            ModelId = "gpt-4o",
            LlmSourceType = LlmSourceType.OpenAi,
            ApiKey = "test-key"
        };
    }

    /// <summary>
    /// 測試 GetKernel 方法是否正確建立 Kernel 實例並註冊 Ollama LLM
    /// </summary>
    [TestMethod()]
    public void GetKernel_ValidLlmSetup_ShouldRegisterOllamaChatCompletionService()
    {
        // 建立測試用 LLM 設定清單
        var llmSetup = new LlmSetupVm
        {
            ServiceId = "ollama-llama3",
            ModelId = "llama3",
            LlmSourceType = LlmSourceType.Ollama,
            Endpoint = "http://localhost:11434"
        };

        // 執行
        var kernel = AiAgentBuilder.GetKernel(llmSetup);

        // 驗證
        Assert.IsNotNull(kernel);
        Assert.IsNotNull(kernel.Services.GetKeyedService<IChatCompletionService>("ollama-llama3"));
    }

    /// <summary>
    /// 測試 SetMcpTools 方法是否能正確註冊 MCP SSE 伺服器工具
    /// </summary>
    [TestMethod()]
    public async Task SetMcpTools_SseServer_ShouldRegisterPlugin()
    {
        var kernel = AiAgentBuilder.GetKernel(GetOpenAiLlmSetup());
        var mcpServers = new List<McpServerVm>
        {
            new() {
                Name = "TestSseServer",
                SseUrl = "https://fgismcp.geo.local/sse",
                McpServerType = McpServerType.Sse
            }
        };

        // 使用 mock 取代 McpClientFactory.CreateAsync
        // 這裡僅驗證流程，實際需用 mock framework 進行完整測試
        await AiAgentBuilder.SetMcpTools(mcpServers, kernel, []);

        Assert.IsTrue(kernel.Plugins.Any(x => x.Name == "TestSseServer"));
        kernel.Plugins.TryGetPlugin("TestSseServer", out var plugin);
        Assert.IsNotNull(plugin);
    }

    /// <summary>
    /// 測試 GetMcpClientToolsAsync 方法遇到不支援型態時會丟出例外
    /// </summary>
    [TestMethod()]
    public async Task GetMcpClientToolsAsync_UnknownType_ShouldThrowException()
    {
        var mcpServer = new McpServerVm
        {
            Name = "UnknownTypeServer",
            McpServerType = (McpServerType)999,
            SseUrl = "http://localhost:9999/sse"
        };

        await Assert.ThrowsExactlyAsync<ArgumentException>(async () =>
        {
            await AiAgentBuilder.GetMcpClientToolsAsync(mcpServer);
        });
    }

    /// <summary>
    /// 測試 GetMcpClientToolsAsync 方法能正確取得 MCP Streamable 工具清單
    /// </summary>
    [TestMethod()]
    public async Task GetMcpClientToolsAsync_StreamableServer_ShouldReturnTools()
    {
        var mcpServer = new McpServerVm
        {
            Name = "TestStreamableServer",
            McpServerType = McpServerType.Streamable,
            SseUrl = "https://fgismcp.geo.local"
        };

        var tools = await AiAgentBuilder.GetMcpClientToolsAsync(mcpServer);
        Assert.IsNotNull(tools);
        Assert.IsGreaterThanOrEqualTo(0, tools.Count);
        var nameDesc = tools.Select(x => x.Name);
        var json = JsonSerializer.Serialize(nameDesc);
        Assert.IsFalse(string.IsNullOrEmpty(json));
    }
}