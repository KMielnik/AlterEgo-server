using System;

namespace AlterEgo.Infrastucture.Exceptions
{
    public class ProcessingAnimationFailedException : ApplicationException
    {
        public string InvalidJsonString { get; private set; }
        public string Filename { get; private set; }
        public ProcessingAnimationFailedException(string message, Exception innerException, string invalidJsonString) : base(message, innerException)
        {
            InvalidJsonString = invalidJsonString;
        }

        public ProcessingAnimationFailedException(string message) : base(message)
        {
        }

        public ProcessingAnimationFailedException(string message, string filename) : base(message)
        {
            Filename = filename;
        }
    }
}
