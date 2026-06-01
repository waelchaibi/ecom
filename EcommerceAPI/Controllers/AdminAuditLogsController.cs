using EcommerceAPI.DTOs;
using EcommerceAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EcommerceAPI.Controllers;

[ApiController]
[Route("api/admin/audit-logs")]
[Authorize(Roles = "Admin")]
public class AdminAuditLogsController : ControllerBase
{
    private readonly IAdminAuditService _auditService;

    public AdminAuditLogsController(IAdminAuditService auditService) => _auditService = auditService;

    [HttpGet]
    [ProducesResponseType(typeof(PagedResultDTO<AuditLogDTO>), StatusCodes.Status200OK)]
    public async Task<ActionResult<PagedResultDTO<AuditLogDTO>>> List(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 50,
        [FromQuery] string? action = null,
        [FromQuery] string? entityType = null,
        [FromQuery] DateTime? fromUtc = null,
        [FromQuery] DateTime? toUtc = null)
    {
        var filter = new AuditLogFilterDTO
        {
            Page = page,
            PageSize = pageSize,
            Action = action,
            EntityType = entityType,
            FromUtc = fromUtc,
            ToUtc = toUtc
        };
        return Ok(await _auditService.ListAsync(filter));
    }
}
