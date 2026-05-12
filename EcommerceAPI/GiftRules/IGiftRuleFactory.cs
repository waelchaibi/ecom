using EcommerceAPI.Models;

namespace EcommerceAPI.GiftRules;

public interface IGiftRuleFactory
{
    /// <summary>Builds a runtime rule strategy, or null if the entity is invalid for evaluation.</summary>
    IGiftRule? TryCreate(GiftRule rule);
}
