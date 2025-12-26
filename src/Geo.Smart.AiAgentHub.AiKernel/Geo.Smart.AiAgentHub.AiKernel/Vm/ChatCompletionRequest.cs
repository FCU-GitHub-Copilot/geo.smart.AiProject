using Microsoft.SemanticKernel.ChatCompletion;

namespace Geo.Smart.AiAgentHub.AiKernel.Vm;

/// <summary>
/// 聊天完成的請求參數
/// </summary>
public sealed class ChatCompletionRequest
{
    /// <summary>
    /// 專案設定黨
    /// </summary>
    public required ProjectSettingVm ProjectSetting { get; set; }

    /// <summary>
    /// 取得或設定聊天歷史記錄
    /// </summary>
    public required ChatHistory ChatHistory { get; set; }
}