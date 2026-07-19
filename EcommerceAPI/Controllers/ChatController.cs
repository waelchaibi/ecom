using System.Security.Claims;
using EcommerceAPI.DTOs;
using EcommerceAPI.Extensions;
using EcommerceAPI.Models;
using EcommerceAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EcommerceAPI.Controllers;

[ApiController]
[Route("api/chat")]
[Authorize(Roles = AuthRoles.Admin + "," + AuthRoles.Customer)]
public class ChatController : ControllerBase
{
    private readonly IChatService _chat;

    public ChatController(IChatService chat) => _chat = chat;

    [HttpGet("threads")]
    [ProducesResponseType(typeof(IReadOnlyList<ChatThreadDTO>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<ChatThreadDTO>>> ListThreads(CancellationToken cancellationToken)
    {
        var (isAdmin, customerId, adminUser) = ResolveIdentity();
        return Ok(await _chat.ListThreadsAsync(isAdmin, customerId, adminUser, cancellationToken));
    }

    [HttpPost("threads")]
    [ProducesResponseType(typeof(ChatThreadDTO), StatusCodes.Status201Created)]
    public async Task<ActionResult<ChatThreadDTO>> CreateThread(CancellationToken cancellationToken)
    {
        var (isAdmin, customerId, adminUser) = ResolveIdentity();
        var thread = await _chat.CreateThreadAsync(isAdmin, customerId, adminUser, cancellationToken);
        return StatusCode(StatusCodes.Status201Created, thread);
    }

    [HttpGet("threads/{id:int}/messages")]
    [ProducesResponseType(typeof(IReadOnlyList<ChatMessageDTO>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<ChatMessageDTO>>> GetMessages(int id, CancellationToken cancellationToken)
    {
        var (isAdmin, customerId, adminUser) = ResolveIdentity();
        return Ok(await _chat.GetMessagesAsync(id, isAdmin, customerId, adminUser, cancellationToken));
    }

    [HttpPost("threads/{id:int}/messages")]
    [ProducesResponseType(typeof(SendChatMessageResponseDTO), StatusCodes.Status200OK)]
    public async Task<ActionResult<SendChatMessageResponseDTO>> SendMessage(
        int id,
        [FromBody] SendChatMessageDTO dto,
        CancellationToken cancellationToken)
    {
        var (isAdmin, customerId, adminUser) = ResolveIdentity();
        return Ok(await _chat.SendMessageAsync(id, dto.Content, isAdmin, customerId, adminUser, cancellationToken));
    }

    private (bool IsAdmin, int? CustomerId, string? AdminUsername) ResolveIdentity()
    {
        if (User.IsInRole(AuthRoles.Admin))
        {
            var name = User.Identity?.Name
                ?? User.FindFirstValue(ClaimTypes.Name)
                ?? User.FindFirstValue(ClaimTypes.NameIdentifier)
                ?? "admin";
            return (true, null, name);
        }

        return (false, User.GetCustomerId(), null);
    }
}
