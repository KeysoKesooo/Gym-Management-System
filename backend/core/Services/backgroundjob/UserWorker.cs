using GymManagement.Core.Repositories.IntUserRepository;
using GymManagement.Core.DTOs.UserDto;
using GymManagement.Core.Models.UserModel;
using GymManagement.Core.Services.QueueService;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace GymManagement.Core.Workers.UserWorker
{
    public class UserWorker : BackgroundService
    {
        private readonly ILogger<UserWorker> _logger;
        private readonly IQueueService _queue;
        private readonly IServiceProvider _provider;

        public UserWorker(ILogger<UserWorker> logger, IQueueService queue, IServiceProvider provider)
        {
            _logger = logger;
            _queue = queue;
            _provider = provider;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                var item = await _queue.DequeueAsync<UserQueueDto>("user_queue");
                if (item != null)
                {
                    try
                    {
                        using var scope = _provider.CreateScope();
                        var repo = scope.ServiceProvider.GetRequiredService<IUserRepository>();

                        if (item.Id.HasValue)
                        {
                            // Update
                            var existing = await repo.GetByIdAsync(item.Id.Value);
                            if (existing != null)
                            {
                                existing.Name = item.Name;
                                existing.Email = item.Email;
                                existing.PasswordHash = item.PasswordHash;
                                existing.Role = item.Role;
                                await repo.UpdateAsync(existing);
                            }
                        }
                        else
                        {
                            // Add
                            var newUser = new User
                            {
                                Name = item.Name,
                                Email = item.Email,
                                PasswordHash = item.PasswordHash,
                                Role = item.Role
                            };
                            await repo.AddAsync(newUser);
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Failed to process user queue item");
                    }
                }
                else
                {
                    await Task.Delay(500, stoppingToken); // sleep if queue empty
                }
            }
        }
    }
}
