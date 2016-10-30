using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shared.Models.db;

namespace BusinessLayer.DbHandler
{
    public class DbDisciplineHandler<T> : DbObjHandler<T> where T : DisciplineDTO
    {
        protected override DataLayer.DbHandler.DbObjHandler<T> _db => new DataLayer.DbHandler.DisciplineHandler<T>();
    }
}
