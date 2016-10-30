using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shared.Models.db;

namespace BusinessLayer.DbHandler
{
    public class DbPersonHandler<T> : DbObjHandler<T> where T : PersonDTO
    {
        protected override DataLayer.DbHandler.DbObjHandler<T> _db => new DataLayer.DbHandler.PersonHandler<T>();

        public List<T> GetAll(bool onlyActive = true)
        {
            var db = (DataLayer.DbHandler.PersonHandler<T>) _db;
            return db.GetAll(onlyActive);
        }
    }
}
