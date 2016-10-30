using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Models.db
{
    public class RelayDisciplineDTO : DbObjDTO
    {
        public int FK_R { get; set; }
        public int FK_D { get; set; }
        public int Position { get; set; }

        public RelayDTO Relay { get; set; }
        public DisciplineDTO Discipline { get; set; }
    }
}
