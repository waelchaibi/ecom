using EcommerceAPI.Data;
using EcommerceAPI.DTOs;
using EcommerceAPI.GiftRules;
using EcommerceAPI.Models;
using EcommerceAPI.Repositories;
using Microsoft.EntityFrameworkCore;

namespace EcommerceAPI.Services;

public class OrderService : IOrderService
{
    private readonly IOrderRepository _orderRepository;
    private readonly IProductRepository _productRepository;
    private readonly AppDbContext _context;
    private readonly IGiftAssignmentService _giftAssignmentService;
    private readonly ILogger<OrderService> _logger;

    public OrderService(
        IOrderRepository orderRepository,
        IProductRepository productRepository,
        AppDbContext context,
        IGiftAssignmentService giftAssignmentService,
        ILogger<OrderService> logger)
    {
        _orderRepository = orderRepository;
        _productRepository = productRepository;
        _context = context;
        _giftAssignmentService = giftAssignmentService;
        _logger = logger;
    }

    public async Task<OrderDTO?> GetOrderByIdAsync(int id)
    {
        if (id <= 0)
            throw new ArgumentException("Order ID must be greater than 0");

        var order = await _orderRepository.GetByIdAsync(id);
        if (order == null)
            return null;

        return MapToDTO(order);
    }

    public async Task<OrderDTO> CreateOrderAsync(CreateOrderDTO dto)
    {
        if (dto.CustomerId <= 0)
            throw new ArgumentException("Valid customer ID is required");

        if (dto.Items == null || dto.Items.Count == 0)
            throw new ArgumentException("Order must contain at least one item");

        var customer = await _context.Customers.FindAsync(dto.CustomerId);
        if (customer == null)
            throw new ArgumentException($"Customer with ID {dto.CustomerId} not found");

        var priorCompletedOrderCount = await _context.Orders
            .CountAsync(o => o.CustomerId == dto.CustomerId);

        decimal totalAmount = 0;
        var orderItems = new List<OrderItem>();

        await using var transaction = await _context.Database.BeginTransactionAsync();
        try
        {
            foreach (var item in dto.Items)
            {
                if (item.ProductId <= 0)
                    throw new ArgumentException("Product ID must be greater than 0");

                if (item.Quantity <= 0)
                    throw new ArgumentException("Quantity must be greater than 0");

                var product = await _productRepository.GetByIdAsync(item.ProductId);
                if (product == null)
                    throw new ArgumentException($"Product with ID {item.ProductId} not found");

                if (product.StockQuantity < item.Quantity)
                    throw new ArgumentException(
                        $"Insufficient stock for product '{product.Name}'. Available: {product.StockQuantity}, Requested: {item.Quantity}");

                product.StockQuantity -= item.Quantity;
                product.UpdatedAt = DateTime.UtcNow;

                orderItems.Add(new OrderItem
                {
                    ProductId = item.ProductId,
                    Quantity = item.Quantity,
                    Price = product.Price
                });
                totalAmount += product.Price * item.Quantity;
            }

            var previewOrder = new Order
            {
                CustomerId = dto.CustomerId,
                TotalAmount = totalAmount,
                Status = "Confirmed",
                Customer = customer
            };

            var giftContext = new GiftRuleEvaluationContext(
                previewOrder,
                customer,
                priorCompletedOrderCount,
                dto.PromotionCode);

            var giftApplications = await _giftAssignmentService.EvaluateAsync(giftContext);

            var order = new Order
            {
                CustomerId = dto.CustomerId,
                TotalAmount = totalAmount,
                Status = "Confirmed",
                OrderItems = orderItems,
                CreatedAt = DateTime.UtcNow
            };

            _context.Orders.Add(order);
            await _context.SaveChangesAsync();

            foreach (var app in giftApplications)
            {
                var gift = await _context.Gifts.FindAsync(app.GiftId);
                if (gift is null)
                {
                    _logger.LogWarning("Gift {GiftId} referenced by rule {RuleId} was not found; skipping.", app.GiftId, app.GiftRuleId);
                    continue;
                }

                if (gift.StockQuantity < 1)
                {
                    _logger.LogWarning(
                        "Gift {GiftId} ({GiftName}) could not be assigned for order {OrderId}: insufficient stock.",
                        gift.Id,
                        gift.Name,
                        order.Id);
                    continue;
                }

                gift.StockQuantity -= 1;

                _context.OrderGifts.Add(new OrderGift
                {
                    OrderId = order.Id,
                    GiftId = gift.Id,
                    GiftRuleId = app.GiftRuleId,
                    Quantity = 1
                });

                _logger.LogInformation(
                    "Applied gift rule {RuleId} for order {OrderId}; assigned gift {GiftId} ({GiftName}).",
                    app.GiftRuleId,
                    order.Id,
                    gift.Id,
                    gift.Name);
            }

            await _context.SaveChangesAsync();
            await transaction.CommitAsync();

            var reloaded = await _orderRepository.GetByIdAsync(order.Id);
            return MapToDTO(reloaded!);
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    private OrderDTO MapToDTO(Order order)
    {
        return new OrderDTO
        {
            Id = order.Id,
            CustomerId = order.CustomerId,
            TotalAmount = order.TotalAmount,
            CreatedAt = order.CreatedAt,
            Status = order.Status,
            OrderItems = order.OrderItems.Select(oi => new OrderItemDTO
            {
                Id = oi.Id,
                ProductId = oi.ProductId,
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
