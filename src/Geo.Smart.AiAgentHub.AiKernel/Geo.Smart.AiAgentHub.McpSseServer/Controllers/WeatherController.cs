using Microsoft.AspNetCore.Mvc;

namespace Geo.Smart.AiAgentHub.McpSseServer.Controllers;

/// <summary>
/// 提供天氣預報相關 API 控制器
/// </summary>
[ApiController]
[Route("[controller]")]
public class WeatherController : ControllerBase
{
    /// <summary>
    /// 取得天氣預報執行狀態
    /// </summary>
    /// <returns>回傳天氣預報執行狀態字串</returns>
    [HttpGet(Name = "GetWeatherForecast")]
    public ActionResult Get()
    {
        return Ok("Weather Forecast is running.");
    }
}