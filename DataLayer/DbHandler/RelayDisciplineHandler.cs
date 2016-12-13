using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using Shared.Models.db;

namespace DataLayer.DbHandler
{
    public class RelayDisciplineHandler : DbObjHandler<RelayDisciplineDTO>
    {
        protected override string OrderBy => string.Empty;

        protected override string SelectStatement => $"SELECT PK, FK_R, FK_D, Position FROM {TableName}";

        protected override string TableName => "RELAYS_DISCIPLINES";

        protected override Dictionary<string, string> GetAttributeValuePairs(RelayDisciplineDTO dbObj)
        {
            var relayDiscipline = dbObj;
            var dictionary = new Dictionary<string, string>();

            dictionary.Add("FK_R", relayDiscipline.FK_R.ToString());
            dictionary.Add("FK_D", relayDiscipline.FK_D.ToString());
            dictionary.Add("Position", relayDiscipline.Position.ToString());

            return dictionary;
        }

        protected override List<RelayDisciplineDTO> ReadParamterized(string cmd, List<string> parameters)
        {

            var relays = new List<RelayDisciplineDTO>();

            using (SqlConnection con = new SqlConnection(_readerConnectionString))
            {
                try
                {
                    var query = new SqlCommand(cmd, con);
                    AddParameters(query, parameters);
                    con.Open();

                    using (var sqlReader = query.ExecuteReader())
                    {
                        RelayDisciplineDTO tmpRelayDiscipline;
                        while (sqlReader.Read())
                        {
                            tmpRelayDiscipline = GetRelayDisciplineDto
                            (
                                sqlReader.GetInt32(0),
                                sqlReader.GetInt32(1),
                                sqlReader.GetInt32(2),
                                sqlReader.GetInt32(3)
                            );
                            relays.Add((RelayDisciplineDTO)tmpRelayDiscipline);
                        }
                    }
                }
                catch (SqlException e)
                {
                    throw new Exception(e.Message);
                }
                catch (Exception e)
                {
                    throw;
                }

            }

            return relays;
        }

        /// <summary>
        /// Gets all the RelayDisciplineDTO's linked with the given RelayDTO
        /// </summary>
        /// <param name="relay"></param>
        /// <returns></returns>
        public List<RelayDisciplineDTO> Select(RelayDTO relay)
        {
            var cmd = SelectStatement + " WHERE FK_R=@var0";
            var parameters = new List<string> { relay.Pk.ToString() };
            var relayDisciplines = ReadParamterized(cmd, parameters);

            relayDisciplines.ForEach(rd => rd.Relay = relay); // make sure the correct object reference is used
            return relayDisciplines;
        }

        public RelayDisciplineDTO GetRelayDisciplineDto(int pk, int fk_r, int fk_d, int position)
        {
            var relayHandler = new RelayHandler();
            var disciplineHandler = new DisciplineHandler();

            var relay = relayHandler.Select(fk_r);
            var discipline = disciplineHandler.Select(fk_d);

            return new RelayDisciplineDTO
            {
                Pk = pk,
                FK_R = fk_r,
                FK_D = fk_d,
                Position = position,
                Relay = relay,
                Discipline = discipline
            };
        }
        public RelayDisciplineDTO GetRelayDisciplineDto(int pk, RelayDTO relay, int fk_d, int position)
        {
            var disciplineHandler = new DisciplineHandler();
            var discipline = disciplineHandler.Select(fk_d);

            return new RelayDisciplineDTO
            {
                Pk = pk,
                FK_R = relay.Pk,
                FK_D = fk_d,
                Position = position,
                Relay = relay,
                Discipline = discipline
            };
        }
    }
}
