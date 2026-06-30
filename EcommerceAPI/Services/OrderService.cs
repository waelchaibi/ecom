using EcommerceAPI.Data;
using EcommerceAPI.DTOs;
using EcommerceAPI.GiftRules;
using EcommerceAPI.Mapping;
using EcommerceAPI.Models;
using EcommerceAPI.Repositories;
using Microsoft.EntityFrameworkCore;

namespace EcommerceAPI.Services;

public class OrderService : IOrderService
{
    private readonly IOrderRepository _orderRepository;
    private readonly IProductRepository _productRepository;
    private readonly IGiftRepository _giftRepository;
    private readonly AppDbContext _context;
    private readonly IGiftAssignmentService _giftAssignmentService;
    private readonly ICheckoutPricingService _checkoutPricing;
    private readonly IPaymentGatewayService _paymentGateway;
    private readonly ILogger<OrderService> _logger;

    public OrderService(
        IOrderRepository orderRepository,
        IProductRepository productRepository,
        IGiftRepository giftRepository,
        AppDbContext context,
        IGiftAssignmentService giftAssignmentService,
        ICheckoutPricingService checkoutPricing,
        IPaymentGatewayService paymentGateway,
        ILogger<OrderService> logger)
    {
        _orderRepository = orderRepository;
        _productRepository = productRepository;
        _giftRepository = giftRepository;
        _context = context;
        _giftAssignmentService = giftAssignmentService;
        _checkoutPricing = checkoutPricing;
        _paymentGateway = paymentGateway;
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

    public async Task<OrderDTO?> GetOrderForCustomerAsync(int customerId, int orderId)
    {
        if (customerId <= 0 || orderId <= 0)
            throw new ArgumentException("Valid customer and order IDs are required");

        var order = await _orderRepository.GetByIdAsync(orderId);
        if (order is null || order.CustomerId != customerId)
            return null;

        return MapToDTO(order);
    }

    public async Task<IReadOnlyList<OrderDTO>> GetOrdersForCustomerAsync(int customerId)
    {
        if (customerId <= 0)
            throw new ArgumentException("Customer ID must be greater than 0");

        var orders = await _orderRepository.GetByCustomerIdAsync(customerId);
        return orders.Select(MapToDTO).ToList();
    }

    public async Task<OrderDTO> CreateOrderForCustomerAsync(int customerId, CustomerCreateOrderDTO dto)
    {
        if (customerId <= 0)
            throw new ArgumentException("Valid customer ID is required");

        if (dto.Items == null || dto.Items.Count == 0)
            throw new ArgumentException("Order must contain at least one item");

        var customer = await _context.Customers.FindAsync(customerId);
        if (customer == null)
            throw new ArgumentException($"Customer with ID {customerId} not found");

        var promo = string.IsNullOrWhiteSpace(dto.PromotionCode) ? null : dto.PromotionCode.Trim();

        decimal subtotalAmount = 0;
        var orderItems = new List<OrderItem>();
        var utc = DateTime.UtcNow;

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

            orderItems.Add(new OrderItem
            {
                ProductId = item.ProductId,
                Quantity = item.Quantity,
                Price = product.Price
            });
            subtotalAmount += product.Price * item.Quantity;
        }

        var pricing = _checkoutPricing.Calculate(subtotalAmount);
        var taxAmount = pricing.TaxAmount;
        var shippingAmount = pricing.ShippingAmount;
        var totalAmount = pricing.TotalAmount;

        var order = new Order
        {
            CustomerId = customerId,
            SubtotalAmount = subtotalAmount,
            TaxAmount = taxAmount,
            ShippingAmount = shippingAmount,
            TotalAmount = totalAmount,
            Status = OrderStatuses.Pending,
            PromotionCode = promo,
            OrderItems = orderItems,
            CreatedAt = utc
        };

        await using var transaction = await _context.Database.BeginTransactionAsync();
        try
        {
            foreach (var item in dto.Items)
            {
                var ok = await _productRepository.TryDecrementStockAsync(item.ProductId, item.Quantity, utc);
                if (!ok)
                {
                    throw new InvalidOperationException(
                        $"Could not reserve stock for product {item.ProductId} (concurrent sale or insufficient quantity). Retry the order.");
                }
            }

            _context.Orders.Add(order);
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

    public async Task<OrderDTO> ConfirmPaymentAsync(int orderId)
    {
        if (orderId <= 0)
            throw new ArgumentException("Order ID must be greater than 0");

        await using var transaction = await _context.Database.BeginTransactionAsync();
        try
        {
            var order = await _context.Orders
                .Include(o => o.OrderItems)
                .Include(o => o.OrderGifts)
                .Include(o => o.Customer)
                .FirstOrDefaultAsync(o => o.Id == orderId);

            if (order is null)
                throw new KeyNotFoundException($"Order with ID {orderId} not found.");

            if (order.Status == OrderStatuses.Confirmed)
            {
                await transaction.CommitAsync();
                return MapToDTO((await _orderRepository.GetByIdAsync(orderId))!);
            }

            if (order.Status != OrderStatuses.Pending)
            {
                throw new InvalidOperationException(
                    $"Cannot confirm payment for order in status '{order.Status}'. Only '{OrderStatuses.Pending}' orders can be confirmed.");
            }

            var customer = order.Customer ?? await _context.Customers.FindAsync(order.CustomerId);
            if (customer is null)
                throw new InvalidOperationException("Customer record is missing for this order.");

            var priorQualifyingOrderCount = await _context.Orders
                .AsNoTracking()
                .CountAsync(o =>
                    o.CustomerId == order.CustomerId &&
                    o.Id != order.Id &&
                    (o.Status == OrderStatuses.Confirmed || o.Status == OrderStatuses.Shipped));

            var previewOrder = new Order
            {
                CustomerId = order.CustomerId,
                TotalAmount = order.TotalAmount,
                Status = OrderStatuses.Confirmed,
                Customer = customer,
                OrderItems = order.OrderItems.ToList()
            };

            var giftContext = new GiftRuleEvaluationContext(
                previewOrder,
                customer,
                priorQualifyingOrderCount,
                order.PromotionCode);

            var giftApplications = await _giftAssignmentService.EvaluateAsync(giftContext);

            foreach (var app in giftApplications)
            {
                var gift = await _context.Gifts.FindAsync(app.GiftId);
                if (gift is null)
                {
                    _logger.LogWarning("Gift {GiftId} referenced by rule {RuleId} was not found; skipping.", app.GiftId, app.GiftRuleId);
                    continue;
                }

                var reserved = await _giftRepository.TryDecrementStockAsync(app.GiftId, 1);
                if (!reserved)
                {
                    _logger.LogWarning(
                        "Gift {GiftId} ({GiftName}) could not be assigned for order {OrderId}: insufficient stock.",
                        gift.Id,
                        gift.Name,
                        order.Id);
                    continue;
                }

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

            order.Status = OrderStatuses.Confirmed;
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

    public async Task<OrderDTO> CancelOrderAsync(int orderId)
    {
        if (orderId <= 0)
            throw new ArgumentException("Order ID must be greater than 0");

        await using var transaction = await _context.Database.BeginTransactionAsync();
        try
        {
            var order = await _context.Orders
                .Include(o => o.OrderItems)
                .Include(o => o.OrderGifts)
                .FirstOrDefaultAsync(o => o.Id == orderId);

            if (order is null)
                throw new KeyNotFoundException($"Order with ID {orderId} not found.");

            if (order.Status == OrderStatuses.Cancelled)
            {
                await transaction.CommitAsync();
                return MapToDTO((await _orderRepository.GetByIdAsync(orderId))!);
            }

            if (order.Status != OrderStatuses.Pending &&
                order.Status != OrderStatuses.Confirmed &&
                order.Status != OrderStatuses.Shipped)
            {
                throw new InvalidOperationException($"Cannot cancel order in status '{order.Status}'.");
            }

            var utc = DateTime.UtcNow;

            foreach (var line in order.OrderItems)
                await _productRepository.IncrementStockAsync(line.ProductId, line.Quantity, utc);

            if (order.Status == OrderStatuses.Confirmed || order.Status == OrderStatuses.Shipped)
            {
                foreach (var og in order.OrderGifts.ToList())
                    await _giftRepository.IncrementStockAsync(og.GiftId, og.Quantity);
            }

            order.Status = OrderStatuses.Cancelled;
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

    public async Task<OrderDTO> PayOrderAsCustomerAsync(int customerId, int orderId, SimulatePaymentDTO payment)
    {
        if (payment is null)
            throw new ArgumentException("Payment details are required.");

        var order = await EnsureOrderOwnedByCustomerAsync(customerId, orderId);
        if (order.Status != OrderStatuses.Pending)
        {
            throw new InvalidOperationException(
                $"Only orders awaiting payment can be paid. Current status: '{order.Status}'.");
        }

        await _paymentGateway.ProcessCardPaymentAsync(payment, order.TotalAmount, orderId);
        return await ConfirmPaymentAsync(orderId);
    }

    public async Task<OrderDTO> CancelOrderAsCustomerAsync(int customerId, int orderId)
    {
        var order = await EnsureOrderOwnedByCustomerAsync(customerId, orderId);
        if (order.Status != OrderStatuses.Pending)
        {
            throw new InvalidOperationException(
                $"Only orders awaiting payment can be cancelled online. Current status: '{order.Status}'.");
        }

        return await CancelOrderAsync(orderId);
    }

    private async Task<Order> EnsureOrderOwnedByCustomerAsync(int customerId, int orderId)
    {
        if (customerId <= 0 || orderId <= 0)
            throw new ArgumentException("Valid customer and order IDs are required.");

        var order = await _orderRepository.GetByIdAsync(orderId);
        if (order is null || order.CustomerId != customerId)
            throw new KeyNotFoundException($"Order with ID {orderId} not found.");

        return order;
    }

    private static OrderDTO MapToDTO(Order order) => OrderMapper.ToDto(order);
}
