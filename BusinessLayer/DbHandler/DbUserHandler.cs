using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shared.Models.db;

namespace BusinessLayer.DbHandler
{
    public class DbUserHandler<T> : DbObjHandler<T> where T : UserDTO
    {
        protected override DataLayer.DbHandler.DbObjHandler<T> _db => new DataLayer.DbHandler.UserHandler<T>();
        public UserDTO GetUserByCredentials(string username, string password)
        {
            var db = (DataLayer.DbHandler.UserHandler<T>) _db;
            return db.GetUserByCredentials(username, password);
        }
    }
}
