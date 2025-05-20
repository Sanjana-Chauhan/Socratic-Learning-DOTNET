// using Microsoft.AspNetCore.Builder;
// using Microsoft.AspNetCore.Routing;
// using SOCRATIC_LEARNING_DOTNET.Services;
// using SOCRATIC_LEARNING_DOTNET.Entities;

// namespace SOCRATIC_LEARNING_DOTNET.endpoints;
// public static class promptEndpoints
// {
//     public static RouteGroupBuilder MapPromptEndpoints(this WebApplication app)
//     {
//         var group = app.MapGroup("/prompt").WithTags("Prompt Endpoints");

//         group.MapPost(
//             "/",
//             async (GroqService groq, string userMsg) =>
//             {
//                 var reply = await groq.GetGroqChatCompletionAsync(userMsg);
//                 //reply received here is AIResponse Objectc
//                 if (reply == null)
//                 {
//                     return Results.Problem("Something went wrong");
//                 }
//                 return Results.Ok(reply);
//             }
//         );

//         group.MapGet("/", () => "ALL Stored Prompts will be displayed here");
//         group.MapGet("/{id}", () => "Promptwith speified id will be displayed here");
//         return group;
//     }
// }
