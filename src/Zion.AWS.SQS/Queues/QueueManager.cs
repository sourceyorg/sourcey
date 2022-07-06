using Amazon.SQS;
using Amazon.SQS.Model;
using Zion.AWS.SQS.Factories;

namespace Zion.AWS.SQS.Queues
{
    internal sealed class QueueManager : IQueueManager
    {
        private readonly IClientFactory _clientFactory;

        public QueueManager(IClientFactory clientFactory)
        {
            if (clientFactory is null)
                throw new ArgumentNullException(nameof(clientFactory));

            _clientFactory = clientFactory;
        }

        public async Task AddOrUpdateQueueAsync(QueueOptions queue, CancellationToken cancellationToken = default)
        {
            var queueUrl = await GetQueueUrlAsync(queue.Name, cancellationToken);

            if (!string.IsNullOrWhiteSpace(queueUrl))
            {
                await UpdateQueue(queue, queueUrl, cancellationToken);
                return;
            }

            var deadLetterQueueUrl = string.IsNullOrWhiteSpace(queue.DeadLetterQueueUrl) 
                ? await CreateQueueAsync(new QueueOptions(SQSQueue.From($"{queue.Name}__dlq")), cancellationToken) 
                : queue.DeadLetterQueueUrl;

            await CreateQueueAsync(new QueueOptions(queue.Name, deadLetterQueueUrl, queue.MaxReceiveCount, queue.ReceiveWaitTime), cancellationToken);
        }

        private async Task UpdateQueue(QueueOptions queue, string queueUrl, CancellationToken cancellationToken)
        {
            using var client = _clientFactory.Create();
            await client.SetQueueAttributesAsync(queueUrl, await BuildQueueAttributesAsync(client, queue, cancellationToken));
        }

        private async Task<string> CreateQueueAsync(QueueOptions queueOptions, CancellationToken cancellationToken)
        {
            using var client = _clientFactory.Create();

            var response = await client.CreateQueueAsync(new CreateQueueRequest { QueueName = queueOptions.Name, Attributes = await BuildQueueAttributesAsync(client, queueOptions, cancellationToken) }, cancellationToken);
            return response.QueueUrl;
        }

        private async Task<string> GetQueueArn(IAmazonSQS sqsClient, string queueUrl, CancellationToken cancellationToken)
        {
            GetQueueAttributesResponse responseGetAtt = await sqsClient.GetQueueAttributesAsync(queueUrl, new List<string> { QueueAttributeName.QueueArn }, cancellationToken);
            return responseGetAtt.QueueARN;
        }

        private async Task<string?> GetQueueUrlAsync(SQSQueue queue, CancellationToken cancellationToken)
        {
            using var client = _clientFactory.Create();
            try
            {
                var response = await client.GetQueueUrlAsync(queue, cancellationToken);

                if (response.HttpStatusCode == System.Net.HttpStatusCode.OK)
                    return response.QueueUrl;

                return null;
            }
            catch(QueueDoesNotExistException)
            {
                return null;
            }
        }

        private async Task<Dictionary<string, string>> BuildQueueAttributesAsync(IAmazonSQS client, QueueOptions queueOptions, CancellationToken cancellationToken)
            => string.IsNullOrWhiteSpace(queueOptions.DeadLetterQueueUrl)
            ? new()
            : new()
            {
                [QueueAttributeName.ReceiveMessageWaitTimeSeconds] = queueOptions.ReceiveWaitTime?.ToString() ?? string.Empty,
                [QueueAttributeName.RedrivePolicy] = $"{{\"deadLetterTargetArn\":\"{await GetQueueArn(client, queueOptions.DeadLetterQueueUrl, cancellationToken)}\",\"maxReceiveCount\":\"{queueOptions.MaxReceiveCount?.ToString() ?? string.Empty}\"}}"
            };
    }
}
