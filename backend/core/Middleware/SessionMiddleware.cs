using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Http;
using GymManagement.Core.Services.RedisService;
using GymManagement.Core.DTOs.AuthDto;


namespace GymManagement.Core.Middleware.Session; // ✅ SessionMiddleware

public class RedisSessionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly RedisSessionService _redis;

    public RedisSessionMiddleware(RequestDelegate next, RedisSessionService redis)
    {
        _next = next;
        _redis = redis;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var authHeader = context.Request.Headers["Authorization"].ToString();

        if (!string.IsNullOrEmpty(authHeader) && authHeader.StartsWith("Bearer "))
        {
            var token = authHeader.Replace("Bearer ", "").Trim();

            // ✅ Get session from Redis using SessionDto
            var session = await _redis.GetSessionAsync<SessionDto>(token);
            if (session == null)
            {
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                await context.Response.WriteAsJsonAsync(new
                {
                    error = "Session expired or invalid. Please log in again."
                });
                return;
            }

            // ✅ Optional: validate token expiration manually
            var jwt = new JwtSecurityTokenHandler().ReadJwtToken(token);
            if (jwt.ValidTo < DateTime.UtcNow)
            {
                await _redis.RemoveSessionAsync(token);
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                await context.Response.WriteAsJsonAsync(new
                {
                    error = "Token has expired."
                });
                return;
            }

            // ✅ Attach session to HttpContext for controllers
            context.Items["UserSession"] = session;
        }

        await _next(context);
    }
}
