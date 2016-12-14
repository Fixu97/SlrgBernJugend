using System.Runtime.Serialization;

namespace Shared.Models.db
{
    [DataContract(Name = "DisciplineDTO")]
    public class DisciplineDTO : DbObjDTO
    {
        #region Public Properties

        [DataMember(Name = "DiscName")]
        public string DiscName;
        [DataMember(Name = "Meters")]
        public int Meters;
        [DataMember(Name = "DisplayName")]
        public string DisplayName
        {
            get { return DiscName + " " + Meters.ToString() + "m";}
        }

        #endregion
    }
}
