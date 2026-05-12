using EcommerceAPI.DTOs;
using EcommerceAPI.Models;
using EcommerceAPI.Repositories;

namespace EcommerceAPI.Services;

public class GiftService : IGiftService
{
    private readonly IGiftRepository _repository;

    public GiftService(IGiftRepository repository)
    {
        _repository = repository;
    }

    public async Task<List<GiftDTO>> GetAllAsync()
    {
        var gifts = await _repository.GetAllAsync();
        return gifts.Select(Map).ToList();
    }

    public async Task<GiftDTO?> GetByIdAsync(int id)
    {
        if (id <= 0)
            throw new ArgumentException("Gift ID must be greater than 0");

        var gift = await _repository.GetByIdAsync(id);
        return gift is null ? null : Map(gift);
    }

    public async Task<GiftDTO> CreateAsync(CreateGiftDTO dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Name))
            throw new ArgumentException("Gift name is required");

        if (dto.StockQuantity < 0)
            throw new ArgumentException("Stock quantity cannot be negative");

        var entity = new Gift
        {
            Name = dto.Name.Trim(),
            Description = dto.Description?.Trim() ?? string.Empty,
            StockQuantity = dto.StockQuantity
        };

        var created = await _repository.AddAsync(entity);
        return Map(created);
    }

    private static GiftDTO Map(Gift g) => new()
    {
        Id = g.Id,
        Name = g.Name,
        Description = g.Description,
        StockQuantity = g.StockQuantity
    };
}
