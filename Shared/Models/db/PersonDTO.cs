using System;
using System.Runtime.Serialization;

namespace Shared.Models.db
{
    [DataContract(Name = "PersonDTO")]
    public class PersonDTO : DbObjDTO
    {
        #region Private properties

        #endregion

        #region Public properties

        [DataMember(Name = "Prename")]
        public string Prename;
        [DataMember(Name = "LastName")]
        public string LastName;
        [DataMember(Name = "DisplayName")]
        public string DisplayName
        {
            get { return Prename + (LastName == null ? "" : " " + LastName.Substring(0, 1) + "."); }
        }
        [DataMember(Name = "Birthday")]
        public DateTime Birthday;
        [DataMember(Name = "Age")]
        public int Age
        {
            get { return Convert.ToInt32(Math.Floor(DateTime.Now.Subtract(this.Birthday).TotalDays / 365)); }
        }
        [DataMember(Name = "Male")]
        public bool Male;
        [DataMember(Name = "Active")]
        public bool Active;
        [DataMember(Name = "PhoneNr")]
        public string PhoneNr;
        [DataMember(Name = "Email")]
        public string Email;

        #endregion

    }
}
