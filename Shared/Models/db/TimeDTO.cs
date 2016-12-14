using System;
using System.Runtime.Serialization;

namespace Shared.Models.db
{
    [DataContract(Name = "TimeDTO")]
    public class TimeDTO : DbObjDTO
    {
        #region Public Properties

        [DataMember(Name = "Person")]
        public PersonDTO Person;
        [DataMember(Name = "Discipline")]
        public DisciplineDTO Discipline;

        #region Database properties

        [DataMember(Name = "FK_P")]
        public int FK_P;
        [DataMember(Name = "FK_D")]
        public int FK_D;
        [DataMember(Name = "Seconds")]
        public decimal Seconds;
        [DataMember(Name = "Date")]
        public DateTime Date;
        [DataMember(Name = "DisplayTime")]
        public string DisplayTime
        {
            get { return Seconds + "s"; }
        }
        #endregion

        #endregion
    }
}
