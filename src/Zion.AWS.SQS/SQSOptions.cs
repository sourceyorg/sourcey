namespace Zion.AWS.SQS
{
    public sealed class SQSOptions
    {
        internal List<QueueOptions> _readQueues;
        internal List<SQSQueue> _writeQueues;

        internal IEnumerable<SQSQueue> SQSPublishQueues => _writeQueues;
        internal IEnumerable<QueueOptions> SQSReadQueues => _readQueues;

        public string AWSKey { get; set; }
        public string AWSSecret { get; set; }
        public long? PollingInterval { get; set; }

        public void WithReadQueue(SQSQueue name, string? deadLetterQueueUrl = null, long? maxReceiveCount = null, long? receiveWaitTime = null) 
            => _readQueues.Add(new(name, deadLetterQueueUrl, maxReceiveCount, receiveWaitTime));
        public void WithWriteQueue(SQSQueue name)
            => _writeQueues.Add(name);
    }

    public sealed record QueueOptions(SQSQueue Name, string? DeadLetterQueueUrl = null, long? MaxReceiveCount = null, long? ReceiveWaitTime = null);
}
