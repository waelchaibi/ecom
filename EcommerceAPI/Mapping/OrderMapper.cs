using EcommerceAPI.DTOs;
using EcommerceAPI.Models;

namespace EcommerceAPI.Mapping;

public static class OrderMapper
{
    public static OrderDTO ToDto(Order order)
    {
        return new OrderDTO
        {
            Id = order.Id,
            CustomerId = order.CustomerId,
            SubtotalAmount = order.SubtotalAmount,
            TaxAmount = order.TaxAmount,
            ShippingAmount = order.ShippingAmount,
            TotalAmount = order.TotalAmount,
            CreatedAt = order.CreatedAt,
            Status = order.Status,
            PromotionCode = order.PromotionCode,
            OrderItems = order.OrderItems.Select(oi => new OrderItemDTO
            {
                Id = oi.Id,
                ProductId = oi.ProductId,
                ProductName = oi.Product?.Name ?? string.Empty,
                Quantity = oi.Quantity,
                Price = oi.Price
            }).ToList(),
            AssignedGifts = order.OrderGifts.Select(og => new OrderGiftDTO
            {
                GiftId = og.GiftId,
                GiftName = og.Gift?.Name ?? string.Empty,
                Quantity = og.Quantity,
                GiftRuleId = og.GiftRuleId
            }).ToList()
        };
    }
}
