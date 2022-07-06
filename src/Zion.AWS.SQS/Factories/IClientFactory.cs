using Amazon.SQS;

namespace Zion.AWS.SQS.Factories
{
    internal interface IClientFactory
    {
        public IAmazonSQS Create();
    }
}
