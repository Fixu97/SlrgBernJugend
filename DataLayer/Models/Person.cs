using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using Shared;
using Shared.Models;
using Shared.Models.db;

namespace DataLayer.Models
{
    internal class Person : DbObj
    {
        #region Public Properties

        public string Prename;
        public string LastName;
        public string DisplayName
        {
            get { return Prename + " " + LastName.Substring(0, 1) + "."; }
        }
        public DateTime Birthday;
        public int Age
        {
            get {return Convert.ToInt32(Math.Floor(DateTime.Now.Subtract(this.Birthday).TotalDays/365)); }
        }
        public bool Male;
        public bool Active;
        public string PhoneNr;
        public string Email;

        public PersonDTO Dto
        {
            get
            {
                return new PersonDTO()
                {
                    Pk = base.Pk,
                    Prename = this.Prename,
                    LastName = this.LastName,
                    Birthday = this.Birthday,
                    Active = this.Active,
                    Male = this.Male,
                    PhoneNr = (this.PhoneNr ?? ""),
                    Email = (this.Email ?? "")
                };
            }
        }

        #endregion

        #region Constructors
        public Person(int Pk)
        {
            base.Pk = Pk;

            this.Select();
        }
        public Person(string prename, string lastName, DateTime birthday, bool male, bool active, string phoneNr, string email)
        {
            this.Prename = prename;
            this.LastName = lastName;
            this.Birthday = birthday;
            this.Male = male;
            this.Active = active;

            this.PhoneNr = phoneNr;
            this.Email = email;
        }
        public Person(int pK, string prename, string lastName, DateTime birthday, bool male, bool active, string phoneNr, string email)
        {
            base.Pk = pK;

            this.Prename = prename;
            this.LastName = lastName;
            this.Birthday = birthday;
            this.Male = male;
            this.Active = active;
            this.PhoneNr = phoneNr;
            this.Email = email;
        }

        public Person(PersonDTO personDto)
        {
            Pk = personDto.Pk;
            Prename = personDto.Prename;
            LastName = personDto.LastName;
            Birthday = personDto.Birthday;
            Male = personDto.Male;
            Active = personDto.Active;
            PhoneNr = personDto.PhoneNr;
            Email = personDto.Email;
        }

        #endregion

        #region Implemented Methods

        internal override List<string> _attributes { get {return new List<string>() {"Prename","Lastname","Birthday","Male","Active","PhoneNr","Email"};} }
        internal override List<string> _values { get {return new List<string>() {Prename,LastName,Birthday.ToString("yyyy-MM-dd"),Male.ToString(),Active.ToString(),PhoneNr,Email};} }
        internal override string _tableName { get { return "PEOPLE"; } }

        protected override void Select()
        {
            var dto = DataLayer.GetPersonByPk(base.Pk);

            this.Prename = dto.Prename;
            this.LastName = dto.LastName;
            this.Birthday = dto.Birthday;
            this.Male = dto.Male;
            this.Active = dto.Active;
            this.PhoneNr = dto.PhoneNr;
            this.Email = dto.Email;
        }

        public override void Insert()
        {
            var values = new List<string>(_values);
            values.Add(SessionFacade.User.Pk.ToString()); // So the Fetch can be made.
            string paramList = "";

            for (var i = 0; i < values.Count; i++)
            {
                paramList += "@var" + i + ",";
            }
            paramList = paramList.Substring(0, paramList.Length - 1) + ";";
            _db.ExecuteQueryParameterized("EXECUTE SLRG_BERN_JUGEND.dbo.InsertPerson " + paramList, values);
        }

        #endregion
    }
}
