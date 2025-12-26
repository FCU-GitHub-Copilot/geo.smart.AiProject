namespace Geo.Smart.AiAgentHub.Entities;

/// <summary>
/// 從 JWT Token 解析出的使用者資訊
/// </summary>
public class UserInfo
{
    /// <summary>
    /// 使用者 ID
    /// </summary>
    public string UserId { get; set; } = string.Empty;

    /// <summary>
    /// 使用者帳號 [UserName]
    /// </summary>
    public string Account { get; set; } = string.Empty;

    /// <summary>
    /// 角色 ID
    /// </summary>
    public string RoleId { get; set; } = string.Empty;

    /// <summary>
    /// 組織 ID
    /// </summary>
    public string OrgId { get; set; } = string.Empty;

    /// <summary>
    /// 使用者名稱
    /// </summary>
    public string FullName { get; set; } = string.Empty;
}