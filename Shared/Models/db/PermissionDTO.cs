using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Shared.Models.db
{
    [DataContract(Name = "PermissionDTO")]
    public class PermissionDTO : DbObjDTO
    {

        [DataMember(Name = "User")]
        public UserDTO User;
        [DataMember(Name = "Person")]
        public PersonDTO Person;

        #region Database properties

        [DataMember(Name = "FK_U")]
        public int FK_U;
        [DataMember(Name = "FK_P")]
        public int FK_P;

        #endregion
    }
}
