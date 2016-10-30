using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using Shared.Models;
using Shared.Models.db;

namespace DataLayer.Models
{
    internal class Time : DbObj
    {

        #region Private properties

        #endregion

        #region Public properties

        public Person Person;
        public Discipline Discipline;
        public string DiscplayTime
        {
            get { return Seconds + "s"; }
        }

        public TimeDTO Dto
        {
            get
            {
                return new TimeDTO()
                {
                    Pk = this.Pk,
                    FK_P = this.FK_P,
                    FK_D = this.FK_D,
                    Seconds = this.Seconds,
                    Date = this.Date,
                    Person = this.Person.Dto,
                    Discipline = this.Discipline.Dto
                };
            }
        }

        #region Database properties

        public int FK_P {
            get { return this.Person.Pk;}
            set
            {
                if (value > 0)
                    this.Person = new Person(value);
            }
        }
        public int FK_D
        {
            get { return this.Discipline.Pk; }
            set
            {
                if (value > 0)
                this.Discipline = new Discipline(value); }
        }
        public decimal Seconds;
        public DateTime Date;
        #endregion

        #endregion

        #region Constructors
        public Time(int pk)
        {
            base.Pk = pk;
            this.Select();
        }

        public Time(int pk, int fk_p, int fk_d, decimal seconds, DateTime date)
        {
            base.Pk = pk;

            this.FK_P = fk_p;
            this.FK_D = fk_d;
            this.Seconds = seconds;
            this.Date = date;
        }
        public Time(int fk_p,int fk_d, decimal seconds, DateTime date)
        {
            this.FK_P = fk_p;
            this.FK_D = fk_d;
            this.Seconds = seconds;
            this.Date = date;

        }
        public Time(Person person, Discipline discipline, decimal seconds, DateTime date)
        {
            this.FK_P = person.Pk;
            this.FK_D = discipline.Pk;
            this.Seconds = seconds;
            this.Date = date;

        }

        public Time(TimeDTO timeDto)
        {
            Pk = timeDto.Pk;
            FK_P = timeDto.FK_P;
            FK_D = timeDto.FK_D;
            Seconds = timeDto.Seconds;
            Date = timeDto.Date;
        }

        #endregion

        #region Implemented Methods

        internal override List<string> _attributes { get {return new List<string>() {"FK_P","FK_D","Seconds","Date"};} }
        internal override List<string> _values { get {return new List<string>() {FK_P.ToString(),FK_D.ToString(),Seconds.ToString(),Date.ToString("yyyy-MM-dd") };} }
        internal override string _tableName { get { return "TIMES"; } }

        protected override void Select()
        {
            var dto = DataLayer.GetTimeByPk(base.Pk);

            this.FK_P = dto.FK_P;
            this.FK_D = dto.FK_D;
            this.Seconds = dto.Seconds;
            this.Date = dto.Date;
        }

        public override void Insert()
        {
            base.InsertDefault();
        }

        #endregion
    }
}
