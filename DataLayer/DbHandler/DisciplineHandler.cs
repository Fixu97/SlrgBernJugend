using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using Shared.Models.db;

namespace DataLayer.DbHandler
{
    public class DisciplineHandler : DbObjHandler<DisciplineDTO>
    {
        protected override string TableName => "DISCIPLINES";
        protected override string SelectStatement => $"SELECT * FROM {TableName} ";
        protected override string OrderBy => string.Empty;

        protected override Dictionary<string, string> GetAttributeValuePairs(DisciplineDTO dbObj)
        {
            var discipline = (DisciplineDTO)dbObj;
            var dictionary = new Dictionary<string, string>();

            dictionary.Add("Discipline", discipline.DiscName);
            dictionary.Add("Meters", discipline.Meters.ToString());

            return dictionary;
        }

        protected override List<DisciplineDTO> ReadParamterized(string cmd, List<string> parameters)
        {

            var disciplines = new List<DisciplineDTO>();

            using (SqlConnection con = new SqlConnection(_readerConnectionString))
            {
                try
                {
                    var query = new SqlCommand(cmd, con);
                    AddParameters(query, parameters);
                    con.Open();

                    using (var sqlReader = query.ExecuteReader())
                    {
                        DisciplineDTO tmpDiscipline;
                        while (sqlReader.Read())
                        {
                            tmpDiscipline = GetDisciplineDto
                            (
                                sqlReader.GetInt32(0),
                                sqlReader.GetString(1),
                                sqlReader.GetInt32(2)
                            );
                            disciplines.Add((DisciplineDTO)tmpDiscipline);
                        }
                    }
                }
                catch (SqlException e)
                {
                    throw new Exception(e.Message);
                }
                catch (Exception e)
                {
                    throw new Exception(e.Message);
                }

            }

            return disciplines;
        }

        public DisciplineDTO GetDisciplineDto(int pk, string discName, int meters)
        {
            var discipline = new DisciplineDTO
            {
                Pk = pk,
                DiscName = discName,
                Meters = meters
            };
            return discipline;
        }
    }
}
