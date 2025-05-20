using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using SOCRATIC_LEARNING_DOTNET.Data;
using SOCRATIC_LEARNING_DOTNET.Entities;
using SOCRATIC_LEARNING_DOTNET.Interfaces;
using SOCRATIC_LEARNING_DOTNET.Services;

var builder = WebApplication.CreateBuilder(args);

// Swagger setup
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// HttpClient for GroqService
builder.Services.AddHttpClient<GroqService>();

// PostgreSQL connection
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"))
);

// Dependency injection for services
builder.Services.AddScoped<IChatService, ChatService>();
builder.Services.AddScoped<GroqService>();

var app = builder.Build();

// Dev tools
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

//  GET: List Conversations
app.MapGet(
    "/api/conversations",
    async ([FromServices] IChatService svc, [FromQuery] string userId) =>
        Results.Ok(await svc.ListConversationsAsync(userId))
);

//  GET: Messages in a conversation
app.MapGet(
    "/api/conversations/{id}/messages",
    async ([FromServices] IChatService svc, Guid id) => Results.Ok(await svc.GetMessagesAsync(id))
);

// POST: Prompt AI and save messages
app.MapPost(
    "/api/chat",
    async (
        [FromServices] IChatService svc,
        [FromServices] GroqService aiSvc,
        [FromBody] PromptRequest req
    ) =>
    {
        var conv = await svc.GetOrCreateConversationAsync(req.UserId, req.ConversationId);

        // Save user message
        var userMsg = new Message
        {
            id = Guid.NewGuid(),
            conversationId = conv.Id,
            role = "user",
            content = req.Prompt,
            timestamp = DateTime.UtcNow,
        };
        await svc.AddMessageAsync(userMsg);

        // Get AI response
        var aiResp = await aiSvc.GetGroqChatCompletionAsync(req.Prompt);

        // Save AI message
        var aiMsg = new Message
        {
            id = Guid.NewGuid(),
            conversationId = conv.Id,
            role = "assistant",
            content = aiResp.Explanation,
            timestamp = DateTime.UtcNow,
        };
        await svc.AddMessageAsync(aiMsg);

        // Return response
        return Results.Ok(
            new
            {
                conversationId = conv.Id,
                aiResp.Explanation,
                aiResp.FollowUpQuestion,
                aiResp.Summary,
            }
        );
    }
);

app.Run();
