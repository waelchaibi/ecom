using EcommerceAPI.Data;
using EcommerceAPI.DTOs;
using EcommerceAPI.Models;
using EcommerceAPI.Repositories;
using Microsoft.EntityFrameworkCore;

namespace EcommerceAPI.Services;

public sealed class CartService : ICartService
{
    private readonly ICartRepository _cartRepository;
    private readonly IProductRepository _productRepository;
    private readonly IOrderService _orderService;
    private readonly ICheckoutPricingService _checkoutPricing;
    private readonly AppDbContext _context;

    public CartService(
        ICartRepository cartRepository,
        IProductRepository productRepository,
        IOrderService orderService,
        ICheckoutPricingService checkoutPricing,
        AppDbContext context)
    {
        _cartRepository = cartRepository;
        _productRepository = productRepository;
        _orderService = orderService;
        _checkoutPricing = checkoutPricing;
        _context = context;
    }

    public async Task<CartDTO> GetCartAsync(int customerId) =>
        MapCart(await _cartRepository.GetOrCreateAsync(customerId), _checkoutPricing);

    public async Task<CartDTO> UpsertItemAsync(int customerId, CartItemUpsertDTO dto)
    {
        if (dto.ProductId <= 0)
            throw new ArgumentException("Product ID must be greater than 0");

        if (dto.Quantity < 0)
            throw new ArgumentException("Quantity cannot be negative");

        var product = await _productRepository.GetByIdAsync(dto.ProductId);
        if (product is null)
            throw new ArgumentException($"Product with ID {dto.ProductId} not found");

        var cart = await _cartRepository.GetOrCreateAsync(customerId);
        var line = cart.Items.FirstOrDefault(i => i.ProductId == dto.ProductId);

        if (dto.Quantity == 0)
        {
            if (line is not null)
                _context.CartItems.Remove(line);
        }
        else
        {
            if (product.StockQuantity < dto.Quantity)
                throw new ArgumentException(
                    $"Only {product.StockQuantity} units available for '{product.Name}'.");

            if (line is null)
            {
                _context.CartItems.Add(new CartItem
                {
                    CartId = cart.Id,
                    ProductId = dto.ProductId,
                    Quantity = dto.Quantity
                });
            }
            else
            {
                line.Quantity = dto.Quantity;
            }
        }

        cart.UpdatedAt = DateTime.UtcNow;
        await _cartRepository.SaveChangesAsync();
        return MapCart((await _cartRepository.GetByCustomerIdAsync(customerId))!, _checkoutPricing);
    }

    public async Task<CartDTO> RemoveItemAsync(int customerId, int productId)
    {
        var cart = await _cartRepository.GetByCustomerIdAsync(customerId);
        if (cart is null)
            return new CartDTO { EstimatedTax = 0, EstimatedShipping = 0, EstimatedTotal = 0 };

        var line = cart.Items.FirstOrDefault(i => i.ProductId == productId);
        if (line is not null)
        {
            _context.CartItems.Remove(line);
            cart.UpdatedAt = DateTime.UtcNow;
            await _cartRepository.SaveChangesAsync();
        }

        return MapCart((await _cartRepository.GetByCustomerIdAsync(customerId)) ?? cart, _checkoutPricing);
    }

    public async Task ClearCartAsync(int customerId)
    {
        var cart = await _cartRepository.GetByCustomerIdAsync(customerId);
        if (cart is null)
            return;

        await _cartRepository.ClearItemsAsync(cart.Id);
    }

    public async Task<OrderDTO> CheckoutAsync(int customerId, CartCheckoutDTO dto)
    {
        var cart = await _cartRepository.GetByCustomerIdAsync(customerId);
        if (cart is null || cart.Items.Count == 0)
            throw new ArgumentException("Cart is empty");

        var orderDto = new CustomerCreateOrderDTO
        {
            Items = cart.Items.Select(i => new CreateOrderItemDTO
            {
                ProductId = i.ProductId,
                Quantity = i.Quantity
            }).ToList(),
            PromotionCode = dto.PromotionCode
        };

        var order = await _orderService.CreateOrderForCustomerAsync(customerId, orderDto);
        await ClearCartAsync(customerId);
        return order;
    }

    private static CartDTO MapCart(Cart cart, ICheckoutPricingService pricing)
    {
        var lines = cart.Items
            .Where(i => i.Product is not null)
            .Select(i => new CartLineDTO
            {
                ProductId = i.ProductId,
                ProductName = i.Product!.Name,
                ImageUrl = i.Product.ImageUrl,
                UnitPrice = i.Product.Price,
                Quantity = i.Quantity,
                LineTotal = i.Product.Price * i.Quantity,
                StockQuantity = i.Product.StockQuantity
            })
            .ToList();

        var subtotal = lines.Sum(l => l.LineTotal);
        var estimate = pricing.Calculate(subtotal);

        return new CartDTO
        {
            Items = lines,
            Subtotal = subtotal,
            EstimatedTax = estimate.TaxAmount,
            EstimatedShipping = estimate.ShippingAmount,
            EstimatedTotal = estimate.TotalAmount
        };
    }
}
