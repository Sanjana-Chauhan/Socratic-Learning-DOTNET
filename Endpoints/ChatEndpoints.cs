using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using SOCRATIC_LEARNING_DOTNET.Entities;
using SOCRATIC_LEARNING_DOTNET.Interfaces;
using SOCRATIC_LEARNING_DOTNET.Services;

namespace SOCRATIC_LEARNING_DOTNET.Endpoints;

public static class ChatEndpoints
{
    public static void MapChatEndpoints(this WebApplication app)
    {
        //Welcome message
        app.MapGet(
            "/",
            () =>
            {
                return "Welcome to Socratic Learning!";
            }
        );
        // GET: List Conversations
        app.MapGet(
            "/api/conversations",
            async ([FromServices] IChatService svc, [FromQuery] string userId) =>
                Results.Ok(await svc.ListConversationsAsync(userId))
        );

        // GET: Messages in a conversation
        app.MapGet(
            "/api/conversations/{id}/messages",
            async ([FromServices] IChatService svc, Guid id) =>
                Results.Ok(await svc.GetMessagesAsync(id))
        );

        // POST: Send prompt, get AI response, save both
        app.MapPost(
            "/api/chat",
            async (
                [FromServices] IChatService svc,
                [FromServices] GroqService aiSvc,
                [FromBody] PromptRequest req
            ) =>
            {
                var conv = await svc.GetOrCreateConversationAsync(req.UserId, req.ConversationId);

                var userMsg = new Message
                {
                    id = Guid.NewGuid(),
                    conversationId = conv.Id,
                    role = "user",
                    Content = req.Prompt,
                    timestamp = DateTime.UtcNow,
                };
                await svc.AddMessageAsync(userMsg);

                var messageList = await svc.GetMessagesAsync(conv.Id);

                AIResponse aiResp;
                try
                {
                    aiResp = await aiSvc.GetGroqChatCompletionAsync(messageList);
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

                return Results.Ok(new { conversationId = conv.Id, aiResponse = aiResp.Response });
            }
        );
    }
}
