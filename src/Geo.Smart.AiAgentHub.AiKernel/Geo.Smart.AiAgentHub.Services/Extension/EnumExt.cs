using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace Geo.Smart.AiAgentHub.Services.Extension;
/// <summary>
/// 列舉擴展方法
/// </summary>
public static class EnumExt
{
    /// <summary>
    /// 取得可空列舉成員的顯示名稱
    /// </summary>
    /// <typeparam name="T">列舉型別</typeparam>
    /// <param name="input">列舉成員，允許為 null</param>
    /// <returns>顯示名稱或空字串</returns>
    public static string GetEnumDesc<T>(this T? input) where T : struct, Enum
    {
        if (input == null)
        {
            return string.Empty;
        }

        return GetEnumDesc(input.Value);
    }

    /// <summary>
    /// 取得列舉成員的顯示名稱
    /// </summary>
    /// <typeparam name="T">列舉型別</typeparam>
    /// <param name="input">列舉成員</param>
    /// <returns>顯示名稱或空字串</returns>
    public static string GetEnumDesc<T>(this T input) where T : Enum
    {
        if (!Enum.IsDefined(typeof(T), input))
        {
            return string.Empty;
        }

        var memberInfo = typeof(T).GetMembers(BindingFlags.Static | BindingFlags.Public)
            .First(r => r.Name.Equals(input.ToString()));

        var displayAttr = memberInfo.GetCustomAttributes(typeof(DisplayAttribute), false).FirstOrDefault() as DisplayAttribute;
        return displayAttr?.Name ?? input.ToString();
    }
}