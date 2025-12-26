using Geo.Smart.AiAgentHub.Services.Common;
using Geo.Smart.AiAgentHub.Services.Contracts;
using Geo.Smart.AiAgentHub.Services.Helpers;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using NetTopologySuite.Geometries;

namespace Geo.Smart.AiAgentHub.Services;

/// <summary>
/// 共通性服務
/// </summary>
public class CommonService(GdbContext dbModel,
    ILogger<CommonService> _logger,
    IMemoryCache _memoryCache
    ) : BaseService(dbModel, _logger), ICommonService
{
    /// <summary>
    /// 列舉資料
    /// </summary>
    /// <returns></returns>
    public Dictionary<string, List<KeyName>> GetAllMapping()
    {
        var data = new Dictionary<string, List<KeyName>>
        {
            [nameof(UserHistoryType)] = EnumeratorHelper.GetEnumDescriptions<UserHistoryType>(),
            [nameof(LlmSourceType)] = EnumeratorHelper.GetEnumDescriptions<LlmSourceType>(),
            [nameof(McpServerType)] = EnumeratorHelper.GetEnumDescriptions<McpServerType>(),
            [nameof(OgcGeometryType)] = EnumeratorHelper.GetEnumDescriptions<OgcGeometryType>(),
        };
        return data;
    }

    /// <summary>
    /// 取得組織 dropdown 清單
    /// </summary>
    /// <returns></returns>
    public List<CodeName> GetOrgs()
    {
        return DbModel.Organizations.Where(x => x.IsEnabled)
            .Select(x => new CodeName
            {
                Code = x.OrgId.ToString(),
                Name = x.Name,
            }).ToList();
    }

    /// <summary>
    /// 取得角色選單
    /// </summary>
    /// <param name="isAll">true:全撈 false:不撈管理者</param>
    /// <returns></returns>
    public List<CodeName> GetRole(bool isAll = false)
    {
        var result = DbModel.Roles.Where(x => x.Name != "系統管理員").OrderBy(x => x.Name)
            .Select(x => new CodeName
            {
                Code = x.Id,
                Name = x.Name,
            }).ToList();
        if (isAll)
        {
            var manageUser = DbModel.Roles.FirstOrDefault(x => x.Name == "系統管理員");
            if (manageUser != null)
            {
                result.Add(new CodeName()
                {
                    Code = manageUser.Id,
                    Name = manageUser.Name
                });
            }
        }
        return result;
    }

    /// <summary>
    /// 清除快取
    /// </summary>
    /// <param name="key">cache key</param>
    public void RemoveCache(string key)
    {
        _memoryCache.Remove(key);
    }
}