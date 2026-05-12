using EcommerceAPI.Models;

namespace EcommerceAPI.GiftRules;

/// <summary>
/// Strategy for a single persisted <see cref="GiftRule"/> row. Implementations are created by <see cref="IGiftRuleFactory"/>.
/// </summary>
public interface IGiftRule
{
    GiftRule SourceRule { get; }

    bool IsApplicable(GiftRuleEvaluationContext context);

    /// <summary>Returns the gift granted by this rule when <see cref="IsApplicable"/> is true.</summary>
    Gift Apply();
}
