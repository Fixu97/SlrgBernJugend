using Shared.Models.db;

namespace BusinessLayer.DbHandler
{
    public class DbRelayDisciplineHandler : DbObjHandler<RelayDisciplineDTO>
    {
        protected override DataLayer.DbHandler.DbObjHandler<RelayDisciplineDTO> _db => new DataLayer.DbHandler.RelayDisciplineHandler();
    }
}
