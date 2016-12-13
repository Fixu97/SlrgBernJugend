using Shared.Models.db;

namespace BusinessLayer.DbHandler
{
    public class DbPermissionHandler : DbObjHandler<PermissionDTO>
    {
        protected override DataLayer.DbHandler.DbObjHandler<PermissionDTO> _db => new DataLayer.DbHandler.PermissionHandler();
    }
}
