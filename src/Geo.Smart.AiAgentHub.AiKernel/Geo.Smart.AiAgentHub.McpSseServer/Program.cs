using Geo.Smart.AiAgentHub.McpSseServer.Tools;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

builder.Services
    .AddMcpServer()
    .WithHttpTransport()
    .WithTools<WeatherTool>();

var app = builder.Build();

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.MapMcp();

await app.RunAsync();