#pragma warning disable CS1591 // 遺漏公用可見類型或成員的 XML 註解

namespace Geo.Smart.AiAgentHub.Entities.Enums;

/// <summary>
/// 登入失敗狀態
/// </summary>
public enum LoginFailStatus
{
    尚未填寫驗證碼,
    帳號已鎖定,
    登入失敗,
    未通過二階段驗證,
    不允許登入,
    需強制修改密碼,
    驗證碼不符,
    帳號已停用,
    更新權杖已過期,
}