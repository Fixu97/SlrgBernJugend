using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Exceptions
{
    public class DbException : ZeitenanalyseException
    {
        public string Query;
        public List<string> Parameters; 
        public string DbMessage;

        #region Implemented members
        public DbException(string msg) : base(msg)
        {
        }

        public DbException(string msg, Exception innerException) : base(msg, innerException)
        {
        }
        #endregion

        public DbException(string msg, Exception innerException, string query) : base(msg, innerException)
        {
            Query = query;
        }
        public DbException(string msg, Exception innerException, string query, List<string> parameters) : base(msg, innerException)
        {
            Query = query;
            Parameters = parameters;
        }
        public DbException(string msg, Exception innerException, string query, List<string> parameters, string dbMessage) : base(msg, innerException)
        {
            Query = query;
            Parameters = parameters;
            DbMessage = dbMessage;
        }
    }
}
