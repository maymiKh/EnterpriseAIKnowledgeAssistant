
using EnterpriseAI.Api.Configuration;
using EnterpriseAI.Api.Contracts;
using EnterpriseAI.Api.Exercises;
using EnterpriseAI.Api.Services;
using Microsoft.Extensions.AI;
using OllamaSharp;
using System.ComponentModel.DataAnnotations;

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
builder.Services.AddTransient<AsyncExercises>();
builder.Services.AddTransient<ConcurrentExercises>();

// ==========================================
// 4. OpenAPI Configuration
// ==========================================

builder.Services.AddOpenApi();
builder.Services.AddProblemDetails();
builder.Services
    .AddOptions<AiOptions>()
    .Bind(builder.Configuration.GetSection(AiOptions.SectionName))
    .Validate(
        options => options.RequestTimeoutSeconds > 0,
        "AI request timeout must be greater than zero.")
    .ValidateOnStart();

var app = builder.Build();

app.UseExceptionHandler(errorApp =>
{
    errorApp.Run(async context =>
    {
        var exception = context.Features
            .Get<Microsoft.AspNetCore.Diagnostics.IExceptionHandlerFeature>()
            ?.Error;

        if (exception is TimeoutException)
        {
            context.Response.StatusCode =
                StatusCodes.Status504GatewayTimeout;

            await Results.Problem(
                statusCode: StatusCodes.Status504GatewayTimeout,
                title: "AI request timed out.",
                detail: "The AI model did not respond within the configured time.")
                .ExecuteAsync(context);

            return;
        }

        context.Response.StatusCode =
            StatusCodes.Status500InternalServerError;

        await Results.Problem(
            statusCode: StatusCodes.Status500InternalServerError,
            title: "An unexpected error occurred.")
            .ExecuteAsync(context);
    });
});
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

app.MapGet("/api/test/sequential", async (
    ConcurrentExercises exercises) =>
{
    var stopwatch = System.Diagnostics.Stopwatch.StartNew();

    var results = await exercises.GetDataSequentiallyAsync();

    stopwatch.Stop();

    return Results.Ok(new
    {
        Results = results,
        DurationMilliseconds = stopwatch.ElapsedMilliseconds
    });
});

app.MapGet("/api/test/concurrent", async (
    ConcurrentExercises exercises) =>
{
    var stopwatch = System.Diagnostics.Stopwatch.StartNew();

    var results = await exercises.GetDataConcurrentlyAsync();

    stopwatch.Stop();

    return Results.Ok(new
    {
        Results = results,
        DurationMilliseconds = stopwatch.ElapsedMilliseconds
    });
});

app.MapGet("/api/test/concurrentany", async (
    ConcurrentExercises exercises) =>
{
    var stopwatch = System.Diagnostics.Stopwatch.StartNew();

    var results = await exercises.GetDataConcurrentlyAnyAsync();

    stopwatch.Stop();

    return Results.Ok(new
    {
        Results = results,
        DurationMilliseconds = stopwatch.ElapsedMilliseconds
    });
});

app.MapGet("/api/test/async", async (
    AsyncExercises exercises) =>
{
    var result = await exercises.GetDataAsync();

    return Results.Ok(result);
});

app.MapGet("/api/test/processasync", async (
    ConcurrentExercises exercises, CancellationToken cancellationToken) =>
{
    var result = await exercises.ProcessAsync(cancellationToken);

    return Results.Ok(result);
});

app.MapPost("/api/chat", async (
    ChatRequest request,
    IChatService chatService,
    CancellationToken cancellationToken) =>
{
    var validationResults = new List<ValidationResult>();

    var validationContext = new ValidationContext(request);

    bool isValid = Validator.TryValidateObject(
        request,
        validationContext,
        validationResults,
        validateAllProperties: true);

    if (!isValid)
    {
        var errors = validationResults
            .Select(error => error.ErrorMessage)
            .ToArray();

        return Results.ValidationProblem(
            new Dictionary<string, string[]>
            {
                ["message"] = errors!
            });
    }

    var answer = await chatService.GetResponse(
        request.Message,
        cancellationToken);

    return Results.Ok(new EnterpriseAI.Api.Contracts.ChatResponse(answer));
})
.WithName("ChatWithAI");

// ==========================================
// 7. Run Application
// ==========================================

app.Run();
