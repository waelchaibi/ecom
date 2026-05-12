namespace EcommerceAPI.Models;

public class GiftRule
{
    public int Id { get; set; }
    public GiftRuleType RuleType { get; set; }
    /// <summary>
    /// Amount: decimal threshold; rule applies when order total is strictly greater than this value (e.g. "100").
    /// Loyalty: integer threshold; rule applies when the customer's prior completed order count is strictly greater than this value (e.g. "2" means third order onward).
    /// Promotion: promo code (case-insensitive), or "*" to match any non-empty promotion code on the order request.
    /// </summary>
    public string ConditionValue { get; set; } = string.Empty;
    public int GiftId { get; set; }
    public bool IsActive { get; set; } = true;
    /// <summary>Higher values are evaluated first when multiple rules could apply.</summary>
    public int Priority { get; set; }

    public Gift? Gift { get; set; }
}
