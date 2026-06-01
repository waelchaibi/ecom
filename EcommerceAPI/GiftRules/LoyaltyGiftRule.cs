using EcommerceAPI.Models;

namespace EcommerceAPI.GiftRules;

public sealed class LoyaltyGiftRule : IGiftRule
{
    private readonly GiftRule _rule;
    private readonly int _minimumPriorOrders;

    public LoyaltyGiftRule(GiftRule rule, int minimumPriorOrders)
    {
        _rule = rule;
        _minimumPriorOrders = minimumPriorOrders;
    }

    public GiftRule SourceRule => _rule;

    /// <summary>Applies when the customer has strictly more than the configured number of prior qualifying orders (see <see cref="OrderStatusHelper.CountsTowardLoyaltyHistory"/>).</summary>
    public bool IsApplicable(GiftRuleEvaluationContext context) =>
        context.PriorQualifyingOrderCount > _minimumPriorOrders;

    public Gift Apply() => _rule.Gift!;
}
