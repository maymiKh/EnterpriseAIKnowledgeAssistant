
using EnterpriseAI.Api.Contracts;
using EnterpriseAI.Api.Services;
using Microsoft.Extensions.AI;
using OllamaSharp;

var builder = WebApplication.CreateBuilder(args);

// ==========================================
// 1. Configuration
// ==========================================

var ollamaUrl = builder.Configuration["AI:Ollama:BaseUrl"]
    ?? "http://localhost:11434";

var model = builder.Configuration["AI:Ollama:Model"]
    ?? "qwen2.5:7b";

// ==========================================
// 2. Dependency Injection - AI Client
// ==========================================

IChatClient chatClient = new OllamaApiClient(
    new Uri(ollamaUrl),
    model
);

builder.Services.AddChatClient(chatClient);

// ==========================================
// 3. Dependency Injection - Application Services
// ==========================================

builder.Services.AddScoped<IChatService, ChatService>();

// ==========================================
// 4. OpenAPI Configuration
// ==========================================

builder.Services.AddOpenApi();

var app = builder.Build();

// ==========================================
// 5. HTTP Request Pipeline
// ==========================================

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

// ==========================================
// 6. AI Chat Endpoint
// ==========================================

app.MapPost("/api/chat", async (
    ChatRequest request,
    IChatService chatService,
    CancellationToken cancellationToken) =>
{
    if (string.IsNullOrWhiteSpace(request.Message))
    {
        return Results.BadRequest(new
        {
            Error = "Message cannot be empty."
        });
    }

    var answer = await chatService.GetResponse(
        request.Message,
        cancellationToken
    );

    return Results.Ok(new EnterpriseAI.Api.Contracts.ChatResponse(answer));
})
.WithName("ChatWithAI");

// ==========================================
// 7. Run Application
// ==========================================

app.Run();
