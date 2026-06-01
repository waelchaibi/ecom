namespace EcommerceAPI.Services;

public interface IJwtTokenService
{
    string CreateAdminToken(string username);
    string CreateCustomerToken(int customerId, string email, string name);
}
