using ModelContextProtocol.Server;
using System.ComponentModel;
using System.Net.Http.Headers;
using System.Text.Json;

namespace Geo.Smart.AiAgentHub.McpSseServer.Tools;

/// <summary>
/// 天氣工具類別，提供取得台灣縣市天氣預報的功能
/// </summary>
[McpServerToolType]
public class WeatherTool
{
    /// <summary>
    /// 取得指定台灣縣市的天氣預報
    /// </summary>
    /// <param name="city">縣市名稱，請以中文輸入</param>
    /// <returns>天氣預報的 JSON 字串</returns>
    [McpServerTool, Description("取得台灣縣市的天氣預報")]
    public async Task<string> Forecast(string city)
    {
        city = city.Replace("台", "臺");
#pragma warning disable S1075 // URIs should not be hardcoded
        var client = new HttpClient()
        {
            BaseAddress = new Uri("https://opendata.cwa.gov.tw")
        };
#pragma warning restore S1075 // URIs should not be hardcoded
        client.DefaultRequestHeaders.UserAgent
            .Add(new ProductInfoHeaderValue("cwa-weather-tool", "1.0"));

        var jsonElement = await client.GetFromJsonAsync<JsonElement>(
            $"/api/v1/rest/datastore/F-C0032-001?Authorization=CWB-BC87FA16-C19A-46AB-8D7E-D36BFDC08439&locationName={city}"
        );
        var forecast = jsonElement.GetProperty("records");

        return forecast.ToString();
    }
}