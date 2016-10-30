using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Data.SqlClient;
using DataLayer.DbHandler;
using Shared.Models;

namespace DataLayer.Models
{
    internal abstract class DbObj
    {

        #region Protected members

        protected bool _changesMade = false;
        internal abstract List<string> _attributes { get; }
        internal abstract List<string> _values { get; }
        internal abstract string _tableName { get; }
        protected DbObjHandler _db = new DbObjHandler();
        protected ZeitenanalyseDataLayer DataLayer = new ZeitenanalyseDataLayer();

        #endregion

        public int Pk = 0;
        protected abstract void Select();
        public abstract void Insert();

        public void Update()
        {
            _db.Update(this);
        }

        public void Delete()
        {
            _db.Delete(this);
        }

        protected void InsertDefault()
        {
            _db.Insert(this);
        }
    }
}
