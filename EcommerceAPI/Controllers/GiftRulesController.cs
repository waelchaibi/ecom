using EcommerceAPI.DTOs;
using EcommerceAPI.Services;
using Microsoft.AspNetCore.Mvc;

namespace EcommerceAPI.Controllers;

[ApiController]
[Route("api/giftrules")]
public class GiftRulesController : ControllerBase
{
    private readonly IGiftRuleService _giftRuleService;

    public GiftRulesController(IGiftRuleService giftRuleService)
    {
        _giftRuleService = giftRuleService;
    }

    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<GiftRuleDTO>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<GiftRuleDTO>>> GetAll()
    {
        var list = await _giftRuleService.GetAllAsync();
        return Ok(list);
    }

    [HttpPost]
    [ProducesResponseType(typeof(GiftRuleDTO), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<GiftRuleDTO>> Create([FromBody] CreateGiftRuleDTO dto)
    {
        var created = await _giftRuleService.CreateAsync(dto);
        return StatusCode(StatusCodes.Status201Created, created);
    }

    [HttpPut("{id:int}")]
    [ProducesResponseType(typeof(GiftRuleDTO), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<GiftRuleDTO>> Update(int id, [FromBody] UpdateGiftRuleDTO dto)
    {
        var updated = await _giftRuleService.UpdateAsync(id, dto);
        return Ok(updated);
    }
}
