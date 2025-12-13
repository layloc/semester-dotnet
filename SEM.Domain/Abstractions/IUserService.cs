using SEM.Domain.DTOs;
using SEM.Entities;

namespace SEM.Domain.Abstractions;

public interface IUserService
{
    public Task LogoutAsync(HttpContext httpContext);

    public Task<bool> LoginAsync(UserAuthRequest credentials, HttpContext httpContext);
    public Task RegisterAsync(UserAuthRequest credentials);
}