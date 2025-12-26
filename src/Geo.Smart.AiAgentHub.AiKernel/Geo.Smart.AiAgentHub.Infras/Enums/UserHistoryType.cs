#pragma warning disable CS1591 // 遺漏公用可見類型或成員的 XML 註解

namespace Geo.Smart.AiAgentHub.Infras.Enums;

/// <summary>
/// 使用者資料異動類型
/// </summary>
public enum UserHistoryType
{
    新增帳號,
    編輯帳號,
    刪除帳號,
    變更密碼,
    登入系統,
    登出系統,
    鎖定帳號,
    取消鎖定,
}