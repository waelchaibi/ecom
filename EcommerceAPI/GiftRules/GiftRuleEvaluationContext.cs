using EcommerceAPI.Models;

namespace EcommerceAPI.GiftRules;

/// <summary>
/// Context passed to gift rules during order placement (in-memory order before persistence is fine).
/// </summary>
public sealed class GiftRuleEvaluationContext
{
    public GiftRuleEvaluationContext(
        Order order,
        Customer customer,
        int priorQualifyingOrderCount,
        string? promotionCode)
    {
        Order = order;
        Customer = customer;
        PriorQualifyingOrderCount = priorQualifyingOrderCount;
        PromotionCode = promotionCode;
    }

    public Order Order { get; }
    public Customer Customer { get; }
    /// <summary>Number of prior orders that count toward loyalty (confirmed/shipped), excluding cancelled/pending.</summary>
    public int PriorQualifyingOrderCount { get; }
    public string? PromotionCode { get; }
}
