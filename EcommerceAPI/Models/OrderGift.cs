namespace EcommerceAPI.Models;

public class OrderGift
{
    public int Id { get; set; }
    public int OrderId { get; set; }
    public int GiftId { get; set; }
    public int GiftRuleId { get; set; }
    public int Quantity { get; set; } = 1;

    public Order? Order { get; set; }
    public Gift? Gift { get; set; }
    public GiftRule? GiftRule { get; set; }
}
