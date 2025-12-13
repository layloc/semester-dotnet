using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Authentication;
using SEM.Domain.DTOs;
using SEM.Domain.Abstractions;
using SEM.Entities;

namespace SEM.Services;

public class UserService(IUserRepository repository) : IUserService
{
    public async Task RegisterAsync(UserAuthRequest credentials)
    {
        var hashPassword = Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(credentials.Password)));
        var user = new User { Email = credentials.Email, HashPassword = hashPassword };
        await repository.AddAsync(user);
    }

    public async Task<bool> LoginAsync(UserAuthRequest credentials, HttpContext httpContext)
    {
        var user = await repository.GetUserByEmail(credentials.Email);
        var enteredPassword = Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(credentials.Password)));
        if (user.HashPassword != enteredPassword)
        {
            return false;
        }
        httpContext.Session.SetString("userId", user.Id.ToString());
        var principal = new ClaimsPrincipal(
            new ClaimsIdentity(
                new List<Claim>
                {
                    new(ClaimTypes.NameIdentifier,  user.Id.ToString())
                }, "session"));
        await httpContext.SignInAsync("session", principal);
        return true;
    }

    public async Task LogoutAsync(HttpContext httpContext)
    {
        httpContext.Session.Remove("userId");
        await httpContext.SignOutAsync("session", new AuthenticationProperties { RedirectUri = "/login" });
    }
}