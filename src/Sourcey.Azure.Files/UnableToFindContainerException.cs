using System.Runtime.Serialization;

namespace Sourcey.Azure.Files
{
    [Serializable]
    internal class UnableToFindContainerException : Exception
    {
        public UnableToFindContainerException()
        {
        }

        public UnableToFindContainerException(string? message) : base(message)
        {
        }

        public UnableToFindContainerException(string? message, Exception? innerException) : base(message, innerException)
        {
        }

        protected UnableToFindContainerException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}