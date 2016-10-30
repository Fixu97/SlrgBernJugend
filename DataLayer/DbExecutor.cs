using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shared.Exceptions;
using Shared.Models.db;

namespace DataLayer
{
    internal class DbExecutor
    {

        private readonly string _readerConnectionString = @"Server=localhost\SQLEXPRESS;Database=SLRG_BERN_JUGEND;User Id=db_read;Password=read-3087";
        private readonly string _writerConnectionString = @"Server=localhost\SQLEXPRESS;Database=SLRG_BERN_JUGEND;User Id=db_write;Password=write-3087";

        internal void WriteParameterized(string cmd, List<string> parameters)
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
                    throw new DbException("An error occoured while executing query on db with parameters!", e, cmd, parameters, e.Message);
                }
                catch (Exception e)
                {
                    throw new ZeitenanalyseException(e.Message, e);
                }
            }
        }

        internal List<DbObjDTO> ReadParameterized(string cmd, List<string> parameters)
        {
            throw new NotImplementedException();
        }
    }
}
