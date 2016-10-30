using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using Shared.Models;
using Shared.Models.db;

namespace DataLayer.Models
{
    internal class Discipline : DbObj
    {
        #region Public Properties

        public string DiscName;
        public int Meters;
        public string DisplayName 
        {
            get { return DiscName + " " + Meters + "m"; }
        }
        public DisciplineDTO Dto
        {
            get
            {
                return new DisciplineDTO()
                {
                    Pk = base.Pk, 
                    DiscName = this.DiscName, 
                    Meters = this.Meters
                };
            }
        }

        #endregion

        #region Constructors
        public Discipline(int pk)
        {
            base.Pk = pk;

            this.Select();
        }
        public Discipline(string discName, int meters)
        {
            this.DiscName = discName;
            this.Meters = meters;

        }
        public Discipline(int pK, string discName, int meters)
        {
            base.Pk = pK;

            this.DiscName = discName;
            this.Meters = meters;
        }

        public Discipline(DisciplineDTO disciplineDto)
        {
            Pk = disciplineDto.Pk;
            DiscName = disciplineDto.DiscName;
            Meters = disciplineDto.Meters;
        }

        #endregion

        #region Implemented Methods

        internal override List<string> _attributes { get {return new List<string>() {"Discipline","Meters"};} }
        internal override List<string> _values { get {return new List<string>() {this.DiscName,this.Meters.ToString()};} }
        internal override string _tableName { get { return "DISCIPLINES"; } }
        public override void Insert()
        {
            base.InsertDefault();
        }

        protected override void Select()
        {
            var dto = DataLayer.GetDisciplineByPk(base.Pk);

            this.DiscName = dto.DiscName;
            this.Meters = dto.Meters;
        }

        #endregion
    }
}
