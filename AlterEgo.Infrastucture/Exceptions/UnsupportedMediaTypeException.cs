using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlterEgo.Infrastructure.Exceptions
{
    public class UnsupportedMediaTypeException : ApplicationException
    {
        public UnsupportedMediaTypeException(string type) : base($"Mediatype {type} is not supported in this context")
        {
        }
    }
}
