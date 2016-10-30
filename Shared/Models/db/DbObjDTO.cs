using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Shared.Models.db
{
    [DataContract(Name = "DbObjDTO")]
    public class DbObjDTO
    {

        [DataMember(Name="Pk")]
        public int Pk = 0;
        public bool IsSet()
        {
            return Pk > 0;
        }
    }
}
