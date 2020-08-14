using System;

namespace AlterEgo.Infrastucture.Exceptions
{
    public class AnimatorConnectionException : ApplicationException
    {
        public AnimatorConnectionException() : base("Cannot start the animation processing process")
        {
        }
    }
}
