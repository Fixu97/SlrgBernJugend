using Shared.Models.db;

namespace BusinessLayer.DbHandler
{
    public class DbUserHandler : DbObjHandler<UserDTO>
    {
        protected override DataLayer.DbHandler.DbObjHandler<UserDTO> _db => new DataLayer.DbHandler.UserHandler();
        public UserDTO GetUserByCredentials(string username, string password)
        {
            var db = (DataLayer.DbHandler.UserHandler) _db;
            return db.GetUserByCredentials(username, password);
        }
    }
}
