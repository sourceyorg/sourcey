using System.Runtime.Serialization;

namespace Sourcey.Azure.Files
{
    [Serializable]
    internal class UnableToFindBlobException : Exception
    {
        public UnableToFindBlobException()
        {
        }

        public UnableToFindBlobException(string? message) : base(message)
        {
        }

        public UnableToFindBlobException(string? message, Exception? innerException) : base(message, innerException)
        {
        }

        protected UnableToFindBlobException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}