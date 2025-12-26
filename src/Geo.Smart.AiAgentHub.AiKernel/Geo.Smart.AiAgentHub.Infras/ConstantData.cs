#pragma warning disable CS1591 // 遺漏公用可見類型或成員的 XML 註解

namespace Geo.Smart.AiAgentHub.Infras;

/// <summary>
/// 系統常值
/// </summary>
public static class ConstantData
{
    /// <summary>
    /// 錯誤訊息
    /// </summary>
    public static class Error
    {
        public const string Exception = "系統發生錯誤，請聯絡系統管理員";
        public const string NoAuthority = "無權限";
        public const string NoData = "查無資料";
        public const string CaptchaError = "圖形驗證碼錯誤";

        /// <summary>
        /// 取得組織樹失敗錯誤訊息
        /// </summary>
        public const string OrgTreeError = "取得組織樹失敗";

        /// <summary>
        /// 同步與更新組織樹失敗錯誤訊息
        /// </summary>
        public const string SyncPortalOrgError = "同步組織樹失敗";
    }

    /// <summary>
    /// 一般通用的常數
    /// </summary>
    public static class CommonConst
    {
        /// <summary>
        /// 開發階段預設的 Captcha Code
        /// </summary>
        public const string Code9527 = "095270";

        public const string LoginSuccess = "登入成功";
    }

    /// <summary>
    /// 角色
    /// </summary>
    public static class Roles
    {
        public const string 系統管理者 = "0b1aac62-10eb-4c16-9558-f86e9979c90b";
    }

    /// <summary>
    /// 刪除檔案或圖片時，可以執行的角色清單
    /// </summary>
    public static readonly string[] FileAdminRoles =
    [
        Roles.系統管理者
    ];

    /// <summary>
    /// 組織 ID
    /// </summary>
    public static class OrgIds
    {
        /// <summary>
        /// 組織樹的根 395000000A	臺南市政府
        /// </summary>
        public const string Root = "395000000A";
    }

    public static class QooLetters
    {
        public const string Uppercase = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        public const string Lowercase = "abcdefghijklmnopqrstuvwxyz";
        public const string Digits = "0123456789";
        public const string SpecialCharacters = "@$#%^&*()_+-=[]{}|;:,.?!";
    }

    /// <summary>
    /// ContentType
    /// </summary>
    public static class ContentType
    {
        public const string Xlsx = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
        public const string Docx = "application/vnd.openxmlformats-officedocument.wordprocessingml.document";
        public const string Xml = "application/xml";
    }

    public static class CacheKeys
    {
        /// <summary>
        /// 快取時間
        /// </summary>
        public const int CacheMinutes = 60;
    }
}