using EcommerceAPI.Data;
using EcommerceAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace EcommerceAPI.GiftRules;

public sealed class GiftAssignmentService : IGiftAssignmentService
{
    private readonly AppDbContext _context;
    private readonly IGiftRuleFactory _ruleFactory;
    private readonly ILogger<GiftAssignmentService> _logger;

    public GiftAssignmentService(
        AppDbContext context,
        IGiftRuleFactory ruleFactory,
        ILogger<GiftAssignmentService> logger)
    {
        _context = context;
        _ruleFactory = ruleFactory;
        _logger = logger;
    }

    public async Task<IReadOnlyList<GiftRuleApplication>> EvaluateAsync(
        GiftRuleEvaluationContext context,
        CancellationToken cancellationToken = default)
    {
        var rules = await _context.GiftRules
            .AsNoTracking()
            .Include(r => r.Gift)
            .Where(r => r.IsActive)
            .OrderByDescending(r => r.Priority)
            .ThenByDescending(r => r.Id)
            .ToListAsync(cancellationToken);

        var result = new List<GiftRuleApplication>();
        var seenGiftIds = new HashSet<int>();

        foreach (var entity in rules)
        {
            if (entity.Gift is null || entity.Gift.StockQuantity <= 0)
                continue;

            var runtime = _ruleFactory.TryCreate(entity);
            if (runtime is null)
            {
                _logger.LogWarning(
                    "Gift rule {RuleId} could not be built from persisted data (invalid condition or missing gift).",
                    entity.Id);
                continue;
            }

            if (!runtime.IsApplicable(context))
                continue;

            var gift = runtime.Apply();
            if (!seenGiftIds.Add(gift.Id))
                continue;

            result.Add(new GiftRuleApplication(entity.Id, gift.Id));
        }

        return result;
    }
}
