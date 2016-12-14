using Shared.Models.db;

namespace BusinessLayer.DbHandler
{
    public class DbRelayHandler : DbObjHandler<RelayDTO>
    {
        protected override DataLayer.DbHandler.DbObjHandler<RelayDTO> _db => new DataLayer.DbHandler.RelayHandler();
    }
}
