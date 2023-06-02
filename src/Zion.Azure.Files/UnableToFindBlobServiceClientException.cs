using System.Runtime.Serialization;

namespace Zion.Azure.Files
{
    [Serializable]
    internal class UnableToFindBlobServiceClientException : Exception
    {
        public UnableToFindBlobServiceClientException()
        {
        }

        public UnableToFindBlobServiceClientException(string? message) : base(message)
        {
        }

        public UnableToFindBlobServiceClientException(string? message, Exception? innerException) : base(message, innerException)
        {
        }

        protected UnableToFindBlobServiceClientException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}