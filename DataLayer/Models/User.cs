using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Shared;
using Shared.Models;
using Shared.Models.db;

namespace DataLayer.Models
{
    class User : DbObj
    {

        #region Public properties

        public string Username;
        public string Password;
        public string Salt;
        public string Email;
        public bool Admin;

        public UserDTO Dto
        {
            get
            {
                return new UserDTO()
                {
                    Pk = base.Pk,
                    Username = this.Username,
                    Password = this.Password,
                    Salt = this.Salt,
                    Email = this.Email,
                    Admin = this.Admin
                };
            }
        }

        #endregion

        #region Constructors

        public User(int pk)
        {
            base.Pk = pk;
            this.Select();
        }

        public User(int pk, string username, string password, string email, bool admin)
        {
            base.Pk = pk;
            this.Username = username;
            this.Password = password;
            this.Email = email;
            this.Admin = admin;
        }

        public User(string username, string password, string email, bool admin)
        {
            this.Username = username;
            this.Password = password;
            this.Email = email;
            this.Admin = admin;
        }

        public User(int pk, string username, string password, string salt, string email, bool admin)
        {
            base.Pk = pk;
            this.Username = username;
            this.Password = password;
            this.Salt = salt;
            this.Email = email;
            this.Admin = admin;
        }

        public User(UserDTO userDto)
        {
            Pk = userDto.Pk;
            Username = userDto.Username;
            Password = userDto.Password;
            Salt = userDto.Salt;
            Email = userDto.Email;
            Admin = userDto.Admin;
        }

        #endregion

        #region Implemented methods

        internal override List<string> _attributes { get { return new List<string>() {"Username", "Password", "Salt", "Email", "Admin"}; }}
        internal override List<string> _values { get { return new List<string>() {Username,Password,Salt,Email,Admin.ToString()}; }}
        internal override string _tableName { get { return "USERS"; } }

        protected override void Select()
        {
            var dto = DataLayer.GetUserByPk(this.Pk);

            this.Username = dto.Username;
            this.Password = dto.Password;
            this.Salt = dto.Salt;
            this.Email = dto.Email;
            this.Admin = dto.Admin;
        }

        public override void Insert()
        {
            var existingUserWithUsername = DataLayer.GetAllUsers().FirstOrDefault(u => u.Username == this.Username);

            if (existingUserWithUsername != null)
            {
                throw new InvalidDataException("User with this username already exists!");
            }

            if (Admin)
            {
                string paramList = "";

                for (var i = 0; i < _values.Count - 1; i++)
                {
                    paramList += "@var" + i + ",";
                }
                paramList = paramList.Substring(0, paramList.Length - 1) + ";";
                _db.ExecuteQueryParameterized("EXECUTE SLRG_BERN_JUGEND.dbo.InsertAdminUser " + paramList, _values.Where(value => value != "Admin").ToList());
            }
            else
            {
                _db.Insert(this);
            }
        }
        #endregion

        #region Public Methods

        public virtual new void Update()
        {
            if (!string.IsNullOrWhiteSpace(Password) && !string.IsNullOrWhiteSpace(Salt))
            {
                _db.Update(this);
            }

            var _values = new List<string>() { Username, Email, Admin.ToString() };
            var cmd = "UPDATE USERS SET Username = @var0, Email = @var1, Admin = @var2 WHERE PK = " + Pk;

            _db.ExecuteQueryParameterized(cmd, _values);
        }

        #endregion
    }
}
