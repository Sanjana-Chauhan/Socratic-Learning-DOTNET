using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using SOCRATIC_LEARNING_DOTNET.Entities;
using SOCRATIC_LEARNING_DOTNET.Interfaces;
using SOCRATIC_LEARNING_DOTNET.Services;
using System.Security.Claims;  // <-- Added this

namespace SOCRATIC_LEARNING_DOTNET.Endpoints;

public static class ChatEndpoints
{
    public static void MapChatEndpoints(this WebApplication app)
    {
        // ✅ Welcome Message (Public)
        app.MapGet("/", () => "Welcome to Socratic Learning!");

        // ✅ List conversations (Secured)
        app.MapGet(
                "/api/conversations",
                async (IChatService svc, HttpContext http) =>
                {
                    var userId = http.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                    if (userId is null)
                        return Results.Unauthorized();

                    var result = await svc.ListConversationsAsync(userId);
                    return Results.Ok(result);
                }
            )
            .RequireAuthorization();

        // ✅ Get messages in a conversation (Secured)
        app.MapGet(
                "/api/conversations/{id}/messages",
                async (Guid id, IChatService svc, HttpContext http) =>
                {
                    var userId = http.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                    if (userId is null)
                        return Results.Unauthorized();

                    var conv = await svc.GetOrCreateConversationAsync(userId, id);
                    if (conv.UserId != userId)
                        return Results.Forbid();

                    var messages = await svc.GetMessagesAsync(id);
                    return Results.Ok(messages);
                }
            )
            .RequireAuthorization();

        // ✅ Post a prompt, get AI response, save both (Secured)
        app.MapPost(
                "/api/chat",
                async (
                    [FromServices] IChatService svc,
                    [FromServices] GroqService aiSvc,
                    [FromBody] PromptRequest req,
                    HttpContext http
                ) =>
                {
                    var userId = http.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                    if (userId is null)
                        return Results.Unauthorized();

                    var conv = await svc.GetOrCreateConversationAsync(userId, req.ConversationId);

                    var userMsg = new Message
                    {
                        id = Guid.NewGuid(),
                        conversationId = conv.Id,
                        role = "user",
                        Content = string.IsNullOrWhiteSpace(req.Prompt) ? "New Chat" : req.Prompt,
                        timestamp = DateTime.UtcNow,
                    };
                    await svc.AddMessageAsync(userMsg);

                    var messages = await svc.GetMessagesAsync(conv.Id);

                    AIResponse aiResp;
                    try
                    {
                        aiResp = await aiSvc.GetGroqChatCompletionAsync(messages);
                    }
                    catch (Exception ex)
                    {
                        return Results.Problem($"AI failed: {ex.Message}");
                    }

                    var aiMsg = new Message
                    {
                        id = Guid.NewGuid(),
                        conversationId = conv.Id,
                        role = "assistant",
                        Content = aiResp.Response ?? "Please try again.",
                        timestamp = DateTime.UtcNow,
                    };
                    await svc.AddMessageAsync(aiMsg);

                    return Results.Ok(
                        new { conversationId = conv.Id, aiResponse = aiResp.Response }
                    );
                }
            )
            .RequireAuthorization();
    }
}
