using EcommerceAPI.DTOs;
using EcommerceAPI.Models;
using EcommerceAPI.Repositories;

namespace EcommerceAPI.Services;

public class GiftRuleService : IGiftRuleService
{
    private readonly IGiftRuleRepository _ruleRepository;
    private readonly IGiftRepository _giftRepository;

    public GiftRuleService(IGiftRuleRepository ruleRepository, IGiftRepository giftRepository)
    {
        _ruleRepository = ruleRepository;
        _giftRepository = giftRepository;
    }

    public async Task<List<GiftRuleDTO>> GetAllAsync()
    {
        var rules = await _ruleRepository.GetAllWithGiftAsync();
        return rules.Select(Map).ToList();
    }

    public async Task<GiftRuleDTO> CreateAsync(CreateGiftRuleDTO dto)
    {
        if (string.IsNullOrWhiteSpace(dto.ConditionValue))
            throw new ArgumentException("Condition value is required");

        if (dto.GiftId <= 0)
            throw new ArgumentException("Valid gift ID is required");

        var gift = await _giftRepository.GetByIdAsync(dto.GiftId);
        if (gift is null)
            throw new ArgumentException($"Gift with ID {dto.GiftId} not found");

        if (dto.IsActive)
            await EnsureNoConflictingActiveRuleAsync(dto.RuleType, dto.ConditionValue.Trim(), dto.GiftId, excludeRuleId: null);

        var entity = new GiftRule
        {
            RuleType = dto.RuleType,
            ConditionValue = dto.ConditionValue.Trim(),
            GiftId = dto.GiftId,
            IsActive = dto.IsActive,
            Priority = dto.Priority
        };

        var created = await _ruleRepository.AddAsync(entity);
        var reloaded = await _ruleRepository.GetByIdWithGiftAsync(created.Id);
        return Map(reloaded!);
    }

    public async Task<GiftRuleDTO> UpdateAsync(int id, UpdateGiftRuleDTO dto)
    {
        if (id <= 0)
            throw new ArgumentException("Rule ID must be greater than 0");

        var entity = await _ruleRepository.GetByIdWithGiftAsync(id);
        if (entity is null)
            throw new ArgumentException($"Gift rule with ID {id} not found");

        if (dto.RuleType.HasValue)
            entity.RuleType = dto.RuleType.Value;

        if (dto.ConditionValue is not null)
        {
            if (string.IsNullOrWhiteSpace(dto.ConditionValue))
                throw new ArgumentException("Condition value cannot be empty");
            entity.ConditionValue = dto.ConditionValue.Trim();
        }

        if (dto.GiftId.HasValue)
        {
            if (dto.GiftId.Value <= 0)
                throw new ArgumentException("Valid gift ID is required");

            var gift = await _giftRepository.GetByIdAsync(dto.GiftId.Value);
            if (gift is null)
                throw new ArgumentException($"Gift with ID {dto.GiftId} not found");

            entity.GiftId = dto.GiftId.Value;
        }

        if (dto.IsActive.HasValue)
            entity.IsActive = dto.IsActive.Value;

        if (dto.Priority.HasValue)
            entity.Priority = dto.Priority.Value;

        if (entity.IsActive)
            await EnsureNoConflictingActiveRuleAsync(entity.RuleType, entity.ConditionValue, entity.GiftId, excludeRuleId: id);

        await _ruleRepository.SaveChangesAsync();

        var reloaded = await _ruleRepository.GetByIdWithGiftAsync(id);
        return Map(reloaded!);
    }

    public async Task<GiftRuleDTO> SetActiveAsync(int id, bool isActive)
    {
        if (id <= 0)
            throw new ArgumentException("Rule ID must be greater than 0");

        var entity = await _ruleRepository.GetByIdWithGiftAsync(id);
        if (entity is null)
            throw new ArgumentException($"Gift rule with ID {id} not found");

        entity.IsActive = isActive;

        if (isActive)
            await EnsureNoConflictingActiveRuleAsync(entity.RuleType, entity.ConditionValue, entity.GiftId, excludeRuleId: id);

        await _ruleRepository.SaveChangesAsync();
        var reloaded = await _ruleRepository.GetByIdWithGiftAsync(id);
        return Map(reloaded!);
    }

    public async Task DeleteAsync(int id)
    {
        if (id <= 0)
            throw new ArgumentException("Rule ID must be greater than 0");

        var entity = await _ruleRepository.GetByIdWithGiftAsync(id);
        if (entity is null)
            throw new ArgumentException($"Gift rule with ID {id} not found");

        if (await _ruleRepository.HasOrderGiftsAsync(id))
            throw new InvalidOperationException("Cannot delete a gift rule that has already been applied to orders.");

        await _ruleRepository.DeleteAsync(entity);
    }

    private async Task EnsureNoConflictingActiveRuleAsync(
        GiftRuleType ruleType,
        string conditionValue,
        int giftId,
        int? excludeRuleId)
    {
        if (await _ruleRepository.HasConflictingActiveRuleAsync(ruleType, conditionValue, giftId, excludeRuleId))
            throw new InvalidOperationException(
                "Another active rule already uses the same type, condition, and gift. Deactivate or change it first.");
    }

    private static GiftRuleDTO Map(GiftRule r) => new()
    {
        Id = r.Id,
        RuleType = r.RuleType,
        ConditionValue = r.ConditionValue,
        GiftId = r.GiftId,
        IsActive = r.IsActive,
        Priority = r.Priority,
        Gift = r.Gift is null
            ? null
            : new GiftDTO
            {
                Id = r.Gift.Id,
                Name = r.Gift.Name,
                Description = r.Gift.Description,
                StockQuantity = r.Gift.StockQuantity
            }
    };
}
