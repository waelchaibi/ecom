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
        int priorCompletedOrderCount,
        string? promotionCode)
    {
        Order = order;
        Customer = customer;
        PriorCompletedOrderCount = priorCompletedOrderCount;
        PromotionCode = promotionCode;
    }

    public Order Order { get; }
    public Customer Customer { get; }
    public int PriorCompletedOrderCount { get; }
    public string? PromotionCode { get; }
}
