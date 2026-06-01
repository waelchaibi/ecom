using EcommerceAPI.DTOs;
using EcommerceAPI.Repositories;

namespace EcommerceAPI.Services;

public sealed class AdminOrderService : IAdminOrderService
{
    private readonly IOrderRepository _orderRepository;

    public AdminOrderService(IOrderRepository orderRepository) => _orderRepository = orderRepository;

    public async Task<PagedResultDTO<AdminOrderListItemDTO>> ListOrdersAsync(AdminOrderFilterDTO filter)
    {
        var (items, total) = await _orderRepository.GetPagedAsync(
            filter.Page,
            filter.PageSize,
            filter.Status,
            filter.CustomerId,
            filter.CustomerSearch,
            filter.FromUtc,
            filter.ToUtc);

        var list = items.Select(o => new AdminOrderListItemDTO
        {
            Id = o.Id,
            CustomerId = o.CustomerId,
            CustomerName = o.Customer?.Name ?? string.Empty,
            TotalAmount = o.TotalAmount,
            CreatedAt = o.CreatedAt,
            Status = o.Status,
            LineItemCount = o.OrderItems.Count
        }).ToList();

        return new PagedResultDTO<AdminOrderListItemDTO>
        {
            Page = filter.Page,
            PageSize = filter.PageSize,
            TotalCount = total,
            Items = list
        };
    }
}
