using ChatApp.Models;
using ChatApp.Services;
using Microsoft.AspNetCore.Mvc;

namespace ChatApp.Controllers;

public class ChatController(ChatService chat, ILogger<ChatController> logger) : Controller
{
    [HttpPost]
    public async Task<IActionResult> Send([FromBody] ChatRequest req, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(req.Message)) return BadRequest("Message is required.");
        try
        {
            return Json(await chat.AskAsync(req.SessionId, req.Message, ct));
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Chat failed");
            return StatusCode(500, new ChatResponse(
                "Error: " + ex.Message + " (is Ollama running and the MCP server started?)", []));
        }
    }

    [HttpPost]
    public IActionResult Reset([FromBody] ChatRequest req)
    {
        chat.Reset(req.SessionId);
        return Ok();
    }
}
