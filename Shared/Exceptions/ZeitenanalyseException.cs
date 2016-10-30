using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Exceptions
{
    public class ZeitenanalyseException :  Exception
    {
        public ZeitenanalyseException(string msg) : base(msg)
        {
        }

        public ZeitenanalyseException(string msg, Exception innerException) : base(msg, innerException)
        {
            
        }
    }
}
