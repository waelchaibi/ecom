namespace EcommerceAPI.DTOs;

public class OrderGiftDTO
{
    public int GiftId { get; set; }
    public string GiftName { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public int GiftRuleId { get; set; }
}
