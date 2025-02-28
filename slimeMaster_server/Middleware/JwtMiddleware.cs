using slimeMaster_server.Services;

namespace slimeMaster_server.Middleware;

public class JwtMiddleware
{
    private readonly RequestDelegate _next;
    private readonly AuthService _authService;

    public JwtMiddleware(RequestDelegate next, AuthService authService)
    {
        _next = next;
        _authService = authService;
    }

    public async Task Invoke(HttpContext context)
    {
        var token = context.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
        if (token != null)
        {
            var principal = _authService.ValidateJwtToken(token);
            if (principal == null)
            {
                context.Response.StatusCode = 401;
                await context.Response.WriteAsync("Invalid Token");
                return;
            }
            context.User = principal;
        }

        await _next(context);
    }
}
