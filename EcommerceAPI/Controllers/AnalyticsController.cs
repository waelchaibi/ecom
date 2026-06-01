using EcommerceAPI.DTOs;
using EcommerceAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EcommerceAPI.Controllers;

[ApiController]
[Route("api/analytics")]
[Authorize(Roles = "Admin")]
public class AnalyticsController : ControllerBase
{
    private readonly IAnalyticsService _analytics;

    public AnalyticsController(IAnalyticsService analytics)
    {
        _analytics = analytics;
    }

    [HttpGet("dashboard")]
    [ProducesResponseType(typeof(AnalyticsDashboardDTO), StatusCodes.Status200OK)]
    public async Task<ActionResult<AnalyticsDashboardDTO>> Dashboard([FromQuery] int lowStockThreshold = 10)
    {
        return Ok(await _analytics.GetDashboardAsync(lowStockThreshold, HttpContext.RequestAborted));
    }

    [HttpGet("export/csv")]
    [ProducesResponseType(typeof(FileContentResult), StatusCodes.Status200OK)]
    public async Task<IActionResult> ExportCsv([FromQuery] int lowStockThreshold = 10)
    {
        var bytes = await _analytics.ExportDashboardCsvAsync(lowStockThreshold, HttpContext.RequestAborted);
        return File(bytes, "text/csv; charset=utf-8", "analytics-dashboard.csv");
    }
}
