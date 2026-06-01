namespace EcommerceAPI.Configuration;

public sealed class AdminSecurityOptions
{
    public const string SectionName = "AdminSecurity";

    /// <summary>When non-empty, only these IPs may call /api/admin/*.</summary>
    public List<string> AllowedIps { get; set; } = new();
}
