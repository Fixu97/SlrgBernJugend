using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shared.Models.db;

namespace BusinessLayer.DbHandler
{
    public class DbRelayDisciplineHandler : DbObjHandler<RelayDisciplineDTO>
    {
        protected override DataLayer.DbHandler.DbObjHandler<RelayDisciplineDTO> _db => new DataLayer.DbHandler.RelayDisciplineHandler();
    }
}
