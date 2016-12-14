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
