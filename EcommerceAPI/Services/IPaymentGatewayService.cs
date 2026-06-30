using EcommerceAPI.DTOs;

namespace EcommerceAPI.Services;

public interface IPaymentGatewayService
{
    Task<SimulatePaymentResultDTO> ProcessCardPaymentAsync(
        SimulatePaymentDTO request,
        decimal amount,
        int orderId,
        CancellationToken cancellationToken = default);
}
