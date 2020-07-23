using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace AlterEgo.Infrastucture.Exceptions
{
    public class ProcessingAnimationFailedException : ApplicationException
    {
        public string InvalidJsonString { get; private set; }
        public ProcessingAnimationFailedException(string message, Exception innerException, string invalidJsonString) : base(message, innerException)
        {
            InvalidJsonString = invalidJsonString;
        }

        protected ProcessingAnimationFailedException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
