using Microsoft.Azure.ServiceBus;

namespace Zion.Azure.ServiceBus.Queues
{
    public class ServiceBusQueueOptions
    {
        internal string EntityPath { get; private set; }
        internal string ConnectionString { get; private set; }
        internal ReceiveMode ReceiveMode { get; private set; } = ReceiveMode.PeekLock;
        internal RetryPolicy RetryPolicy { get; private set; } = RetryPolicy.Default;
        internal bool EnableConsumer { get; private set; } = true;
        public void WithEntityName(string entityName) => EntityPath = entityName;
        public void WithConsumer(bool enableConsumer) => EnableConsumer = enableConsumer;
        public void WithConnectionString(string connectionString) => ConnectionString = connectionString;
        public void WithReceiveMode(ReceiveMode receiveMode) => ReceiveMode = receiveMode;
        public void WithRetryPolicy(RetryPolicy retryPolicy) => RetryPolicy = retryPolicy;
    }
}
