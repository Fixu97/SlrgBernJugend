using Shared.Models.db;

namespace BusinessLayer.DbHandler
{
    public class DbPermissionHandler<T> : DbObjHandler<T> where T : PermissionDTO
    {
        protected override DataLayer.DbHandler.DbObjHandler<T> _db => new DataLayer.DbHandler.PermissionHandler<T>();
    }
}
