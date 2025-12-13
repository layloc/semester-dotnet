using System.Security.Claims;

namespace WebApplication1.Middlewares;

public class SessionAuthMiddleware : IMiddleware
{
    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        var session = context.Session;
        var userId = session.GetString("userId");
        if (userId != null)
        {
            context.User = new ClaimsPrincipal(
                new ClaimsIdentity(
                    new List<Claim>
                    {
                        new(ClaimTypes.NameIdentifier, userId), 
                    }, "session"));
        }

        await next.Invoke(context);
    }
}