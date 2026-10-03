# ASP.NET Core MVC: Chat (Ollama qwen3.5:2b) + MCP Product Server

Two MVC apps:
- ProductMcpServer (http://localhost:5100) - MVC page /Products + MCP endpoint /mcp
- ChatApp          (http://localhost:5200) - MVC chat UI, calls Ollama and MCP tools

## Prerequisites
- .NET 9 SDK (for .NET 8 change TargetFramework to net8.0 in both csproj files)
- Ollama running on http://localhost:11434

## Run
    ollama pull qwen3.5:2b

Terminal 1 (start the MCP server first):
    cd ProductMcpServer
    dotnet run

Terminal 2:
    cd ChatApp
    dotnet run

Open http://localhost:5200 (product master table: http://localhost:5100)

## Configuration (ChatApp/appsettings.json)
- Ollama:Url / Ollama:Model - Ollama endpoint and model tag
- Mcp:Endpoint              - MCP server URL (default http://localhost:5100/mcp)
- SystemPrompt              - forces tool usage for product questions
- Urls                      - listening port (same key exists in ProductMcpServer/appsettings.json)

## Try
- List all products
- Details of product 1003
- Which products expire within 7 days?
- Dairy products under 150 rupees
- Products made by Nestle India

## Test MCP server alone
    npx @modelcontextprotocol/inspector   (Transport: Streamable HTTP, URL: http://localhost:5100/mcp)
