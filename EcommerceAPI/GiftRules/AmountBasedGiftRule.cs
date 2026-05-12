using System.Globalization;
using EcommerceAPI.Models;

namespace EcommerceAPI.GiftRules;

public sealed class AmountBasedGiftRule : IGiftRule
{
    private readonly GiftRule _rule;
    private readonly decimal _minimumTotal;

    public AmountBasedGiftRule(GiftRule rule, decimal minimumTotal)
    {
        _rule = rule;
        _minimumTotal = minimumTotal;
    }

    public GiftRule SourceRule => _rule;

    public bool IsApplicable(GiftRuleEvaluationContext context) =>
        context.Order.TotalAmount > _minimumTotal;

    public Gift Apply() => _rule.Gift!;
}
