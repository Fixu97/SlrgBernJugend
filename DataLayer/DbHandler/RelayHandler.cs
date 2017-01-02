using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using Shared.Models.db;

namespace DataLayer.DbHandler
{
    public class RelayHandler : DbObjHandler<RelayDTO>
    {
        protected override string OrderBy => " ORDER BY RELAYS.Relay ASC, RELAYS.PK ASC";

        protected override string SelectStatement => $"SELECT RELAYS.PK, RELAYS.Relay, RD.Position, D.PK, D.Discipline, D.Meters FROM {TableName} "
                                                   + $"LEFT JOIN RELAYS_DISCIPLINES AS RD ON RD.FK_R = RELAYS.PK "
                                                   + $"LEFT JOIN DISCIPLINES AS D ON RD.FK_D = D.PK";

        protected override string TableName => "RELAYS";

        protected override Dictionary<string, string> GetAttributeValuePairs(RelayDTO dbObj)
        {
            var relay = dbObj;
            var dictionary = new Dictionary<string, string>();

            dictionary.Add("Relay", relay.Relay);

            return dictionary;
        }

        protected override List<RelayDTO> ReadParamterized(string cmd, List<string> parameters)
        {

            var relays = new List<RelayDTO>();

            using (SqlConnection con = new SqlConnection(_readerConnectionString))
            {
                try
                {
                    var query = new SqlCommand(cmd, con);
                    AddParameters(query, parameters);
                    con.Open();

                    using (var sqlReader = query.ExecuteReader())
                    {
                        RelayDTO tmpRelay;
                        while (sqlReader.Read())
                        {
                            tmpRelay = GetRelayDto
                            (
                                sqlReader.GetInt32(0),
                                sqlReader.GetString(1)
                            );

                            // Checking if the relay already exists in the list
                            var existingRelay = relays.FirstOrDefault(r => r.Pk == tmpRelay.Pk);

                            // if not, add the current relay to the list
                            if (existingRelay == null)
                            {
                                relays.Add(tmpRelay);
                                existingRelay = tmpRelay;
                            }

                            if (sqlReader.IsDBNull(2) || sqlReader.IsDBNull(3) || sqlReader.IsDBNull(4) || sqlReader.IsDBNull(5))
                            {
                                continue;
                            }

                            // assemble the Discipline object
                            var tmpDiscipline = new DisciplineDTO
                            {
                                Pk = sqlReader.GetInt32(3),
                                DiscName = sqlReader.GetString(4),
                                Meters = sqlReader.GetInt32(5)
                            };

                            // assemble the RelayDiscipline object
                            var tmpRelayDiscipline = new RelayDisciplineDTO
                            {
                                Pk = sqlReader.GetInt32(2),
                                FK_R = existingRelay.Pk,
                                FK_D = tmpDiscipline.Pk,
                                Relay = existingRelay,
                                Discipline = tmpDiscipline
                            };

                            // Add the RelayDiscipline to the relay
                            tmpRelay.RelaysDisciplines = new List<RelayDisciplineDTO>();
                            existingRelay.RelaysDisciplines.Add(tmpRelayDiscipline);
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

        public RelayDTO GetRelayDto(int pk, string relay)
        {
            return new RelayDTO
            {
                Pk = pk,
                Relay = relay
            };
        }
    }
}
