namespace ChatApp.Models;

public record ChatRequest(string SessionId, string Message);
public record ChatResponse(string Reply, List<string> ToolsUsed);
