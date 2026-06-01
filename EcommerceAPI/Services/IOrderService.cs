using EcommerceAPI.DTOs;

namespace EcommerceAPI.Services;

public interface IOrderService
{
    Task<OrderDTO?> GetOrderByIdAsync(int id);
    Task<OrderDTO?> GetOrderForCustomerAsync(int customerId, int orderId);
    Task<IReadOnlyList<OrderDTO>> GetOrdersForCustomerAsync(int customerId);
    Task<OrderDTO> CreateOrderForCustomerAsync(int customerId, CustomerCreateOrderDTO dto);
    Task<OrderDTO> ConfirmPaymentAsync(int orderId);
    Task<OrderDTO> CancelOrderAsync(int orderId);
    /// <summary>Emulated payment: Pending → Confirmed for the owning customer only.</summary>
    Task<OrderDTO> PayOrderAsCustomerAsync(int customerId, int orderId);
    /// <summary>Customer may cancel only their own Pending orders.</summary>
    Task<OrderDTO> CancelOrderAsCustomerAsync(int customerId, int orderId);
}