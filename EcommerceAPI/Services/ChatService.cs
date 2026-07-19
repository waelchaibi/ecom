using EcommerceAPI.Data;
using EcommerceAPI.DTOs;
using EcommerceAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace EcommerceAPI.Services;

public interface IChatService
{
    Task<IReadOnlyList<ChatThreadDTO>> ListThreadsAsync(bool isAdmin, int? customerId, string? adminUsername, CancellationToken ct = default);
    Task<ChatThreadDTO> CreateThreadAsync(bool isAdmin, int? customerId, string? adminUsername, CancellationToken ct = default);
    Task<IReadOnlyList<ChatMessageDTO>> GetMessagesAsync(int threadId, bool isAdmin, int? customerId, string? adminUsername, CancellationToken ct = default);
    Task<SendChatMessageResponseDTO> SendMessageAsync(int threadId, string content, bool isAdmin, int? customerId, string? adminUsername, CancellationToken ct = default);
}

public sealed class ChatService : IChatService
{
    private readonly AppDbContext _db;
    private readonly IChatAgentService _agent;

    public ChatService(AppDbContext db, IChatAgentService agent)
    {
        _db = db;
        _agent = agent;
    }

    public async Task<IReadOnlyList<ChatThreadDTO>> ListThreadsAsync(
        bool isAdmin, int? customerId, string? adminUsername, CancellationToken ct = default)
    {
        var q = _db.ChatThreads.AsNoTracking().AsQueryable();
        q = isAdmin
            ? q.Where(t => t.UserRole == AuthRoles.Admin && t.AdminUsername == adminUsername)
            : q.Where(t => t.UserRole == AuthRoles.Customer && t.CustomerId == customerId);

        return await q.OrderByDescending(t => t.UpdatedAt)
            .Select(t => new ChatThreadDTO { Id = t.Id, Title = t.Title, CreatedAt = t.CreatedAt, UpdatedAt = t.UpdatedAt })
            .ToListAsync(ct);
    }

    public async Task<ChatThreadDTO> CreateThreadAsync(
        bool isAdmin, int? customerId, string? adminUsername, CancellationToken ct = default)
    {
        var thread = new ChatThread
        {
            UserRole = isAdmin ? AuthRoles.Admin : AuthRoles.Customer,
            CustomerId = isAdmin ? null : customerId,
            AdminUsername = isAdmin ? adminUsername : null,
            Title = "New chat",
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
        _db.ChatThreads.Add(thread);
        await _db.SaveChangesAsync(ct);
        return MapThread(thread);
    }

    public async Task<IReadOnlyList<ChatMessageDTO>> GetMessagesAsync(
        int threadId, bool isAdmin, int? customerId, string? adminUsername, CancellationToken ct = default)
    {
        var thread = await GetOwnedThreadAsync(threadId, isAdmin, customerId, adminUsername, ct);
        return await _db.ChatMessages.AsNoTracking()
            .Where(m => m.ThreadId == thread.Id && (m.Role == ChatRoles.User || m.Role == ChatRoles.Assistant))
            .OrderBy(m => m.Id)
            .Select(m => new ChatMessageDTO { Id = m.Id, Role = m.Role, Content = m.Content, CreatedAt = m.CreatedAt })
            .ToListAsync(ct);
    }

    public async Task<SendChatMessageResponseDTO> SendMessageAsync(
        int threadId, string content, bool isAdmin, int? customerId, string? adminUsername, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(content))
            throw new ArgumentException("Message content is required.");

        var thread = await GetOwnedThreadAsync(threadId, isAdmin, customerId, adminUsername, ct);
        var history = await _db.ChatMessages.AsNoTracking()
            .Where(m => m.ThreadId == thread.Id && (m.Role == ChatRoles.User || m.Role == ChatRoles.Assistant))
            .OrderBy(m => m.Id)
            .Select(m => new { m.Role, m.Content })
            .ToListAsync(ct);

        var userMsg = new ChatMessage
        {
            ThreadId = thread.Id,
            Role = ChatRoles.User,
            Content = content.Trim(),
            CreatedAt = DateTime.UtcNow
        };
        _db.ChatMessages.Add(userMsg);

        if (thread.Title == "New chat")
            thread.Title = content.Trim().Length <= 60 ? content.Trim() : content.Trim()[..57] + "...";
        thread.UpdatedAt = DateTime.UtcNow;

        var reply = await _agent.ReplyAsync(
            history.Select(h => (h.Role, h.Content)).ToList(),
            content.Trim(),
            isAdmin,
            customerId,
            ct);

        var assistantMsg = new ChatMessage
        {
            ThreadId = thread.Id,
            Role = ChatRoles.Assistant,
            Content = reply,
            CreatedAt = DateTime.UtcNow
        };
        _db.ChatMessages.Add(assistantMsg);
        await _db.SaveChangesAsync(ct);

        return new SendChatMessageResponseDTO
        {
            UserMessage = MapMessage(userMsg),
            AssistantMessage = MapMessage(assistantMsg)
        };
    }

    private async Task<ChatThread> GetOwnedThreadAsync(
        int threadId, bool isAdmin, int? customerId, string? adminUsername, CancellationToken ct)
    {
        var thread = await _db.ChatThreads.FirstOrDefaultAsync(t => t.Id == threadId, ct)
            ?? throw new KeyNotFoundException($"Chat thread {threadId} not found.");

        var ok = isAdmin
            ? thread.UserRole == AuthRoles.Admin && thread.AdminUsername == adminUsername
            : thread.UserRole == AuthRoles.Customer && thread.CustomerId == customerId;

        if (!ok)
            throw new UnauthorizedAccessException("You do not own this chat thread.");

        return thread;
    }

    private static ChatThreadDTO MapThread(ChatThread t) => new()
    {
        Id = t.Id,
        Title = t.Title,
        CreatedAt = t.CreatedAt,
        UpdatedAt = t.UpdatedAt
    };

    private static ChatMessageDTO MapMessage(ChatMessage m) => new()
    {
        Id = m.Id,
        Role = m.Role,
        Content = m.Content,
        CreatedAt = m.CreatedAt
    };
}
