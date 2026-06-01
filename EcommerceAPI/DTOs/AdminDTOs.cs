namespace EcommerceAPI.DTOs;

public class UpdateProductDTO
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public int StockQuantity { get; set; }
    public int? CategoryId { get; set; }
    public string? ImageUrl { get; set; }
}

public class UpdateStockDTO
{
    public int StockQuantity { get; set; }
}

public class AdminOrderListItemDTO
{
    public int Id { get; set; }
    public int CustomerId { get; set; }
    public string CustomerName { get; set; } = string.Empty;
    public decimal TotalAmount { get; set; }
    public DateTime CreatedAt { get; set; }
    public string Status { get; set; } = string.Empty;
    public int LineItemCount { get; set; }
}

public class PagedResultDTO<T>
{
    public int Page { get; set; }
    public int PageSize { get; set; }
    public int TotalCount { get; set; }
    public IReadOnlyList<T> Items { get; set; } = Array.Empty<T>();
}
