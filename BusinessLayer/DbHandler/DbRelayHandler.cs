using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shared.Models.db;

namespace BusinessLayer.DbHandler
{
    public class DbRelayHandler<T> : DbObjHandler<T> where T : RelayDTO
    {
        protected override DataLayer.DbHandler.DbObjHandler<T> _db => new DataLayer.DbHandler.RelayHandler<T>();
    }
}
