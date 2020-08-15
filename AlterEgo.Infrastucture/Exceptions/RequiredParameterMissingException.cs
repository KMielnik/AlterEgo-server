using System;
using System.Runtime.Serialization;

namespace AlterEgo.Infrastructure.Exceptions
{
    public class RequiredParameterMissingException : ApplicationException
    {
        public RequiredParameterMissingException()
        {
        }

        public RequiredParameterMissingException(string message) : base(message)
        {
        }

        public RequiredParameterMissingException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected RequiredParameterMissingException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
