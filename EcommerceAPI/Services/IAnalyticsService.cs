using EcommerceAPI.DTOs;

namespace EcommerceAPI.Services;

public interface IAnalyticsService
{
    Task<AnalyticsDashboardDTO> GetDashboardAsync(int lowStockThreshold = 10, CancellationToken cancellationToken = default);
    Task<byte[]> ExportDashboardCsvAsync(int lowStockThreshold = 10, CancellationToken cancellationToken = default);
    Task<byte[]> ExportDashboardExcelAsync(int lowStockThreshold = 10, CancellationToken cancellationToken = default);
}
