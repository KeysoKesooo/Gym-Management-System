using System.Security.Claims;
using GymManagement.Core.Models.AttendanceModel;
using GymManagement.Core.DTOs.AttendanceDto;
using GymManagement.Core.Repositories.IntAttendanceRepository;
using GymManagement.Core.Services.CacheService;
using GymManagement.Core.Services.RatelimiterService;
using GymManagement.Core.Services.QueueService;

namespace GymManagement.Core.Services.IntAttendanceService
{
    public class AttendanceService : IAttendanceService
    {
        private readonly IAttendanceRepository _repo;
        private readonly RedisCacheService _cache;
        private readonly RedisRateLimiter _rateLimit;
        private readonly IQueueService _queue;

        private readonly TimeSpan _ttl = TimeSpan.FromMinutes(1); // cache TTL

        public AttendanceService(
            IAttendanceRepository repo,
            RedisCacheService cache,
            RedisRateLimiter rateLimit,
            IQueueService queue)
        {
            _repo = repo;
            _cache = cache;
            _rateLimit = rateLimit;
            _queue = queue;
        }

        private string GetCacheKey(int? userId = null) =>
            userId.HasValue ? $"attendance:user:{userId}" : "attendance:all";

        private string GetRateLimitKey(int userId) => $"ratelimit:user:{userId}";

        // ------------------------
        // GET ALL ATTENDANCES
        // ------------------------
        public async Task<IEnumerable<Attendance>> GetAllAttendancesAsync(ClaimsPrincipal user)
        {
            var cacheKey = GetCacheKey();
            var cached = await _cache.GetCacheAsync<IEnumerable<Attendance>>(cacheKey);
            if (cached != null) return cached;

            var attendances = await _repo.GetAllAttendancesAsync();
            await _cache.SetCacheAsync(cacheKey, attendances, _ttl);
            return attendances;
        }

        // ------------------------
        // GET ATTENDANCES BY USER ID
        // ------------------------
        public async Task<IEnumerable<Attendance>> GetByUserIdAttendancesAsync(int userId, ClaimsPrincipal user)
        {
            var cacheKey = GetCacheKey(userId);
            var cached = await _cache.GetCacheAsync<IEnumerable<Attendance>>(cacheKey);
            if (cached != null) return cached;

            var attendances = await _repo.GetAttendancesByUserIdAsync(userId);
            await _cache.SetCacheAsync(cacheKey, attendances, _ttl);
            return attendances;
        }

        // ------------------------
        // GET MY ATTENDANCE
        // ------------------------
        public async Task<IEnumerable<Attendance>> GetMyAttendanceAsync(ClaimsPrincipal user)
        {
            var userIdClaim = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userIdClaim == null || !int.TryParse(userIdClaim, out var userId))
                throw new UnauthorizedAccessException("Invalid or missing user ID in token.");

            return await GetByUserIdAttendancesAsync(userId, user);
        }

        // ------------------------
        // ADD ATTENDANCE
        // ------------------------
        public async Task<Attendance> AddAttendanceAsync(Attendance attendance, ClaimsPrincipal user)
        {
            var role = user.FindFirst(ClaimTypes.Role)?.Value ?? "";
            var cacheKey = GetCacheKey(attendance.UserId);

            if (role == "admin")
            {
                // Write-through
                var added = await _repo.AddAttendanceAsync(attendance);
                await _cache.SetCacheAsync(cacheKey, new[] { added }, _ttl);
                return added;
            }

            // Members → write-behind via queue
            var cached = await _cache.GetCacheAsync<List<Attendance>>(cacheKey) ?? new List<Attendance>();
            cached.Add(attendance);
            await _cache.SetCacheAsync(cacheKey, cached, _ttl);

            var queueItem = new AttendanceQueueModel
            {
                UserId = attendance.UserId,
                Type = "checkin", // generic add
                Time = attendance.CheckIn
            };
            await _queue.EnqueueAsync("attendance_queue", queueItem);

            return attendance;
        }

        // ------------------------
        // UPDATE ATTENDANCE
        // ------------------------
        public async Task<Attendance?> UpdateAttendanceAsync(Attendance attendance, ClaimsPrincipal user)
        {
            var role = user.FindFirst(ClaimTypes.Role)?.Value ?? "";
            var cacheKey = GetCacheKey(attendance.UserId);

            if (role == "admin")
            {
                var updated = await _repo.UpdateAttendanceAsync(attendance);
                if (updated != null)
                    await _cache.SetCacheAsync(cacheKey, new[] { updated }, _ttl);
                return updated;
            }

            // Members → write-behind via queue
            var cached = await _cache.GetCacheAsync<List<Attendance>>(cacheKey) ?? new List<Attendance>();
            var index = cached.FindIndex(a => a.Id == attendance.Id);
            if (index >= 0) cached[index] = attendance;
            else cached.Add(attendance);
            await _cache.SetCacheAsync(cacheKey, cached, _ttl);

            var queueItem = new AttendanceQueueModel
            {
                UserId = attendance.UserId,
                Type = "update",
                Time = attendance.CheckIn,
                AttendanceId = attendance.Id
            };
            await _queue.EnqueueAsync("attendance_queue", queueItem);

            return attendance;
        }

        // ------------------------
        // CHECK-IN / CHECK-OUT (queue + rate limit + pending cache)
        // ------------------------
        public async Task<(string message, List<AttendanceResponseDto> attendances)> CheckInOrOutAsync(ClaimsPrincipal user)
        {
            var userIdClaim = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userIdClaim == null || !int.TryParse(userIdClaim, out var userId))
                throw new UnauthorizedAccessException("Invalid or missing user ID in token.");

            var rateLimitKey = GetRateLimitKey(userId);

            // 1️⃣ Rate-limiting
            var allowed = await _rateLimit.IsAllowedAsync(rateLimitKey, 1, TimeSpan.FromSeconds(30));
            if (!allowed)
                throw new Exception("Too many requests. Wait before checking in/out again.");

            // 2️⃣ Load cached attendances
            var cacheKey = GetCacheKey(userId);
            var cached = await _cache.GetCacheAsync<List<Attendance>>(cacheKey) ?? new List<Attendance>();
            if (!cached.Any())
            {
                var dbAttendances = (await _repo.GetAttendancesByUserIdAsync(userId)).ToList();
                cached = dbAttendances;
                await _cache.SetCacheAsync(cacheKey, cached, _ttl);
            }

            // 3️⃣ Check pending check-in ID from cache
            var pendingCheckInKey = $"pending_checkin:{userId}";
            var pendingCheckInId = await _cache.GetCacheAsync<int?>(pendingCheckInKey);

            AttendanceQueueModel queueItem;
            string message;

            if (pendingCheckInId == null)
            {
                // ✅ No pending check-in → create new check-in
                queueItem = new AttendanceQueueModel
                {
                    UserId = userId,
                    Type = "checkin",
                    Time = DateTime.UtcNow,
                    AttendanceId = 0 // Worker will assign DB id
                };

                // Add placeholder attendance to cache for immediate UI feedback
                cached.Add(new Attendance
                {
                    UserId = userId,
                    CheckIn = queueItem.Time,
                    CheckOut = null
                });

                // Save placeholder ID in cache
                await _cache.SetCacheAsync(pendingCheckInKey, 0, _ttl);

                message = "Checked in (pending DB save).";
            }
            else
            {
                // ✅ Pending check-in exists → check-out
                var last = cached.OrderByDescending(a => a.CheckIn).FirstOrDefault(a => a.CheckOut == null);

                if (last == null)
                {
                    throw new Exception("Cannot check out: no open check-in found.");
                }

                queueItem = new AttendanceQueueModel
                {
                    UserId = userId,
                    Type = "checkout",
                    Time = DateTime.UtcNow,
                    AttendanceId = pendingCheckInId.Value
                };

                last.CheckOut = queueItem.Time;
                message = "Checked out (pending DB save).";

                // Clear pending check-in cache after enqueue
                await _cache.RemoveCacheAsync(pendingCheckInKey);
            }

            // 4️⃣ Update attendances cache immediately
            await _cache.SetCacheAsync(cacheKey, cached, _ttl);

            // 5️⃣ Enqueue for background DB save
            await _queue.EnqueueAsync("attendance_queue", queueItem);

            // 6️⃣ Return DTOs for UI
            var dtoList = cached
                .OrderByDescending(a => a.CheckIn)
                .Select(a => new AttendanceResponseDto
                {
                    Id = a.Id,
                    UserId = a.UserId,
                    CheckIn = a.CheckIn,
                    CheckOut = a.CheckOut
                })
                .ToList();

            return (message, dtoList);
        }

    }
}
