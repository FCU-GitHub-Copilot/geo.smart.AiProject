#pragma warning disable CS1591 // 遺漏公用可見類型或成員的 XML 註解

namespace Geo.Smart.AiAgentHub.Infras.Enums;

/// <summary>
/// LLM 服務來源類型
/// </summary>
public enum LlmSourceType
{
    OpenAi,
    AzureOpenAi,
    Ollama,
    Gemini,

    // 數發部的算力平台
    Afs,
    //Anthropic,
    //HuggingFace,
}