using static Geo.Smart.AiAgentHub.Infras.ConstantData;

namespace Geo.Smart.AiAgentHub.Services.Extension;

/// <summary>
/// 系統常用設定值
/// </summary>
public static class DefaultCfgs
{
    /// <summary>
    /// 是否為系統管理者
    /// </summary>
    /// <param name="userInfo"></param>
    /// <returns></returns>
    public static bool IsAdmin(UserInfo userInfo)
    {
        return userInfo.RoleId == Roles.系統管理者;
    }
}