using EcommerceAPI.DTOs;

namespace EcommerceAPI.Services;

public interface IAdminOrderService
{
    Task<PagedResultDTO<AdminOrderListItemDTO>> ListOrdersAsync(AdminOrderFilterDTO filter);
}
