using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using Shared.Models;
using Shared.Models.db;

namespace DataLayer.Models
{
    class Permission : DbObj
    {

        #region Public properties

        public User User;
        public Person Person;

        #region Database properties

        public int FK_U
        {
            get { return this.User.Pk; }
            set
            {
                if (value > 0) 
                    this.User = new User(value);
            }
        }
        public int FK_P
        {
            get { return this.Person.Pk; }
            set
            {
                if (value > 0) 
                    this.Person = new Person(value);
            }
        }

        #endregion

        public PermissionDTO Dto
        {
            get {
                return new PermissionDTO()
                {
                    Pk = base.Pk,
                    FK_U = this.FK_U,
                    FK_P = this.FK_P,
                    User = this.User.Dto,
                    Person = this.Person.Dto
                };
            }
        }

        #endregion

        #region Constructors

        public Permission(int pk)
        {
            base.Pk = pk;
            this.Select();
        }

        public Permission(int pk, int fk_u, int fk_p)
        {
            Pk = pk;
            FK_P = fk_p;
            FK_U = fk_u;
        }

        public Permission(int fk_u, int fk_p)
        {
            FK_P = fk_p;
            FK_U = fk_u;
        }

        public Permission(User user, Person person)
        {
            User = user;
            Person = person;
        }

        public Permission(PermissionDTO permissionDto)
        {
            Pk = permissionDto.Pk;
            FK_P = permissionDto.FK_P;
            FK_U = permissionDto.FK_U;
        }

        #endregion

        #region Implemented methods

        internal override List<string> _attributes { get {return new List<string>() {"FK_U","FK_P"};} }
        internal override List<string> _values { get {return new List<string>() {FK_U.ToString(),FK_P.ToString()};} }
        internal override string _tableName { get { return "PERMISSIONS"; } }
        public override void Insert()
        {
            base.InsertDefault();
        }

        protected override void Select()
        {
            var dto = DataLayer.GetPermissionByPk(base.Pk);

            this.FK_U = dto.FK_U;
            this.FK_P = dto.FK_P;
        }

        #endregion
    }
}
