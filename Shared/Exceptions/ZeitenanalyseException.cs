using System;

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
