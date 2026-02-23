using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using SEM.Domain.Abstractions;
using SEM.Domain.DTOs;

namespace SEM.API.Endpoints;

public static class AuthEndpoints
{
    public static void MapAuthEndpoints(this WebApplication webApplication)
    {
        var group = webApplication.MapGroup("/api/auth").WithOpenApi();
        group.MapPost("/register", async ([FromServices] IUserService userService,
            IValidator<UserAuthRequest> validator,
            UserAuthRequest dto) =>
        {
            var result = await validator.ValidateAsync(dto);
            if (!result.IsValid)
            {
                return Results.ValidationProblem(result.ToDictionary());
            }
            await userService.RegisterAsync(dto);
            return Results.Created();
        });
        group.MapPost("/login", async ([FromServices] IUserService userService, 
            HttpContext httpContext,
            UserAuthRequest request) =>
        {
            var result = await userService.LoginAsync(request, httpContext);
            return !result ? Results.Unauthorized() : Results.Ok();
        });
        group.MapPost("/logout", async ([FromServices] IUserService userService, HttpContext httpContext) =>
        {
            try
            {
                await userService.LogoutAsync(httpContext);
            }
            catch (Exception e)
            {
                return Results.BadRequest(e.Message);
            }
            return Results.Ok();
        });
    }
}