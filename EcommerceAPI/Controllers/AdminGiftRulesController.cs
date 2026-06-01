using EcommerceAPI.DTOs;
using EcommerceAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EcommerceAPI.Controllers;

[ApiController]
[Route("api/admin/gift-rules")]
[Authorize(Roles = "Admin")]
public class AdminGiftRulesController : ControllerBase
{
    private readonly IGiftRuleService _giftRuleService;

    public AdminGiftRulesController(IGiftRuleService giftRuleService)
    {
        _giftRuleService = giftRuleService;
    }

    [HttpPost]
    [ProducesResponseType(typeof(GiftRuleDTO), StatusCodes.Status201Created)]
    public async Task<ActionResult<GiftRuleDTO>> Create([FromBody] CreateGiftRuleDTO dto)
    {
        var created = await _giftRuleService.CreateAsync(dto);
        return StatusCode(StatusCodes.Status201Created, created);
    }

    [HttpPut("{id:int}")]
    [ProducesResponseType(typeof(GiftRuleDTO), StatusCodes.Status200OK)]
    public async Task<ActionResult<GiftRuleDTO>> Update(int id, [FromBody] UpdateGiftRuleDTO dto)
    {
        return Ok(await _giftRuleService.UpdateAsync(id, dto));
    }

    [HttpPatch("{id:int}/active")]
    [ProducesResponseType(typeof(GiftRuleDTO), StatusCodes.Status200OK)]
    public async Task<ActionResult<GiftRuleDTO>> SetActive(int id, [FromBody] SetGiftRuleActiveDTO dto)
    {
        return Ok(await _giftRuleService.SetActiveAsync(id, dto.IsActive));
    }

    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> Delete(int id)
    {
        await _giftRuleService.DeleteAsync(id);
        return NoContent();
    }
}
