using System.Globalization;

namespace Geo.Smart.AiAgentHub.Services.Helpers;
/// <summary>
/// 時間處理
/// </summary>
public static class DateHelper
{
    /// <summary>
    /// 轉民國年Datetime格式
    /// </summary>
    /// <param name="oriDate">字串，如:"20210416"</param>
    /// <returns></returns>
    public static DateTime? CovertStringDate(string oriDate)
    {
        if (!string.IsNullOrEmpty(oriDate) && oriDate.Length == 8)
        {
            DateTime date = DateTime.ParseExact(oriDate, "yyyyMMdd", CultureInfo.InvariantCulture);

            // 建立新的日期物件
            return new DateTime(date.Year, date.Month, date.Day, 0, 0, 0, DateTimeKind.Local);
        }
        else
        {
            return null;
        }
    }

    /// <summary>
    /// 轉民國年109-10-20格式
    /// </summary>
    /// <param name="oriDate">字串，如:"20210416"</param>
    /// <returns></returns>
    public static string? CovertStringDateTw(string oriDate)
    {
        if (!string.IsNullOrEmpty(oriDate) && oriDate.Length == 8)
        {
            DateTime date = DateTime.ParseExact(oriDate, "yyyyMMdd", CultureInfo.InvariantCulture);

            return CovertDateTw(date, 2);
        }
        else if (!string.IsNullOrEmpty(oriDate) && oriDate.Length == 10)
        {
            DateTime date = DateTime.ParseExact(oriDate, "yyyy-MM-dd", CultureInfo.InvariantCulture);

            return CovertDateTw(date, 2);
        }
        else
        {
            return null;
        }
    }

    /// <summary>
    /// 轉民國日期格式
    /// </summary>
    /// <param name="oriDate"></param>
    /// <param name="formatType"></param>
    /// <returns>
    /// formatType:1 => 1091020
    /// formatType:2 => 109-10-20
    /// formatType:3 => 109年10月20日
    /// formatType:4 => 109年10月20日16時04分46秒
    /// formatType:5 => 109-10-20 16:04:46
    /// formatType:6 => 109.10.20
    /// </returns>
    public static string CovertDateTw(DateTime? oriDate, int formatType = 1)
    {
        if (oriDate.HasValue)
        {
            var year = oriDate.Value.Year - 1911;
            var month = oriDate.Value.Month.ToString("00");
            var day = oriDate.Value.Day.ToString("00");
            var hour = oriDate.Value.Hour.ToString("00");
            var min = oriDate.Value.Minute.ToString("00");
            var sec = oriDate.Value.Second.ToString("00");
            return formatType switch
            {
                1 => $"{year}{month}{day}",
                2 => $"{year}-{month}-{day}",
                3 => $"{year}年{month}月{day}日",
                4 => $"{year}年{month}月{day}日{hour}時{min}分{sec}秒",
                5 => $"{year}-{month}-{day} {hour}:{min}:{sec}",
                6 => $"{year}.{month}.{day}",
                _ => $"{year}{month}{day}",
            };
        }
        else
        {
            return string.Empty;
        }
    }
}