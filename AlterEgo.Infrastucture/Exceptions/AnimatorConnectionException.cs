using System;

namespace AlterEgo.Infrastucture.Exceptions
{
    public class AnimatorConnectionException : ApplicationException
    {
        public AnimatorConnectionException(string message) : base(message)
        {
        }
    }
}
