using System.Collections.Generic;
using Shared.Models.db;
using DataLayer.DbHandler;

namespace BusinessLayer.DbHandler
{
    public class DbPersonHandler : DbObjHandler<PersonDTO>
    {
        protected override DataLayer.DbHandler.DbObjHandler<PersonDTO> _db => new PersonHandler();

        public List<PersonDTO> GetAll(bool onlyActive = true)
        {
            return ((PersonHandler)_db).GetAll(onlyActive);
        }
    }
}
