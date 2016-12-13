using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shared;
using Shared.Models.db;

namespace DataLayer.DbHandler
{
    public class PersonHandler : DbObjHandler<PersonDTO>
    {
        private bool _onlyActive = true;
        protected override string TableName => "PEOPLE";
        protected override string SelectStatement => $"SELECT * FROM {TableName} ";
        protected override string OrderBy => "ORDER BY Prename ASC, Lastname ASC";

        public override void Insert(PersonDTO dbObj)
        {
            string paramList = "";

            var dictionary = GetAttributeValuePairs(dbObj);

            for (var i = 0; i < dictionary.Count; i++)
            {
                paramList += "@var" + i + ",";
            }
            paramList = paramList.Substring(0, paramList.Length - 1) + ";";
            WriteParameterized("EXECUTE SLRG_BERN_JUGEND.dbo.InsertPerson " + paramList, dictionary.Values.ToList());
        }

        /// <summary>
        /// This method fetches all people, including the inactive, if onlyActive == false
        /// </summary>
        /// <param name="onlyActive">True if only the active, false if all</param>
        /// <returns>A list of all people</returns>
        public List<PersonDTO> GetAll(bool onlyActive = true)
        {
            _onlyActive = onlyActive;
            var people = base.GetAll();
            _onlyActive = true;
            return people;
        } 

        protected override Dictionary<string, string> GetAttributeValuePairs(PersonDTO dbObj)
        {
            var person = dbObj;
            var dictionary = new Dictionary<string, string>();

            dictionary.Add("Prename", person.Prename);
            dictionary.Add("Lastname", person.LastName);
            dictionary.Add("Birthday", person.Birthday.ToString("yyyy-MM-dd"));
            dictionary.Add("Male", person.Male.ToString());
            dictionary.Add("Active", person.Active.ToString());
            dictionary.Add("PhoneNr", person.PhoneNr);
            dictionary.Add("Email", person.Email);

            return dictionary;
        }

        protected override List<PersonDTO> ReadParamterized(string cmd, List<string> parameters)
        {

            var people = new List<PersonDTO>();

            using (SqlConnection con = new SqlConnection(_readerConnectionString))
            {
                var query = new SqlCommand(cmd, con);
                AddParameters(query, parameters);

                con.Open();

                using (var sqlReader = query.ExecuteReader())
                {
                    while (sqlReader.Read())
                    {
                        var birthday = sqlReader.IsDBNull(3) ? new DateTime() : sqlReader.GetDateTime(3);
                        var phoneNr = sqlReader.IsDBNull(6) ? "" : sqlReader.GetString(6);
                        var email = sqlReader.IsDBNull(7) ? "" : sqlReader.GetString(7);

                        var tmpPerson = GetPersonDto
                        (
                            sqlReader.GetInt32(0),
                            sqlReader.GetString(1),
                            sqlReader.GetString(2),
                            birthday,
                            sqlReader.GetBoolean(4),
                            sqlReader.GetBoolean(5),
                            phoneNr,
                            email
                        );
                        people.Add((PersonDTO)tmpPerson);
                    }
                    con.Close();
                }

            }

            if (_onlyActive)
            {
                people = people.Where(p => p.Active).ToList();
            }

            var result = new List<DbObjDTO>();
            people.ForEach(p => result.Add((DbObjDTO) p));

            return people;
        }

        public PersonDTO GetPersonDto(int pk, string prename, string lastname, DateTime birthday, bool male, bool active, string phoneNr, string email)
        {
            var person = new PersonDTO
            {
                Pk = pk,
                Prename = prename,
                LastName = lastname,
                Birthday = birthday,
                Male = male,
                Active = active,
                PhoneNr = phoneNr,
                Email = email
            };
            return person;
        }
    }
}
