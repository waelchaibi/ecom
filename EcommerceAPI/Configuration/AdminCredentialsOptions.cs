namespace EcommerceAPI.Configuration;

public sealed class AdminCredentialsOptions
{
    public const string SectionName = "Admin";

    public string Username { get; set; } = "admin";
    public string Password { get; set; } = string.Empty;
}
