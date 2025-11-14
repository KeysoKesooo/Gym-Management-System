using GymManagement.Core.Services.RatelimiterService;


namespace GymManagement.Core.Middleware
{
    public class RateLimitMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly int _limit;
        private readonly TimeSpan _period;

        public RateLimitMiddleware(RequestDelegate next, int limit = 10, TimeSpan? period = null)
        {
            _next = next;
            _limit = limit;
            _period = period ?? TimeSpan.FromSeconds(10);
        }

        public async Task InvokeAsync(HttpContext context, RedisRateLimiter limiter)
        {
            var key = $"rate:{context.Connection.RemoteIpAddress}";
            var allowed = await limiter.IsAllowedAsync(key, _limit, _period);

            if (!allowed)
            {
                context.Response.StatusCode = StatusCodes.Status429TooManyRequests;
                await context.Response.WriteAsJsonAsync(new { error = "Rate limit exceeded" });
                return;
            }

            await _next(context);
        }
    }
}
