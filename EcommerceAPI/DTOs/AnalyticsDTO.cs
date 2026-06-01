namespace EcommerceAPI.DTOs;

public class SalesSummaryDTO
{
    public decimal TotalRevenue { get; set; }
    public int OrderCount { get; set; }
    public decimal AverageOrderValue { get; set; }
}

public class BestSellingProductDTO
{
    public int ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public int QuantitySold { get; set; }
}

public class LowStockProductDTO
{
    public int ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public int StockQuantity { get; set; }
}

public class LoyalCustomerDTO
{
    public int CustomerId { get; set; }
    public string CustomerName { get; set; } = string.Empty;
    public int OrderCount { get; set; }
}

public class HighValueCustomerDTO
{
    public int CustomerId { get; set; }
    public string CustomerName { get; set; } = string.Empty;
    public decimal TotalPurchaseValue { get; set; }
}

public class GiftAssignmentStatDTO
{
    public int GiftId { get; set; }
    public string GiftName { get; set; } = string.Empty;
    public int TimesAssigned { get; set; }
}

public class AnalyticsDashboardDTO
{
    public SalesSummaryDTO Sales { get; set; } = new();
    public IReadOnlyList<BestSellingProductDTO> BestSellingProducts { get; set; } = Array.Empty<BestSellingProductDTO>();
    public IReadOnlyList<LowStockProductDTO> LowStockProducts { get; set; } = Array.Empty<LowStockProductDTO>();
    public IReadOnlyList<LoyalCustomerDTO> TopCustomersByOrders { get; set; } = Array.Empty<LoyalCustomerDTO>();
    public IReadOnlyList<HighValueCustomerDTO> TopCustomersByRevenue { get; set; } = Array.Empty<HighValueCustomerDTO>();
    public IReadOnlyList<GiftAssignmentStatDTO> MostAssignedGifts { get; set; } = Array.Empty<GiftAssignmentStatDTO>();
}
