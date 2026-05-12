using System.Globalization;
using EcommerceAPI.Models;

namespace EcommerceAPI.GiftRules;

public sealed class GiftRuleFactory : IGiftRuleFactory
{
    public IGiftRule? TryCreate(GiftRule rule)
    {
        if (rule.Gift is null)
            return null;

        switch (rule.RuleType)
        {
            case GiftRuleType.Amount:
                if (!decimal.TryParse(
                        rule.ConditionValue.Trim(),
                        NumberStyles.Number,
                        CultureInfo.InvariantCulture,
                        out var minTotal))
                    return null;
                return new AmountBasedGiftRule(rule, minTotal);

            case GiftRuleType.Loyalty:
                if (!int.TryParse(rule.ConditionValue.Trim(), NumberStyles.Integer, CultureInfo.InvariantCulture, out var minPrior))
                    return null;
                return new LoyaltyGiftRule(rule, minPrior);

            case GiftRuleType.Promotion:
                if (string.IsNullOrWhiteSpace(rule.ConditionValue))
                    return null;
                return new PromotionGiftRule(rule, rule.ConditionValue.Trim());

            default:
                return null;
        }
    }
}
