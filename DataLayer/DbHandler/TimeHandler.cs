using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using Shared.Models.db;

namespace DataLayer.DbHandler
{
    public class TimeHandler : DbObjHandler<TimeDTO>
    {
        private DisciplineHandler _disciplineHandler = new DisciplineHandler();
        private PersonHandler _personHandler = new PersonHandler();

        public DisciplineHandler DisciplineHandler { protected get { return _disciplineHandler; } set { _disciplineHandler = value; } }
        public PersonHandler PersonHandler { protected get { return _personHandler; } set { _personHandler = value; } }

        private string _activePeopleClause = " P.Active = 1 ";
        protected override string TableName => "TIMES";

        protected override string SelectStatement
            =>
                $"SELECT {TableName}.PK, P.PK, D.PK, {TableName}.Seconds, {TableName}.Date, P.Prename, P.Lastname, P.Birthday, P.Male, P.Active, P.PhoneNr, P.Email, D.Discipline, D.Meters FROM {TableName} " +
                $"JOIN PEOPLE AS P ON P.PK = {TableName}.FK_P " +
                $"JOIN DISCIPLINES AS D ON D.PK = {TableName}.FK_D ";
        protected override string OrderBy => " ORDER BY [Date] ASC, FK_D, FK_P, Seconds";

        public List<TimeDTO> GetAll(bool onlyActive = true)
        {
            var cmd = AssembleQuery($"WHERE {_activePeopleClause}");
            return ReadParamterized(cmd, new List<string>());
        }

        protected override Dictionary<string, string> GetAttributeValuePairs(TimeDTO dbObj)
        {
            var time = dbObj;
            var dictionary = new Dictionary<string, string>();
            
            dictionary.Add("FK_P", time.FK_P.ToString());
            dictionary.Add("FK_D", time.FK_D.ToString());
            dictionary.Add("Seconds", time.Seconds.ToString("##.###"));
            dictionary.Add("Date", time.Date.ToString("yyyy-MM-dd"));

            return dictionary;
        }

        protected override List<TimeDTO> ReadParamterized(string cmd, List<string> parameters)
        {
            var times = new List<TimeDTO>();

            using (SqlConnection con = new SqlConnection(_readerConnectionString))
            {
                try
                {
                    var query = new SqlCommand(cmd, con);
                    AddParameters(query, parameters);

                    con.Open();

                    using (var sqlReader = query.ExecuteReader())
                    {
                        TimeDTO tmpTime;
                        while (sqlReader.Read())
                        {
                            tmpTime = GetTimeDto(
                                sqlReader.GetInt32(0),
                                sqlReader.GetInt32(1),
                                sqlReader.GetInt32(2),
                                sqlReader.GetDecimal(3),
                                sqlReader.GetDateTime(4)
                            );

                            var birthday = sqlReader.IsDBNull(7) ? new DateTime() : sqlReader.GetDateTime(7);
                            var phoneNr = sqlReader.IsDBNull(10) ? "" : sqlReader.GetString(10);
                            var email = sqlReader.IsDBNull(11) ? "" : sqlReader.GetString(11);

                            // Fill up person object
                            tmpTime.Person = PersonHandler.GetPersonDto(
                                tmpTime.FK_P, 
                                sqlReader.GetString(5), 
                                sqlReader.GetString(6),
                                birthday,
                                sqlReader.GetBoolean(8),
                                sqlReader.GetBoolean(9),
                                phoneNr,
                                email
                                );

                            // Fill up discipline object
                            tmpTime.Discipline = DisciplineHandler.GetDisciplineDto(
                                tmpTime.FK_D,
                                sqlReader.GetString(12),
                                sqlReader.GetInt32(13)
                                );

                            times.Add((TimeDTO)tmpTime);
                        }
                    }
                }
                catch (SqlException e)
                {
                    throw;
                }
                catch (Exception e)
                {
                    throw;
                }

            }

            return times;
        }

        public TimeDTO GetTimeDto(int pk, int fk_p, int fk_d, decimal seconds, DateTime date)
        {
            var time = new TimeDTO
            {
                Pk = pk,
                FK_P = fk_p,
                FK_D = fk_d,
                Seconds = seconds,
                Date = date
            };
            return (TimeDTO)time;
        }

        public List<TimeDTO> GetTimesByPeople(List<PersonDTO> personDtos)
        {

            var whereStatement = "WHERE (";
            var parameters = new List<string>();

            for (var i = 0; i < personDtos.Count; i++)
            {
                whereStatement += $"FK_P = @var{i} OR ";
                parameters.Add(personDtos[i].Pk.ToString());
            }

            whereStatement += "FK_P = 0)";

            var cmd = AssembleQuery(whereStatement);

            var times = new List<TimeDTO>();
            var dbObjs = ReadParamterized(cmd, parameters);
            dbObjs.ForEach(TimeDTO => times.Add(TimeDTO));

            return times;


        }
        public List<TimeDTO> GetTimesByPeople(List<PersonDTO> personDtos, DisciplineDTO disciplineDto)
        {

            var whereStatement = "WHERE (";
            var parameters = new List<string>();
            var i = 0;

            for (; i < personDtos.Count; i++)
            {
                whereStatement += $"FK_P = @var{i} OR ";
                parameters.Add(personDtos[i].Pk.ToString());
            }

            whereStatement += $"FK_P = 0) AND FK_D = @var{i}";
            parameters.Add(disciplineDto.Pk.ToString());

            var cmd = AssembleQuery(whereStatement);

            var times = new List<TimeDTO>();
            var dbObjs = ReadParamterized(cmd, parameters);
            dbObjs.ForEach(TimeDTO => times.Add(TimeDTO));

            return times;


        }
        public List<TimeDTO> GetTimesByPeople(List<PersonDTO> personDtos, DisciplineDTO disciplineDto, DateTime[] timeScope)
        {

            var whereStatement = "WHERE (";
            var parameters = new List<string>();
            var i = 0;

            for (; i < personDtos.Count; i++)
            {
                whereStatement += $"FK_P = @var{i} OR ";
                parameters.Add(personDtos[i].Pk.ToString());
            }

            string fromTo = _getTimeScope(timeScope);
            whereStatement += $"FK_P = 0) AND FK_D = @var{i} AND {fromTo} ORDER BY [Date] ASC, FK_D, FK_P, Seconds, PK;";
            parameters.Add(disciplineDto.Pk.ToString());

            var cmd = AssembleQuery(whereStatement);

            var times = new List<TimeDTO>();
            var dbObjs = ReadParamterized(cmd, parameters);
            dbObjs.ForEach(TimeDTO => times.Add(TimeDTO));

            return times;


        }
        public List<TimeDTO> GetTimesByDisciplines(List<DisciplineDTO> disciplineDtos)
        {

            var whereStatement = "WHERE (";
            var parameters = new List<string>();

            for (var i = 0; i < disciplineDtos.Count; i++)
            {
                whereStatement += $"FK_D = @var{i} OR ";
                parameters.Add(disciplineDtos[i].Pk.ToString());
            }
            
            whereStatement += $"FK_D = 0) AND {_activePeopleClause}";

            var cmd = AssembleQuery(whereStatement);

            var times = new List<TimeDTO>();
            var dbObjs = ReadParamterized(cmd, parameters);
            dbObjs.ForEach(TimeDTO => times.Add(TimeDTO));

            return times;
        }
        public List<TimeDTO> GetTimesByDisciplines(List<DisciplineDTO> disciplineDtos, PersonDTO personDto)
        {

            var whereStatement = "WHERE (";
            var parameters = new List<string>();
            var i = 0;

            for (; i < disciplineDtos.Count; i++)
            {
                whereStatement += $"FK_D = @var{i} OR ";
                parameters.Add(disciplineDtos[i].Pk.ToString());
            }

            whereStatement += $"FK_D = 0) AND FK_P = @var{i}";
            parameters.Add(personDto.Pk.ToString());

            var cmd = AssembleQuery(whereStatement);

            var times = new List<TimeDTO>();
            var dbObjs = ReadParamterized(cmd, parameters);
            dbObjs.ForEach(TimeDTO => times.Add(TimeDTO));

            return times;
        }
        public List<TimeDTO> GetTimesByDisciplines(List<DisciplineDTO> disciplineDtos, PersonDTO personDto, DateTime[] timeScope)
        {

            var whereStatement = "WHERE (";
            var parameters = new List<string>();
            var i = 0;

            for (; i < disciplineDtos.Count; i++)
            {
                whereStatement += $"FK_D = @var{i} OR ";
                parameters.Add(disciplineDtos[i].Pk.ToString());
            }

            var fromTo = _getTimeScope(timeScope);
            whereStatement += $"FK_D = 0) AND FK_P = @var{i} AND {fromTo}";
            parameters.Add(personDto.Pk.ToString());

            var cmd = AssembleQuery(whereStatement);

            var times = new List<TimeDTO>();
            var dbObjs = ReadParamterized(cmd, parameters);
            dbObjs.ForEach(TimeDTO => times.Add(TimeDTO));

            return times;
        }

        private string _getTimeScope(DateTime[] timeScope)
        {
            if (timeScope != null)
            {
                if (timeScope.Length == 2)
                {
                    return " [Date] BETWEEN '" + timeScope[0].ToString("yyyy-MM-dd") + "' and '" + timeScope[1].ToString("yyyy-MM-dd") + "' ";
                }
            }
            return " PK <> 0 ";
        }
    }
}
