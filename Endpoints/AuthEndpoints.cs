// Endpoints/AuthEndpoints.cs
using Microsoft.AspNetCore.Mvc;
using SOCRATIC_LEARNING_DOTNET.Entities;
using SOCRATIC_LEARNING_DOTNET.Interfaces;
using SOCRATIC_LEARNING_DOTNET.Services;

namespace SOCRATIC_LEARNING_DOTNET.Endpoints;

public static class AuthEndpoints
{
    public static void MapAuthEndpoints(this WebApplication app)
    {
        // POST: Signup
        app.MapPost(
            "/api/auth/signup",
            async (
                [FromServices] IUserService userService,
                [FromServices] JwtService jwtService,
                [FromBody] UserRegisterRequest req
            ) =>
            {
                var user = await userService.AddUserAsync(req.name, req.email, req.password);
                if (user == null)
                    return Results.BadRequest("User already exists");
                var token = jwtService.GenerateToken(user);
                return Results.Ok(
                    new
                    {
                        token,
                        userId = user.Id,
                        userName = user.Name,
                    }
                );
            }
        );

        // POST: Login
        app.MapPost(
            "/api/auth/login",
            async (
                [FromServices] IUserService userService,
                [FromServices] JwtService jwtService,
                [FromBody] UserLoginRequest req
            ) =>
            {
                var user = await userService.ValidateUserAsync(req.email, req.password);

                if (user == null)
                    return Results.Unauthorized();

                var token = jwtService.GenerateToken(user);
                return Results.Ok(
                    new
                    {
                        token,
                        userId = user.Id,
                        userName = user.Name,
                    }
                );
            }
        );

        app.MapGet(
            "/api/verify-token",
            async ([FromQuery] string token, [FromServices] JwtService jwtService) =>
            {
                var principal = jwtService.ValidateToken(token);
                return principal != null ? Results.Ok() : Results.Unauthorized();
            }
        );
    }
}
