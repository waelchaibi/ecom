namespace EcommerceAPI.DTOs;

public class CartDTO
{
    public List<CartLineDTO> Items { get; set; } = new();
    public decimal Subtotal { get; set; }
    public decimal EstimatedTax { get; set; }
    public decimal EstimatedShipping { get; set; }
    public decimal EstimatedTotal { get; set; }
}

public class CartLineDTO
{
    public int ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public string? ImageUrl { get; set; }
    public decimal UnitPrice { get; set; }
    public int Quantity { get; set; }
    public decimal LineTotal { get; set; }
    public int StockQuantity { get; set; }
}

public class CartItemUpsertDTO
{
    public int ProductId { get; set; }
    public int Quantity { get; set; }
}

public class CartCheckoutDTO
{
    public string? PromotionCode { get; set; }
}
