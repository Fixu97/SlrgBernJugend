using System;
using System.Collections.Generic;
using Shared.Models.db;

namespace BusinessLayer.DbHandler
{
    /// <summary>
    /// This interface is used so that the generics derived from <see cref="DbObjHandler{T}"/> 
    /// can be used in a polymorphic way (which would not be possible with abstract classes)
    /// For further information see:
    /// http://stackoverflow.com/questions/12324020/cannot-implicitly-convert-derived-type-to-its-base-generic-type
    /// </summary>
    public interface IDbHandler<T> where T : DbObjDTO
    {

        List<T> GetAll();

        T Select(int pk);

        void Insert(T dbObj);

        void Insert(List<T> dbObj);

        void Update(T dbObj);

        void Update(List<T> dbObj);

        void Delete(T dbObj);

        void Delete(List<T> dbObj);
    }
}
