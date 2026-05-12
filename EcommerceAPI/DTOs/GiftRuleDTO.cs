using EcommerceAPI.Models;

namespace EcommerceAPI.DTOs;

public class GiftRuleDTO
{
    public int Id { get; set; }
    public GiftRuleType RuleType { get; set; }
    public string ConditionValue { get; set; } = string.Empty;
    public int GiftId { get; set; }
    public bool IsActive { get; set; }
    public int Priority { get; set; }
    public GiftDTO? Gift { get; set; }
}

public class CreateGiftRuleDTO
{
    public GiftRuleType RuleType { get; set; }
    public string ConditionValue { get; set; } = string.Empty;
    public int GiftId { get; set; }
    public bool IsActive { get; set; } = true;
    public int Priority { get; set; }
}

public class UpdateGiftRuleDTO
{
    public GiftRuleType? RuleType { get; set; }
    public string? ConditionValue { get; set; }
    public int? GiftId { get; set; }
    public bool? IsActive { get; set; }
    public int? Priority { get; set; }
}
