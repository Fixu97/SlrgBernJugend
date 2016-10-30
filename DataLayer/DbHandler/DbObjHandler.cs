using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using Shared.Exceptions;
using Shared.Models.db;

namespace DataLayer.DbHandler
{
    public abstract class DbObjHandler <T> where T : DbObjDTO
    {

        protected readonly string _readerConnectionString = @"Server=localhost\SQLEXPRESS;Database=SLRG_BERN_JUGEND;User Id=db_read;Password=read-3087";
        protected readonly string _writerConnectionString = @"Server=localhost\SQLEXPRESS;Database=SLRG_BERN_JUGEND;User Id=db_write;Password=write-3087";

        protected abstract string TableName { get; }

        /// <summary>
        /// Use this attribute to get all the "joined" values
        /// </summary>
        protected abstract string SelectStatement { get; }
        protected abstract string OrderBy { get; }

        protected virtual string AssembleQuery(string whereStatement)
        {
            return $"{SelectStatement} {whereStatement} {OrderBy};";
        }


        public T Select(int pk)
        {
            var query = AssembleQuery($"WHERE {TableName}.PK = @var0");
            var result = ReadParamterized(query, new List<string> { pk.ToString() });
            return result.FirstOrDefault();
        }
        public List<T> GetAll()
        {
            var result = ReadParamterized(SelectStatement, new List<string>());

            return result.Cast<T>().ToList();
        }

        public virtual void Insert(T dbObj)
        {

            string cmd = "INSERT INTO  " + TableName + " ";

            var attributeRow = " (";
            var valueRow = " VALUES (";

            var dictionary = GetAttributeValuePairs(dbObj);

            for (var i = 0; i < dictionary.Count; i++)
            {
                attributeRow += $"{dictionary.Keys.ElementAt(i)}, ";
                valueRow += $"@var{i}, ";
            }

            attributeRow = attributeRow.Substring(0, attributeRow.Length - 2) + ")";
            valueRow = valueRow.Substring(0, valueRow.Length - 2) + ")";

            cmd += attributeRow + valueRow;

            WriteParameterized(cmd, dictionary.Values.ToList());
        }

        public virtual void Insert(List<T> dbObj)
        {
            throw new NotImplementedException();
        }

        public virtual void Update(T dbObj)
        {

            if(dbObj.Pk == 0)
            {
                throw new InvalidDataException("Pk must not be 0 for the update!");
            }

            string cmd = "UPDATE " + TableName + " SET ";

            var dictionary = GetAttributeValuePairs(dbObj);

            for (var i = 0; i < dictionary.Count; i++)
            {
                cmd += $"{dictionary.Keys.ElementAt(i)} = @var{i}, ";
            }

            // remove whitespace and comma
            cmd = cmd.Substring(0, cmd.Length - 2);

            cmd += $" WHERE PK = {dbObj.Pk};";

            WriteParameterized(cmd, dictionary.Values.ToList());
        }

        public virtual void Update(List<T> dbObj)
        {
            throw new NotImplementedException();
        }

        public virtual void Delete(T dbObj)
        {
            var pk = dbObj.Pk;

            string cmd = $"DELETE FROM {TableName} WHERE PK = @var0;";
            WriteParameterized(cmd, new List<string>() { pk.ToString() });
        }

        public virtual void Delete(List<T> dbObj)
        {
            throw new NotImplementedException();
        }

        protected abstract Dictionary<string, string> GetAttributeValuePairs(T dbObj);

        protected void WriteParameterized(string cmd, List<string> parameters)
        {

            // Check permission
            var user = Shared.SessionFacade.User;
            if (!user.Admin)
            {
                throw new UnauthorizedAccessException($"User: {user.Username} is not allowed to perform this action!");
            }

            using (SqlConnection con = new SqlConnection(_writerConnectionString))
            {
                try
                {
                    var query = new SqlCommand(cmd, con);
                    AddParameters(query, parameters);
                    con.Open();
                    query.ExecuteNonQuery();
                }
                catch (SqlException e)
                {
                    throw new DbException("An error occoured while executing query on db with parameters!", e, cmd, parameters, e.Message);
                }
                catch (Exception e)
                {
                    throw new ZeitenanalyseException(e.Message, e);
                }
            }
        }

        protected abstract List<T> ReadParamterized(string cmd, List<string> parameters);

        protected void AddParameters(SqlCommand query, List<string> parameters)
        {

            for (int i = 0; i < parameters.Count; i++)
            {
                if (string.IsNullOrEmpty(parameters[i]))
                {
                    query.Parameters.AddWithValue("@var" + i, DBNull.Value);
                    continue;
                }
                query.Parameters.AddWithValue("@var" + i, parameters[i]);
            }
        }
    }
}
