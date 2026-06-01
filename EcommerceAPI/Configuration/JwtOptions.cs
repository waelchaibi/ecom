namespace EcommerceAPI.Configuration;

public sealed class JwtOptions
{
    public const string SectionName = "Jwt";

    public string Secret { get; set; } = string.Empty;
    public string Issuer { get; set; } = "EcommerceAPI";
    public string Audience { get; set; } = "EcommerceAdmin";
    public int ExpiresMinutes { get; set; } = 480;
}
