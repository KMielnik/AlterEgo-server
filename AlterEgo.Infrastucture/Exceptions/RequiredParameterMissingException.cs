using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace AlterEgo.Infrastucture.Exceptions
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
