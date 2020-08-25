using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlterEgo.Infrastructure.Exceptions
{
    public class OwnerMismatchException : ApplicationException
    {
        public OwnerMismatchException(string message) : base(message)
        {
        }
    }
}
