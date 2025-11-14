namespace GymManagement.Core.Services.QueueService
{
    public interface IQueueService
    {
        Task EnqueueAsync<T>(string queue, T data);
        Task<T?> DequeueAsync<T>(string queue);
    }
}
