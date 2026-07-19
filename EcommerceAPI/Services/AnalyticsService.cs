using ClosedXML.Excel;
using EcommerceAPI.Data;
using EcommerceAPI.DTOs;
using EcommerceAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace EcommerceAPI.Services;

public sealed class AnalyticsService : IAnalyticsService
{
    private readonly AppDbContext _db;

    public AnalyticsService(AppDbContext db)
    {
        _db = db;
    }

    public async Task<AnalyticsDashboardDTO> GetDashboardAsync(int lowStockThreshold = 10, CancellationToken cancellationToken = default)
    {
        var revenueOrders = _db.Orders.AsNoTracking()
            .Where(o => o.Status != OrderStatuses.Cancelled && o.Status != OrderStatuses.Pending);

        var totalRevenue = await revenueOrders.SumAsync(o => (decimal?)o.TotalAmount, cancellationToken) ?? 0m;
        var orderCount = await revenueOrders.CountAsync(cancellationToken);
        var aov = orderCount == 0 ? 0m : Math.Round(totalRevenue / orderCount, 2, MidpointRounding.AwayFromZero);

        var bestSelling = await (
                from oi in _db.OrderItems.AsNoTracking()
                join o in _db.Orders.AsNoTracking() on oi.OrderId equals o.Id
                where o.Status != OrderStatuses.Cancelled && o.Status != OrderStatuses.Pending
                join p in _db.Products.AsNoTracking() on oi.ProductId equals p.Id
                group oi by new { oi.ProductId, p.Name }
                into g
                select new BestSellingProductDTO
                {
                    ProductId = g.Key.ProductId,
                    ProductName = g.Key.Name,
                    QuantitySold = g.Sum(x => x.Quantity)
                })
            .OrderByDescending(x => x.QuantitySold)
            .Take(5)
            .ToListAsync(cancellationToken);

        var lowStock = await _db.Products.AsNoTracking()
            .Where(p => p.StockQuantity <= lowStockThreshold)
            .OrderBy(p => p.StockQuantity)
            .Take(20)
            .Select(p => new LowStockProductDTO
            {
                ProductId = p.Id,
                ProductName = p.Name,
                StockQuantity = p.StockQuantity
            })
            .ToListAsync(cancellationToken);

        var topByOrders = await _db.Orders.AsNoTracking()
            .Where(o => o.Status != OrderStatuses.Cancelled && o.Status != OrderStatuses.Pending)
            .GroupBy(o => new { o.CustomerId, o.Customer!.Name })
            .Select(g => new LoyalCustomerDTO
            {
                CustomerId = g.Key.CustomerId,
                CustomerName = g.Key.Name,
                OrderCount = g.Count()
            })
            .OrderByDescending(x => x.OrderCount)
            .Take(5)
            .ToListAsync(cancellationToken);

        var topByRevenue = await _db.Orders.AsNoTracking()
            .Where(o => o.Status != OrderStatuses.Cancelled && o.Status != OrderStatuses.Pending)
            .GroupBy(o => new { o.CustomerId, o.Customer!.Name })
            .Select(g => new HighValueCustomerDTO
            {
                CustomerId = g.Key.CustomerId,
                CustomerName = g.Key.Name,
                TotalPurchaseValue = g.Sum(x => x.TotalAmount)
            })
            .OrderByDescending(x => x.TotalPurchaseValue)
            .Take(5)
            .ToListAsync(cancellationToken);

        var giftStats = await (
                from og in _db.OrderGifts.AsNoTracking()
                join o in _db.Orders.AsNoTracking() on og.OrderId equals o.Id
                where o.Status != OrderStatuses.Cancelled && o.Status != OrderStatuses.Pending
                group og by new { og.GiftId, og.Gift!.Name }
                into g
                select new GiftAssignmentStatDTO
                {
                    GiftId = g.Key.GiftId,
                    GiftName = g.Key.Name,
                    TimesAssigned = g.Sum(x => x.Quantity)
                })
            .OrderByDescending(x => x.TimesAssigned)
            .Take(10)
            .ToListAsync(cancellationToken);

        return new AnalyticsDashboardDTO
        {
            Sales = new SalesSummaryDTO
            {
                TotalRevenue = totalRevenue,
                OrderCount = orderCount,
                AverageOrderValue = aov
            },
            BestSellingProducts = bestSelling,
            LowStockProducts = lowStock,
            TopCustomersByOrders = topByOrders,
            TopCustomersByRevenue = topByRevenue,
            MostAssignedGifts = giftStats
        };
    }

    public async Task<byte[]> ExportDashboardCsvAsync(int lowStockThreshold = 10, CancellationToken cancellationToken = default)
    {
        var d = await GetDashboardAsync(lowStockThreshold, cancellationToken);
        await using var ms = new MemoryStream();
        await using var w = new StreamWriter(ms, System.Text.Encoding.UTF8);

        await w.WriteLineAsync("metric,value");
        await w.WriteLineAsync($"totalRevenue,{d.Sales.TotalRevenue}");
        await w.WriteLineAsync($"orderCount,{d.Sales.OrderCount}");
        await w.WriteLineAsync($"averageOrderValue,{d.Sales.AverageOrderValue}");
        await w.WriteLineAsync("bestSellingProductId,productName,quantitySold");
        foreach (var p in d.BestSellingProducts)
            await w.WriteLineAsync($"{p.ProductId},\"{p.ProductName.Replace("\"", "\"\"")}\",{p.QuantitySold}");
        await w.WriteLineAsync("lowStockProductId,productName,stockQuantity");
        foreach (var p in d.LowStockProducts)
            await w.WriteLineAsync($"{p.ProductId},\"{p.ProductName.Replace("\"", "\"\"")}\",{p.StockQuantity}");
        await w.WriteLineAsync("customerId,customerName,orderCount");
        foreach (var c in d.TopCustomersByOrders)
            await w.WriteLineAsync($"{c.CustomerId},\"{c.CustomerName.Replace("\"", "\"\"")}\",{c.OrderCount}");
        await w.WriteLineAsync("customerId,customerName,totalPurchaseValue");
        foreach (var c in d.TopCustomersByRevenue)
            await w.WriteLineAsync($"{c.CustomerId},\"{c.CustomerName.Replace("\"", "\"\"")}\",{c.TotalPurchaseValue}");
        await w.WriteLineAsync("giftId,giftName,timesAssigned");
        foreach (var g in d.MostAssignedGifts)
            await w.WriteLineAsync($"{g.GiftId},\"{g.GiftName.Replace("\"", "\"\"")}\",{g.TimesAssigned}");

        await w.FlushAsync();
        return ms.ToArray();
    }

    public async Task<byte[]> ExportDashboardExcelAsync(int lowStockThreshold = 10, CancellationToken cancellationToken = default)
    {
        var d = await GetDashboardAsync(lowStockThreshold, cancellationToken);
        using var wb = new XLWorkbook();

        var metrics = wb.Worksheets.Add("Metrics");
        metrics.Cell(1, 1).Value = "metric";
        metrics.Cell(1, 2).Value = "value";
        metrics.Cell(2, 1).Value = "totalRevenue";
        metrics.Cell(2, 2).Value = d.Sales.TotalRevenue;
        metrics.Cell(3, 1).Value = "orderCount";
        metrics.Cell(3, 2).Value = d.Sales.OrderCount;
        metrics.Cell(4, 1).Value = "averageOrderValue";
        metrics.Cell(4, 2).Value = d.Sales.AverageOrderValue;

        WriteSheet(
            wb.Worksheets.Add("BestSelling"),
            new[] { "productId", "productName", "quantitySold" },
            d.BestSellingProducts.Select(p => new object[] { p.ProductId, p.ProductName, p.QuantitySold }));

        WriteSheet(
            wb.Worksheets.Add("LowStock"),
            new[] { "productId", "productName", "stockQuantity" },
            d.LowStockProducts.Select(p => new object[] { p.ProductId, p.ProductName, p.StockQuantity }));

        WriteSheet(
            wb.Worksheets.Add("TopByOrders"),
            new[] { "customerId", "customerName", "orderCount" },
            d.TopCustomersByOrders.Select(c => new object[] { c.CustomerId, c.CustomerName, c.OrderCount }));

        WriteSheet(
            wb.Worksheets.Add("TopByRevenue"),
            new[] { "customerId", "customerName", "totalPurchaseValue" },
            d.TopCustomersByRevenue.Select(c => new object[] { c.CustomerId, c.CustomerName, c.TotalPurchaseValue }));

        WriteSheet(
            wb.Worksheets.Add("GiftAssignments"),
            new[] { "giftId", "giftName", "timesAssigned" },
            d.MostAssignedGifts.Select(g => new object[] { g.GiftId, g.GiftName, g.TimesAssigned }));

        using var ms = new MemoryStream();
        wb.SaveAs(ms);
        return ms.ToArray();
    }

    private static void WriteSheet(IXLWorksheet sheet, string[] headers, IEnumerable<object[]> rows)
    {
        for (var c = 0; c < headers.Length; c++)
            sheet.Cell(1, c + 1).Value = headers[c];
        var r = 2;
        foreach (var row in rows)
        {
            for (var c = 0; c < row.Length; c++)
            {
                var cell = sheet.Cell(r, c + 1);
                switch (row[c])
                {
                    case int i:
                        cell.Value = i;
                        break;
                    case decimal m:
                        cell.Value = m;
                        break;
                    case double dbl:
                        cell.Value = dbl;
                        break;
                    case string s:
                        cell.Value = s;
                        break;
                    default:
                        cell.Value = row[c]?.ToString() ?? string.Empty;
                        break;
                }
            }
            r++;
        }
    }
}
