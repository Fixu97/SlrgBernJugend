using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using Shared;
using Shared.Models.db;

namespace DataLayer.DbHandler
{
    public class UserHandler<T> : DbObjHandler<T> where T : UserDTO
    {

        protected override string TableName => "USERS";
        protected override string SelectStatement => $"SELECT * FROM {TableName} ";
        protected override string OrderBy => string.Empty;

        public override void Insert(T dbObj)
        {
            var user = dbObj;
            var allUsers = new List<T>();
            var allDbObjs = GetAll();
            allDbObjs.ForEach(u => allUsers.Add(u));

            var existingUsersWithSameUSername = allUsers.FirstOrDefault(u => u.Username == user.Username);

            if (existingUsersWithSameUSername != null)
            {
                throw new InvalidDataException("User with this username already exists!");
            }
            var dictionary = GetAttributeValuePairs(dbObj);
            if (user.Admin)
            {
                string paramList = "";

                for (var i = 0; i < dictionary.Count - 1; i++)
                {
                    paramList += "@var" + i + ",";
                }
                paramList = paramList.Substring(0, paramList.Length - 1) + ";";
                WriteParameterized("EXECUTE SLRG_BERN_JUGEND.dbo.InsertAdminUser " + paramList, dictionary.Values.Where(value => value != "Admin").ToList());
            }
            else
            {
                base.Insert(dbObj);
            }
        }

        protected override Dictionary<string, string> GetAttributeValuePairs(T dbObj)
        {
            var permission = (UserDTO)dbObj;
            var dictionary = new Dictionary<string, string>();

            dictionary.Add("Username", permission.Username);
            dictionary.Add("Password", permission.Password);
            dictionary.Add("Salt", permission.Salt);
            dictionary.Add("Email", permission.Email);
            dictionary.Add("Admin", permission.Admin.ToString());

            return dictionary;
        }

        protected override List<T> ReadParamterized(string cmd, List<string> parameters)
        {

            var users = new List<T>();

            using (SqlConnection con = new SqlConnection(_readerConnectionString))
            {
                try
                {
                    var query = new SqlCommand(cmd, con);
                    AddParameters(query, parameters);

                    con.Open();

                    using (var sqlReader = query.ExecuteReader())
                    {
                        UserDTO tmpUser;
                        while (sqlReader.Read())
                        {
                            tmpUser = GetUserDto(
                                sqlReader.GetInt32(0), 
                                sqlReader.GetString(1), 
                                sqlReader.GetString(2), 
                                sqlReader.GetString(3), 
                                sqlReader.GetString(4), 
                                sqlReader.GetBoolean(5)
                                );
                            users.Add((T)tmpUser);
                            }
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

            return users;
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
            var query = $"{SelectStatement} WHERE Username = @var0;";
            var result = ReadParamterized(query, new List<string> { username });

            var users = new List<UserDTO>();
            result.ForEach(r => users.Add((UserDTO)r));

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

        public UserDTO GetUserDto(int pk, string username, string password, string salt, string email, bool admin)
        {
            var user = new UserDTO
            {
                Pk = pk,
                Username = username,
                Password = password,
                Salt = salt,
                Email = email,
                Admin = admin
            };
            return user;
        }
    }
}
