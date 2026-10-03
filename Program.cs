using ChatApp.Services;
using Microsoft.Extensions.AI;
using OllamaSharp;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();

// Ollama chat client with automatic tool-call loop
builder.Services.AddSingleton<IChatClient>(sp =>
{
    var cfg = sp.GetRequiredService<IConfiguration>();
    return new ChatClientBuilder(new OllamaApiClient(new Uri(cfg["Ollama:Url"]!), cfg["Ollama:Model"]!))
        .UseFunctionInvocation()
        .Build();
});

builder.Services.AddSingleton<McpToolService>();   // connects to the MCP server lazily
builder.Services.AddSingleton<ChatService>();      // per-session history + chat calls

var app = builder.Build();

app.UseStaticFiles();
app.UseRouting();
app.MapControllerRoute("default", "{controller=Home}/{action=Index}/{id?}");

app.Run();
