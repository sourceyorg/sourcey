namespace Zion.AWS.SQS.Queues
{
    public interface IQueueManager
    {
        Task AddOrUpdateQueueAsync(QueueOptions queue, CancellationToken cancellationToken = default);
    }
}
