using Amazon.SQS;
using Microsoft.Extensions.Options;

namespace Zion.AWS.SQS.Factories
{
    internal sealed class ClientFactory : IClientFactory
    {
        private readonly IOptionsMonitor<SQSOptions> _optionsMonitor;

        public ClientFactory(IOptionsMonitor<SQSOptions> optionsMonitor)
        {
            if (optionsMonitor is null)
                throw new ArgumentNullException(nameof(optionsMonitor));

            _optionsMonitor = optionsMonitor;
        }

        public IAmazonSQS Create()
            => new AmazonSQSClient(_optionsMonitor.CurrentValue.AWSKey, _optionsMonitor.CurrentValue.AWSKey);
    }
}
