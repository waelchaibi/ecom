using EcommerceAPI.DTOs;

namespace EcommerceAPI.Services;

public interface IOrderService
{
    Task<OrderDTO?> GetOrderByIdAsync(int id);
    Task<OrderDTO> CreateOrderAsync(CreateOrderDTO dto);
}