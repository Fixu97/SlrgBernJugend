using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace Shared.Models.db
{
    [DataContract]
    public class RelayDTO : DbObjDTO
    {
        [DataMember(Name = "Pk")]
        public string Relay { get; set; }
        [DataMember(Name = "RelaysDisciplines")]
        public List<RelayDisciplineDTO> RelaysDisciplines { get; set; }
        [DataMember(Name = "SortedDisciplines")]
        public List<DisciplineDTO> SortedDisciplines
        {
            get
            {
                if(RelaysDisciplines == null || !RelaysDisciplines.Any())
                {
                    return new List<DisciplineDTO>();
                }

                var sortedRelaysDisciplines = RelaysDisciplines.OrderBy(rd => rd.Position);
                var sortedDisciplines = sortedRelaysDisciplines.Select( rd => rd.Discipline).ToList();

                return sortedDisciplines;
            }
        }
    }
}
