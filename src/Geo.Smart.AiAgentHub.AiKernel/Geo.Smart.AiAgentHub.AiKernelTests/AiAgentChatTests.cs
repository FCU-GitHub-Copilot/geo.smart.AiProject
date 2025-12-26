using Geo.Smart.AiAgentHub.AiKernel;
using Geo.Smart.AiAgentHub.AiKernel.Vm;
using Geo.Smart.AiAgentHub.Infras.Enums;
using Microsoft.SemanticKernel;

namespace Geo.Smart.AiAgentHub.AiKernelTests;

[TestClass()]
public class AiAgentChatTests
{
    /// <summary>
    /// 測試 AiAgentChat.Ask 方法是否能正確呼叫 Kernel 並取得聊天訊息內容
    /// </summary>
    [TestMethod()]
    public async Task Ask_ValidRequest_ShouldReturnChatMessageContent()
    {
        var apiKey = Environment.GetEnvironmentVariable("OPENAI_API_KEY") ?? string.Empty;

        var llmSetup = new LlmSetupVm
        {
            ServiceId = "openai_gpt4o",
            ModelId = "gpt-4o",
            LlmSourceType = LlmSourceType.OpenAi,
            ApiKey = apiKey
        };
        var projectSetting = new ProjectSettingVm
        {
            Name = "TestProject",
            SystemPrompt = "你是 AI 助手",
            LlmInfos = [llmSetup],
            McpServers = []
        };
        var askVm = new AskVm
        {
            Message = "請提供麥可喬丹的英文名字",
            ServiceId = llmSetup.ServiceId,
            ToolSelected = [],
            ConnectionId = string.Empty
        };
        var request = new ChatCompletionRequest
        {
            ProjectSetting = projectSetting,
            ChatHistory = []
        };
        var cancellationToken = CancellationToken.None;

        var result = await AiAgentChat.Ask(askVm, request, cancellationToken);

        Assert.IsNotNull(result);
        Assert.IsInstanceOfType(result, typeof(ChatMessageContent));

        var output = result.ToString();
        Assert.Contains("Jordan", output);
    }
}