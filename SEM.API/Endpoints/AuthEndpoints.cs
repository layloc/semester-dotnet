using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using SEM.Domain.Abstractions;
using SEM.Domain.DTOs;
using SEM.Services;

namespace WebApplication1.Endpoints;

public static class AuthEndpoints
{
    public static void MapAuthEndpoints(this WebApplication webApplication)
    {
        var group = webApplication.MapGroup("/api/auth").WithOpenApi();
        group.MapPost("/register", async ([FromServices] IUserService userService, UserAuthRequest dto) =>
        {
            await userService.RegisterAsync(dto);
            return Results.Created();
        });
        group.MapPost("/login", async ([FromServices] IUserService userService, HttpContext httpContext, UserAuthRequest request) =>
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