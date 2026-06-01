using EcommerceAPI.DTOs;
using EcommerceAPI.Services;
using Microsoft.AspNetCore.Mvc;

namespace EcommerceAPI.Controllers;

[ApiController]
[Route("api/gifts")]
public class GiftsController : ControllerBase
{
    private readonly IGiftService _giftService;

    public GiftsController(IGiftService giftService)
    {
        _giftService = giftService;
    }

    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<GiftDTO>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<GiftDTO>>> GetAll()
    {
        var list = await _giftService.GetAllAsync();
        return Ok(list);
    }

    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(GiftDTO), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<GiftDTO>> GetById(int id)
    {
        var gift = await _giftService.GetByIdAsync(id);
        if (gift is null)
            return NotFound();
        return Ok(gift);
    }
}
