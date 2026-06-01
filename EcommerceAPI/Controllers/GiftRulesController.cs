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
}
