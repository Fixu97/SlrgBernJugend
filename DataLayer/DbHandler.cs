using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataLayer.Models;
using Shared.Exceptions;

namespace DataLayer
{
    class DbHandler
    {
        #region private static Members

        private readonly string _readerConnectionString = @"Server=localhost\SQLEXPRESS;Database=SLRG_BERN_JUGEND;User Id=db_read;Password=read-3087";
        private readonly string _writerConnectionString = @"Server=localhost\SQLEXPRESS;Database=SLRG_BERN_JUGEND;User Id=db_write;Password=write-3087";

        #endregion

        internal void Update(DbObj dbObj)
        {
            var attributes = dbObj._attributes;
            var values = dbObj._values;
            var tableName = dbObj._tableName;
            var pk = dbObj.Pk;

            if (pk < 1)
            {
                throw new InvalidDataException("You must provide a primary key that is greater than zero!");
            }
            if (attributes.Count != values.Count)
            {
                throw new InvalidDataException("The number of attributes does not match the number of values!");
            }
            if (values.Count == 0)
            {
                throw new InvalidDataException("You must provide at least one attribute and value!");
            }

            int ubValues = values.Count - 1;
            string cmd = "UPDATE " + tableName + " SET \n";
            List<string> parameters = new List<string>();

            for (int i = 0; i <= ubValues; i++) { cmd += attributes[i] + " = @var" + i + ",\n"; }
            cmd = cmd.Substring(0, cmd.Length - 2) + "\n WHERE PK = " + pk + ";";
            for (int i = 0; i <= ubValues; i++) { parameters.Add(values[i]); }

            ExecuteQueryParameterized(cmd, parameters);
        }
        internal void Insert(DbObj dbObj)
        {
            var attributes = dbObj._attributes;
            var values = dbObj._values;
            var tableName = dbObj._tableName;

            if (attributes.Count != values.Count)
            {
                throw new InvalidDataException("The number of attributes does not match the number of values!");
            }
            if (values.Count == 0)
            {
                throw new InvalidDataException("You must provide at least one attribute and value!");
            }

            string valueRow = " VALUES (";
            string cmd = "INSERT INTO  " + tableName + "\n (";
            List<string> parameters = new List<string>();
            int ubValues = values.Count - 1;

            for (int i = 0; i <= ubValues; i++)
            {
                cmd += attributes[i] + ",";
                valueRow += "@var" + i + ",";
            }
            cmd = cmd.Substring(0, cmd.Length - 1) + ")\n" + valueRow.Substring(0, valueRow.Length - 1) + ");";
            for (int i = 0; i <= ubValues; i++) { parameters.Add(values[i]); }

            ExecuteQueryParameterized(cmd, parameters);
        }
        internal void Delete(DbObj dbObj)
        {
            var tableName = dbObj._tableName;
            var pk = dbObj.Pk;

            string cmd = "DELETE FROM " + tableName + " WHERE PK = @var0;";
            ExecuteQueryParameterized(cmd, new List<string>() {pk.ToString()});
        }
        internal void ExecuteQueryParameterized(string cmd, List<string> parameters)
        {

            var paramCount = parameters.Count;

            using (SqlConnection con = new SqlConnection(_writerConnectionString))
            {
                try
                {
                        var query = new SqlCommand(cmd, con);
                    for (int i = 0; i < paramCount; i++)
                    {
                        if (string.IsNullOrEmpty(parameters[i]))
                        {

                            query.Parameters.AddWithValue("@var" + i, DBNull.Value);
                            continue;
                        }
                        query.Parameters.AddWithValue("@var" + i, parameters[i]);
                    }
                        con.Open();
                        query.ExecuteNonQuery();
                }
                catch (SqlException e)
                {
                    throw new DbException("An error occoured while executing query on db with parameters!",e,cmd, parameters, e.Message);
                }
                catch (Exception e)
                {
                    throw new ZeitenanalyseException(e.Message, e);
                }
            }
        }
    }
}
