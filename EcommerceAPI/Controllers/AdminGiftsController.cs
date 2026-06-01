using EcommerceAPI.DTOs;
using EcommerceAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EcommerceAPI.Controllers;

[ApiController]
[Route("api/admin/gifts")]
[Authorize(Roles = "Admin")]
public class AdminGiftsController : ControllerBase
{
    private readonly IGiftService _giftService;

    public AdminGiftsController(IGiftService giftService)
    {
        _giftService = giftService;
    }

    [HttpPost]
    [ProducesResponseType(typeof(GiftDTO), StatusCodes.Status201Created)]
    public async Task<ActionResult<GiftDTO>> Create([FromBody] CreateGiftDTO dto)
    {
        var created = await _giftService.CreateAsync(dto);
        return StatusCode(StatusCodes.Status201Created, created);
    }
}
