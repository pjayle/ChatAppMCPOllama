using Microsoft.Extensions.AI;
using ModelContextProtocol.Client;

namespace ChatApp.Services;

public class McpToolService(IConfiguration cfg) : IAsyncDisposable
{
    private readonly SemaphoreSlim _lock = new(1, 1);
    private McpClient? _client;
    private List<AITool>? _tools;

    public async Task<List<AITool>> GetToolsAsync(CancellationToken ct = default)
    {
        if (_tools is not null) return _tools;

        await _lock.WaitAsync(ct);
        try
        {
            if (_tools is null)
            {
                var transport = new HttpClientTransport(new HttpClientTransportOptions
                {
                    Name = "ProductMaster",
                    Endpoint = new Uri(cfg["Mcp:Endpoint"]!)
                });
                _client = await McpClient.CreateAsync(transport, cancellationToken: ct);
                _tools = (await _client.ListToolsAsync(cancellationToken: ct)).Cast<AITool>().ToList();
            }
        }
        finally { _lock.Release(); }

        return _tools;
    }

    public async ValueTask DisposeAsync()
    {
        if (_client is not null) await _client.DisposeAsync();
    }
}
