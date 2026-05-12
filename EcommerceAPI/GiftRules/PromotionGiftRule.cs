using EcommerceAPI.Models;

namespace EcommerceAPI.GiftRules;

public sealed class PromotionGiftRule : IGiftRule
{
    private readonly GiftRule _rule;
    private readonly string _expectedCode;

    public PromotionGiftRule(GiftRule rule, string expectedCode)
    {
        _rule = rule;
        _expectedCode = expectedCode;
    }

    public GiftRule SourceRule => _rule;

    public bool IsApplicable(GiftRuleEvaluationContext context)
    {
        var incoming = context.PromotionCode?.Trim();
        if (string.IsNullOrEmpty(incoming))
            return false;

        if (_expectedCode == "*")
            return true;

        return string.Equals(incoming, _expectedCode, StringComparison.OrdinalIgnoreCase);
    }

    public Gift Apply() => _rule.Gift!;
}
