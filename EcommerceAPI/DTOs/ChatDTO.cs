namespace EcommerceAPI.DTOs;

public class ChatThreadDTO
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}

public class ChatMessageDTO
{
    public int Id { get; set; }
    public string Role { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}

public class SendChatMessageDTO
{
    public string Content { get; set; } = string.Empty;
}

public class SendChatMessageResponseDTO
{
    public ChatMessageDTO UserMessage { get; set; } = new();
    public ChatMessageDTO AssistantMessage { get; set; } = new();
}
