namespace EcommerceAPI.GiftRules;

/// <summary>
/// Evaluates persisted rules and returns applicable gift applications (deduplicated by gift, ordered by rule priority).
/// </summary>
public interface IGiftAssignmentService
{
    Task<IReadOnlyList<GiftRuleApplication>> EvaluateAsync(
        GiftRuleEvaluationContext context,
        CancellationToken cancellationToken = default);
}

public sealed record GiftRuleApplication(int GiftRuleId, int GiftId);
