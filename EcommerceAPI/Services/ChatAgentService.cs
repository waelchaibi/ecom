using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using EcommerceAPI.Configuration;
using Microsoft.Extensions.Options;

namespace EcommerceAPI.Services;

public interface IChatAgentService
{
    Task<string> ReplyAsync(
        IReadOnlyList<(string Role, string Content)> history,
        string userMessage,
        bool isAdmin,
        int? customerId,
        CancellationToken cancellationToken = default);
}

public sealed class ChatAgentService : IChatAgentService
{
    private readonly HttpClient _http;
    private readonly GroqOptions _options;
    private readonly IChatQueryTools _tools;

    public ChatAgentService(HttpClient http, IOptions<GroqOptions> options, IChatQueryTools tools)
    {
        _http = http;
        _options = options.Value;
        _tools = tools;
    }

    public async Task<string> ReplyAsync(
        IReadOnlyList<(string Role, string Content)> history,
        string userMessage,
        bool isAdmin,
        int? customerId,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(_options.ApiKey))
            throw new InvalidOperationException(
                "Groq API key is not configured. Set Groq:ApiKey in appsettings.Development.json or user secrets.");

        var messages = new List<Dictionary<string, object?>>
        {
            new()
            {
                ["role"] = "system",
                ["content"] = BuildSystemPrompt(isAdmin)
            }
        };

        foreach (var (role, content) in history.TakeLast(20))
        {
            if (role is "user" or "assistant")
                messages.Add(new Dictionary<string, object?> { ["role"] = role, ["content"] = content });
        }

        messages.Add(new Dictionary<string, object?> { ["role"] = "user", ["content"] = userMessage });

        var tools = _tools.GetToolDefinitions(isAdmin);

        for (var i = 0; i < 6; i++)
        {
            var body = new Dictionary<string, object?>
            {
                ["model"] = _options.Model,
                ["messages"] = messages,
                ["tools"] = tools,
                ["tool_choice"] = "auto",
                ["temperature"] = 0.2
            };

            using var req = new HttpRequestMessage(HttpMethod.Post, $"{_options.ApiBaseUrl.TrimEnd('/')}/chat/completions");
            req.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _options.ApiKey);
            req.Content = new StringContent(JsonSerializer.Serialize(body), Encoding.UTF8, "application/json");

            using var res = await _http.SendAsync(req, cancellationToken);
            var raw = await res.Content.ReadAsStringAsync(cancellationToken);
            if (!res.IsSuccessStatusCode)
                throw new InvalidOperationException($"Groq API error ({(int)res.StatusCode}): {raw}");

            using var doc = JsonDocument.Parse(raw);
            var choice = doc.RootElement.GetProperty("choices")[0].GetProperty("message");
            var assistantMsg = new Dictionary<string, object?>
            {
                ["role"] = "assistant",
                ["content"] = choice.TryGetProperty("content", out var c) && c.ValueKind != JsonValueKind.Null
                    ? c.GetString()
                    : null
            };

            if (choice.TryGetProperty("tool_calls", out var toolCalls) && toolCalls.ValueKind == JsonValueKind.Array && toolCalls.GetArrayLength() > 0)
            {
                var serializedCalls = JsonSerializer.Deserialize<JsonElement>(toolCalls.GetRawText());
                assistantMsg["tool_calls"] = serializedCalls;
                messages.Add(assistantMsg);

                foreach (var call in toolCalls.EnumerateArray())
                {
                    var id = call.GetProperty("id").GetString() ?? "";
                    var fn = call.GetProperty("function");
                    var name = fn.GetProperty("name").GetString() ?? "";
                    var args = fn.TryGetProperty("arguments", out var a) ? a.GetString() ?? "{}" : "{}";
                    var result = await _tools.ExecuteAsync(name, args, isAdmin, customerId, cancellationToken);
                    messages.Add(new Dictionary<string, object?>
                    {
                        ["role"] = "tool",
                        ["tool_call_id"] = id,
                        ["content"] = result
                    });
                }

                continue;
            }

            var text = assistantMsg["content"] as string;
            return string.IsNullOrWhiteSpace(text)
                ? "I could not produce an answer from the available data."
                : text!;
        }

        return "I reached the tool-call limit. Please rephrase your question.";
    }

    private static string BuildSystemPrompt(bool isAdmin) =>
        "You are the Olympia e-commerce assistant. Answer in the same language the user uses (French or English). "
        + "Use only tool results for facts about products, orders, customers, analytics, gifts, or audit logs. "
        + "Never invent IDs, prices, stock, or order status. If tools return no data, say so. "
        + "You are read-only: refuse any request to create, update, delete, pay, cancel, or change data."
        + (isAdmin
            ? " The user is an Admin — you may use admin tools (customers with CIN, all orders, analytics, gifts, audit)."
            : " The user is a Customer — only their own profile/orders plus public catalog tools.");
}
