using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using DataLayer.Models;
using Shared;
using Shared.Models;
using Shared.Models.db;

namespace DataLayer
{
    public class ZeitenanalyseDataLayer
    {

        #region private Members

        private readonly string _readerConnectionString = @"Server=localhost\SQLEXPRESS;Database=SLRG_BERN_JUGEND;User Id=db_read;Password=read-3087";

        #endregion

        #region Discipline

        /// <summary>
        /// Gets a Discipline by its primary key (from the database).
        /// </summary>
        /// <param name="pK">The primary key</param>
        /// <returns>A DisciplineDTO Object</returns>
        public DisciplineDTO GetDisciplineByPk(int pK)
        {
            var disciplines = _getQueryResultDisciplineFromDb("SELECT * FROM DISCIPLINES WHERE PK = " + pK);
            return disciplines[0];
        }

        public List<DisciplineDTO> GetAllDisciplines()
        {
            return _getQueryResultDisciplineFromDb("SELECT * FROM DISCIPLINES ORDER BY Discipline ASC;");
        }

        #endregion

        #region Person


        /// <summary>
        /// Gets a Person by its primary key (from the database).
        /// </summary>
        /// <param name="pK">The primary key</param>
        /// <returns>A PersonDTO Object</returns>
        public PersonDTO GetPersonByPk(int pK)
        {
            var people = _getQueryResultPersonFromDb("SELECT * FROM PEOPLE AS P1 JOIN PERMISSIONS AS P2 ON P1.PK = P2.FK_P WHERE P1.PK = " + pK + " AND FK_U = " + SessionFacade.User.Pk + ";", false);
            return people[0];
        }

        public List<PersonDTO> GetAllPeople(bool showInactive)
        {
            return _getQueryResultPersonFromDb("SELECT * FROM PEOPLE AS P1 JOIN PERMISSIONS AS P2 ON P1.PK = P2.FK_P WHERE FK_U = " + SessionFacade.User.Pk + " ORDER BY Prename ASC, Lastname ASC;", !showInactive);
        }

        #endregion

        #region Time

        // Add Joins to DB Querys instead of letting them select themselfes


        /// <summary>
        /// Gets a Time by its primary key (from the database).
        /// </summary>
        /// <param name="pK">The primary key</param>
        /// <returns>A TimeDTO Object</returns>
        public TimeDTO GetTimeByPk(int pK)
        {
            var times = _getQueryResultTimeFromDb("SELECT * FROM TIMES WHERE PK = " + pK);
            return times[0];
        }

        public List<TimeDTO> GetTimesByPeople(List<PersonDTO> personDtos)
        {
            var times = new List<TimeDTO>();

            string cmd = "SELECT PK, FK_P, FK_D, Seconds, [Date] FROM TIMES WHERE (";

            foreach (PersonDTO personDto in personDtos)
            {
                cmd += "FK_P = " + personDto.Pk + " OR ";
            }

            cmd += "FK_P = 0) ORDER BY [Date] ASC, FK_D, FK_P, Seconds, PK;";

            times = _getQueryResultTimeFromDb(cmd);

            return times;


        }
        public List<TimeDTO> GetTimesByPeople(List<PersonDTO> personDtos, DisciplineDTO disciplineDto)
        {
            var times = new List<TimeDTO>();

            string cmd = "SELECT PK, FK_P, FK_D, Seconds, [Date] FROM TIMES WHERE (";

            foreach (PersonDTO personDto in personDtos)
            {
                cmd += "FK_P = " + personDto.Pk + " OR ";
            }

            cmd += $"FK_P = 0) AND FK_D = {disciplineDto.Pk} ORDER BY [Date] ASC, FK_D, FK_P, Seconds, PK;";

            times = _getQueryResultTimeFromDb(cmd);

            return times;


        }
        public List<TimeDTO> GetTimesByPeople(List<PersonDTO> personDtos, DisciplineDTO disciplineDto, DateTime[] timeScope)
        {
            var times = new List<TimeDTO>();
            string fromTo = _getTimeScope(timeScope);

            string cmd = "SELECT PK, FK_P, FK_D, Seconds, [Date] FROM TIMES WHERE (";

            foreach (PersonDTO personDto in personDtos)
            {
                cmd += "FK_P = " + personDto.Pk + " OR ";
            }

            cmd += $"FK_P = 0) AND FK_D = {disciplineDto.Pk} AND {fromTo} ORDER BY [Date] ASC, FK_D, FK_P, Seconds, PK;";

            times = _getQueryResultTimeFromDb(cmd);

            return times;


        }
        public List<TimeDTO> GetTimesByDisciplines(List<DisciplineDTO> disciplineDtos)
        {

            string cmd = "SELECT PK, FK_P, FK_D, Seconds, [Date] FROM TIMES WHERE (";

            foreach (DisciplineDTO disciplineDto in disciplineDtos)
            {
                cmd += "FK_D = " + disciplineDto.Pk + " OR ";
            }

            cmd += "FK_D = 0) ORDER BY [Date] ASC, FK_D, FK_P, Seconds, PK;";


            return _getQueryResultTimeFromDb(cmd);
        }
        public List<TimeDTO> GetTimesByDisciplines(List<DisciplineDTO> disciplineDtos, PersonDTO personDto)
        {

            string cmd = "SELECT PK, FK_P, FK_D, Seconds, [Date] FROM TIMES WHERE (";

            foreach (DisciplineDTO disciplineDto in disciplineDtos)
            {
                cmd += "FK_D = " + disciplineDto.Pk + " OR ";
            }

            cmd += $"FK_D = 0) AND FK_P = {personDto.Pk} ORDER BY [Date] ASC, FK_D, FK_P, Seconds, PK;";


            return _getQueryResultTimeFromDb(cmd);
        }
        public List<TimeDTO> GetTimesByDisciplines(List<DisciplineDTO> disciplineDtos, PersonDTO personDto, DateTime[] timeScope)
        {
            string fromTo = _getTimeScope(timeScope);

            string cmd = "SELECT PK, FK_P, FK_D, Seconds, [Date] FROM TIMES WHERE (";

            foreach (DisciplineDTO disciplineDto in disciplineDtos)
            {
                cmd += "FK_D = " + disciplineDto.Pk + " OR ";
            }

            cmd += $"FK_D = 0) AND FK_P = {personDto.Pk} AND {fromTo} ORDER BY [Date] ASC, FK_D, FK_P, Seconds, PK;";


            return _getQueryResultTimeFromDb(cmd);
        }
        public List<TimeDTO> GetAllTimes(DateTime[] timeScope)
        {
            string fromTo = _getTimeScope(timeScope);

            string cmd = "SELECT PK, FK_P, FK_D, Seconds, [Date] FROM TIMES WHERE " + fromTo + " ORDER BY [Date] ASC, FK_D, FK_P, Seconds, PK;";

            return _getQueryResultTimeFromDb(cmd);
        }

        #endregion

        #region User

        public UserDTO GetUserByPk(int pK)
        {
            var users = _getQueryResultUserFromDb("SELECT * FROM USERS WHERE PK = " + pK);
            return users[0];
        }

        public List<UserDTO> GetAllUsers()
        {
            return _getQueryResultUserFromDb("SELECT * FROM USERS ORDER BY PK ASC;");
        }

        /// <summary>
        /// 1. Retrieve the user's salt and hash from the database.
        /// 2. Prepend the salt to the given password and hash it using the same hash function.
        /// 3. Compare the hash of the given password with the hash from the database. If they match, the password is correct. Otherwise, the password is incorrect
        /// </summary>
        /// <param name="username">The username</param>
        /// <param name="password">The hashed password</param>
        /// <returns></returns>
        public UserDTO GetUserByCredentials(string username, string password)
        {
            var users = _getQueryResultUserFromDb("SELECT * FROM USERS WHERE Username = '" + username + "';");

            if (users.Count > 0)
            {
                var user = users[0];

                if (user.Password == ZeitenanalyseHelper.Sha256Hash(password + user.Salt))
                {
                    return user;
                }
            }

            return new UserDTO();
        }

        #endregion

        #region Permission

        public PermissionDTO GetPermissionByPk(int pK)
        {
            var permissions = _getQueryResultPermissionFromDb("SELECT * FROM PERMISSIONS WHERE PK = " + pK);
            return permissions[0];
        }

        public List<PermissionDTO> GetAllPermissions()
        {
            return _getQueryResultPermissionFromDb("SELECT * FROM PERMISSIONS ORDER BY FK_U, FK_P, PK;");
        }

        #endregion

        #region Write methods
        public void Insert(DbObjDTO dbObjDto)
        {
            Convert(dbObjDto).Insert();
        }

        public void Update(DbObjDTO dbObjDto)
        {
            Convert(dbObjDto).Update();
        }

        public void Delete(DbObjDTO dbObjDto)
        {
            Convert(dbObjDto).Delete();
        }
        #endregion
        private DbObj Convert(DbObjDTO dbObjDto)
        {
            DbObj obj;

            if (dbObjDto is DisciplineDTO)
            {
                obj = new Discipline(dbObjDto as DisciplineDTO);
            }
            else if (dbObjDto is PermissionDTO)
            {
                obj = new Permission(dbObjDto as PermissionDTO);
            }
            else if (dbObjDto is PersonDTO)
            {
                obj = new Person(dbObjDto as PersonDTO);
            }
            else if (dbObjDto is TimeDTO)
            {
                obj = new Time(dbObjDto as TimeDTO);
            }
            else if (dbObjDto is UserDTO)
            {
                obj = new User(dbObjDto as UserDTO);
            }
            else
            {
                throw new Exception("DTO not valid!");
            }
            return obj;
        }

        private List<DisciplineDTO> _getQueryResultDisciplineFromDb(string cmd)
        {

            var disciplines = new List<DisciplineDTO>();

            using (SqlConnection con = new SqlConnection(_readerConnectionString))
            {
                try
                {
                    var query = new SqlCommand(cmd, con);
                    con.Open();

                    using (var sqlReader = query.ExecuteReader())
                    {
                        Discipline tmpDiscipline;
                        while (sqlReader.Read())
                        {
                            tmpDiscipline = new Discipline(
                                sqlReader.GetInt32(0),
                                sqlReader.GetString(1),
                                sqlReader.GetInt32(2)
                                );
                            disciplines.Add(tmpDiscipline.Dto);
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
        private List<PersonDTO> _getQueryResultPersonFromDb(string cmd, bool onlyActivePeople = true)
        {
            
            var people = new List<PersonDTO>();

            using (SqlConnection con = new SqlConnection(_readerConnectionString))
            {
                try
                {
                    var query = new SqlCommand(cmd, con);
                    con.Open();

                    using (var sqlReader = query.ExecuteReader())
                    {
                        Person tmpPerson;
                        DateTime birthday;
                        string phoneNr;
                        string email;

                        while (sqlReader.Read())
                        {
                            birthday = sqlReader.IsDBNull(3) ? new DateTime() : sqlReader.GetDateTime(3);
                            phoneNr = sqlReader.IsDBNull(6) ? "" : sqlReader.GetString(6);
                            email = sqlReader.IsDBNull(7) ? "" : sqlReader.GetString(7);
                            
                            tmpPerson = new Person(
                                sqlReader.GetInt32(0),
                                sqlReader.GetString(1),
                                sqlReader.GetString(2),
                                birthday,
                                sqlReader.GetBoolean(4),
                                sqlReader.GetBoolean(5),
                                phoneNr,
                                email
                                );
                            people.Add(tmpPerson.Dto);
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
            if (onlyActivePeople)
            {
                people = people.Where(p => p.Active).ToList();
            }
            return people;
        }
        private List<TimeDTO> _getQueryResultTimeFromDb(string cmd, bool onlyActivePeople = true)
        {

            List<TimeDTO> times = new List<TimeDTO>();

            using (SqlConnection con = new SqlConnection(_readerConnectionString))
            {
                try
                {
                    var query = new SqlCommand(cmd, con);
                    con.Open();

                    using (var sqlReader = query.ExecuteReader())
                    {
                        Time tmpTime;
                        if (sqlReader.HasRows)
                        {
                            while (sqlReader.Read())
                            {
                                tmpTime = new Time(
                                    sqlReader.GetInt32(0),
                                    sqlReader.GetInt32(1),
                                    sqlReader.GetInt32(2),
                                    sqlReader.GetDecimal(3),
                                    sqlReader.GetDateTime(4)
                                    );
                                times.Add(tmpTime.Dto);
                            }
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
            if (onlyActivePeople)
            {
                times = times.Where(t => t.Person.Active).ToList();
            }
            return times;
        }
        private List<UserDTO> _getQueryResultUserFromDb(string cmd)
        {

            var users = new List<UserDTO>();

            using (SqlConnection con = new SqlConnection(_readerConnectionString))
            {
                try
                {
                    var query = new SqlCommand(cmd, con);
                    con.Open();

                    using (var sqlReader = query.ExecuteReader())
                    {
                        User tmpUser;
                        while (sqlReader.Read())
                        {
                            tmpUser = new User(
                                sqlReader.GetInt32(0),
                                sqlReader.GetString(1),
                                sqlReader.GetString(2),
                                sqlReader.GetString(3),
                                sqlReader.GetString(4),
                                sqlReader.GetBoolean(5)
                                );
                            users.Add(tmpUser.Dto);
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
            return users;
        }
        private List<PermissionDTO> _getQueryResultPermissionFromDb(string cmd)
        {

            var permissions = new List<PermissionDTO>();

            using (SqlConnection con = new SqlConnection(_readerConnectionString))
            {
                try
                {
                    var query = new SqlCommand(cmd, con);
                    con.Open();

                    using (var sqlReader = query.ExecuteReader())
                    {
                        Permission tmpPermission;
                        while (sqlReader.Read())
                        {
                            var pk = sqlReader.GetInt32(0);
                            var fk_u = sqlReader.GetInt32(1);
                            var fk_p = sqlReader.GetInt32(2);

                            tmpPermission = new Permission(
                                pk,
                                fk_u,
                                fk_p
                                );
                            permissions.Add(tmpPermission.Dto);
                        }
                        sqlReader.Close();
                    }
                    con.Close();
                }
                catch (SqlException e)
                {
                    throw;
                }
                catch (Exception e)
                {
                    con.Dispose();
                    throw;
                }

            }
            return permissions;
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
