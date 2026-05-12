namespace EcommerceAPI.Models;

public class Gift
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int StockQuantity { get; set; }

    public ICollection<GiftRule> GiftRules { get; set; } = new List<GiftRule>();
    public ICollection<OrderGift> OrderGifts { get; set; } = new List<OrderGift>();
}
