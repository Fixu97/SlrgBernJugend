using System.Runtime.Serialization;

namespace Shared.Models.db
{
    [DataContract(Name = "UserDTO")]
    public class UserDTO : DbObjDTO
    {

        #region Public properties

        [DataMember(Name = "Username")]
        public string Username;
        [DataMember(Name = "Password")]
        public string Password;
        [DataMember(Name = "Email")]
        public string Email;
        [DataMember(Name = "Admin")]
        public bool Admin;
        [DataMember(Name = "Salt")]
        public string Salt;

        #endregion
    }
}
