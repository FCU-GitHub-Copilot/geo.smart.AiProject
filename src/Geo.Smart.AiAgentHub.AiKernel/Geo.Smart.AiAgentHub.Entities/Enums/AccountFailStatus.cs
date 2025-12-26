#pragma warning disable CS1591 // 遺漏公用可見類型或成員的 XML 註解

namespace Geo.Smart.AiAgentHub.Entities.Enums;

/// <summary>
/// 帳號註冊、強制更新密碼、忘記密碼失敗狀態
/// </summary>
public enum AccountFailStatus
{
    密碼更新失敗,
    新密碼未輸入,
    新密碼與確認新密碼不相同,
    超過有效時間,
    帳號不存在,
    前三次密碼相同,
    Email重複,
    Email必填,
    Email格式錯誤,
    密碼必填,
    密碼格式錯誤,
    寄發驗證信失敗,
    無驗證碼紀錄,
    驗證碼檢核失敗,
    註冊帳號失敗,
    註冊驗證失敗,
    忘記密碼失敗,
    驗證碼不符,
    帳號必填,
    姓名必填
}