namespace Geo.Smart.AiAgentHub.Services.Contracts;

/// <summary>
/// 共通性資料服務
/// </summary>
public interface ICommonService
{
    /// <summary>
    /// 取得所有列舉資料對應表
    /// </summary>
    /// <returns>回傳所有列舉資料的字典，Key 為類型名稱，Value 為對應的 KeyName 清單</returns>
    Dictionary<string, List<KeyName>> GetAllMapping();

    /// <summary>
    /// 取得組織下拉選單清單
    /// </summary>
    /// <returns>回傳組織的 CodeName 清單</returns>
    List<CodeName> GetOrgs();

    /// <summary>
    /// 取得角色選單
    /// </summary>
    /// <param name="isAll">true: 取得所有角色，false: 不包含管理者角色</param>
    /// <returns>回傳角色的 CodeName 清單</returns>
    List<CodeName> GetRole(bool isAll = false);

    /// <summary>
    /// 清除指定快取
    /// </summary>
    /// <param name="key">快取鍵值</param>
    void RemoveCache(string key);
}