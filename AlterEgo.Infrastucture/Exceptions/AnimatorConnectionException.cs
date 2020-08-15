using System;

namespace AlterEgo.Infrastructure.Exceptions
{
    public class AnimatorConnectionException : ApplicationException
    {
        public AnimatorConnectionException() : base("Cannot start the animation processing process")
        {
        }
    }
}
