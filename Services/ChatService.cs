using System.Collections.Concurrent;
using System.Text.RegularExpressions;
using ChatApp.Models;
using Microsoft.Extensions.AI;
namespace ChatApp.Services;

public class ChatService(IChatClient chat, McpToolService mcp, IConfiguration cfg)
{
    private readonly ConcurrentDictionary<string, List<ChatMessage>> _sessions = new();

    public async Task<Models.ChatResponse> AskAsync(string sessionId, string message, CancellationToken ct)
    {
        var tools = await mcp.GetToolsAsync(ct);

        var history = _sessions.GetOrAdd(sessionId,
            _ => [new ChatMessage(ChatRole.System, cfg["SystemPrompt"]!)]);
        history.Add(new ChatMessage(ChatRole.User, message));

        var response = await chat.GetResponseAsync(history, new ChatOptions { Tools = tools }, ct);
        history.AddMessages(response);   // keep tool calls/results in context

        // Qwen3 models may emit <think>...</think>; hide it from the UI
        var reply = Regex.Replace(response.Text, "<think>.*?</think>", "", RegexOptions.Singleline).Trim();

        var used = response.Messages.SelectMany(m => m.Contents)
            .OfType<FunctionCallContent>().Select(c => c.Name).Distinct().ToList();

        return new Models.ChatResponse(reply, used);
    }

    public void Reset(string sessionId) => _sessions.TryRemove(sessionId, out _);
}
