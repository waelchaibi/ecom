using EcommerceAPI.DTOs;

namespace EcommerceAPI.Services;

public interface ICartService
{
    Task<CartDTO> GetCartAsync(int customerId);
    Task<CartDTO> UpsertItemAsync(int customerId, CartItemUpsertDTO dto);
    Task<CartDTO> RemoveItemAsync(int customerId, int productId);
    Task ClearCartAsync(int customerId);
    Task<OrderDTO> CheckoutAsync(int customerId, CartCheckoutDTO dto);
}
