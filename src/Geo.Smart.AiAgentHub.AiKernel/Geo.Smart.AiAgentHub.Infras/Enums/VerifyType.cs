#pragma warning disable CS1591 // 遺漏公用可見類型或成員的 XML 註解

namespace Geo.Smart.AiAgentHub.Infras.Enums;

/// <summary>
/// 驗證類型
/// </summary>
public enum VerifyType
{
    信箱註冊 = 1,
    第一次登入 = 2,
    三個月強制變更密碼 = 3,
    忘記密碼 = 4,
}