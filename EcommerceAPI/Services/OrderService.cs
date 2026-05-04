using EcommerceAPI.Data;
using EcommerceAPI.DTOs;
using EcommerceAPI.Models;
using EcommerceAPI.Repositories;

namespace EcommerceAPI.Services;

public class OrderService : IOrderService
{
    private readonly IOrderRepository _orderRepository;
    private readonly IProductRepository _productRepository;
    private readonly AppDbContext _context;

    public OrderService(
        IOrderRepository orderRepository,
        IProductRepository productRepository,
        AppDbContext context)
    {
        _orderRepository = orderRepository;
        _productRepository = productRepository;
        _context = context;
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

        // Verify customer exists
        var customer = await _context.Customers.FindAsync(dto.CustomerId);
        if (customer == null)
            throw new ArgumentException($"Customer with ID {dto.CustomerId} not found");

        // Validate and calculate totals
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
            await transaction.CommitAsync();

            return MapToDTO(order);
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
            }).ToList()
        };
    }
}