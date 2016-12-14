using Shared.Models.db;
using System;
using System.Collections.Generic;

namespace BusinessLayer.DbHandler
{
    public abstract class DbObjHandler<T> : IDbHandler<T> where T : DbObjDTO
    {
        protected abstract DataLayer.DbHandler.DbObjHandler<T> _db { get; }

        public virtual List<T> GetAll()
        {
            return _db.GetAll();
        }

        public virtual T Select(int pk)
        {
            return _db.Select(pk);
        }

        public virtual void Insert(T dbObj)
        {
            _db.Insert(dbObj);
        }

        public virtual void Insert(List<T> dbObj)
        {
            throw new NotImplementedException();
        }

        public virtual void Update(T dbObj)
        {
            _db.Update(dbObj);
        }

        public virtual void Update(List<T> dbObj)
        {
            throw new NotImplementedException();
        }

        public virtual void Delete(T dbObj)
        {
            _db.Delete(dbObj);
        }

        public virtual void Delete(List<T> dbObj)
        {
            throw new NotImplementedException();
        }
    }
}
