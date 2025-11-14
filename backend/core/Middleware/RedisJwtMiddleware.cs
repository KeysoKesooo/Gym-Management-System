using System.IdentityModel.Tokens.Jwt;
using GymManagement.Core.DTOs.AuthDto;
using GymManagement.Core.Services.SessionService;

namespace GymManagement.Core.Middleware.RJMiddleware
{
    public class RedisJwtMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly string[] _publicPaths = new[]
        {
            "/api/auth/login",
            "/api/auth/register"
        };

        public RedisJwtMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context, RedisSessionService redisSession)
        {
            // ✅ Skip OPTIONS requests (CORS preflight)
            if (context.Request.Method == HttpMethods.Options)
            {
                context.Response.StatusCode = StatusCodes.Status204NoContent;
                return;
            }

            // ✅ Skip public paths
            if (_publicPaths.Any(p => context.Request.Path.StartsWithSegments(p, StringComparison.OrdinalIgnoreCase)))
            {
                await _next(context);
                return;
            }

            // Get Bearer token from header
            var authHeader = context.Request.Headers["Authorization"].ToString();
            if (string.IsNullOrEmpty(authHeader) || !authHeader.StartsWith("Bearer "))
            {
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                await context.Response.WriteAsJsonAsync(new { error = "Authorization header missing or invalid" });
                return;
            }

            var token = authHeader.Replace("Bearer ", "").Trim();

            // Validate session from Redis
            var session = await redisSession.GetSessionAsync<SessionDto>(token);
            if (session == null)
            {
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                await context.Response.WriteAsJsonAsync(new { error = "Session expired or invalid. Please log in again." });
                return;
            }

            // Validate JWT expiry
            var jwt = new JwtSecurityTokenHandler().ReadJwtToken(token);
            if (jwt.ValidTo < DateTime.UtcNow)
            {
                await redisSession.RemoveSessionAsync(token);
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                await context.Response.WriteAsJsonAsync(new { error = "Token has expired." });
                return;
            }

            // Attach session to HttpContext
            context.Items["UserSession"] = session;

            await _next(context);
        }
    }
}
