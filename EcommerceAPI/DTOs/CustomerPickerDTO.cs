namespace EcommerceAPI.DTOs;

/// <summary>Minimal customer info for anonymous storefront checkout (no PII beyond display name).</summary>
public class CustomerPickerDTO
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
}
