namespace EcommerceAPI.Models;

public class Customer
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    /// <summary>BCrypt hash; null for legacy seeded rows until they register.</summary>
    public string? PasswordHash { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Navigation property
    public ICollection<Order> Orders { get; set; } = new List<Order>();
}