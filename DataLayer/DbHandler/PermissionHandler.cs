using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Shared.Models.db;

namespace DataLayer.DbHandler
{
    public class PermissionHandler : DbObjHandler<PermissionDTO>
    {

        protected override string TableName => "PERMISSIONS";
        protected override string SelectStatement => $"SELECT {TableName}.PK, PE.PK, U.PK, PE.Prename, PE.Lastname, PE.Birthday, PE.Male, PE.Active, PE.PhoneNr, PE.Email, U.Username, U.Password, U.Salt, U.Email, U.Admin FROM {TableName} " +
                                                     $"JOIN PEOPLE AS PE ON PE.PK = {TableName}.FK_P " +
                                                     $"JOIN USERS AS U ON U.PK = {TableName}.FK_U ";
        protected override string OrderBy => string.Empty;

        protected override Dictionary<string, string> GetAttributeValuePairs(PermissionDTO dbObj)
        {
            var permission = (PermissionDTO) dbObj;
            var dictionary = new Dictionary<string, string>();

            dictionary.Add("FK_U", permission.FK_U.ToString());
            dictionary.Add("FK_P", permission.FK_P.ToString());

            return dictionary;
        }

        protected override List<PermissionDTO> ReadParamterized(string cmd, List<string> parameters)
        {

            var permissions = new List<PermissionDTO>();

            using (SqlConnection con = new SqlConnection(_readerConnectionString))
            {
                try
                {
                    var query = new SqlCommand(cmd, con);
                    AddParameters(query, parameters);
                    con.Open();

                    using (var sqlReader = query.ExecuteReader())
                    {
                        PermissionDTO tmpPermission;
                        while (sqlReader.Read())
                        {
                            var pk = sqlReader.GetInt32(0);
                            var fk_p = sqlReader.GetInt32(1);
                            var fk_u = sqlReader.GetInt32(2);

                            tmpPermission = GetPermissionDto
                            (
                                pk,
                                fk_u,
                                fk_p
                            );

                            var birthday = sqlReader.IsDBNull(5) ? new DateTime() : sqlReader.GetDateTime(5);
                            var phoneNr = sqlReader.IsDBNull(8) ? "" : sqlReader.GetString(8);
                            var email = sqlReader.IsDBNull(9) ? "" : sqlReader.GetString(9);

                            var personHandler = new PersonHandler();

                            // Fill up person object
                            tmpPermission.Person = personHandler.GetPersonDto(
                                fk_p,
                                sqlReader.GetString(3),
                                sqlReader.GetString(4),
                                birthday,
                                sqlReader.GetBoolean(6),
                                sqlReader.GetBoolean(7),
                                phoneNr,
                                email
                                );
                            permissions.Add((PermissionDTO)tmpPermission);

                            var userHandler = new UserHandler();

                            // Fill up user object
                            tmpPermission.User = userHandler.GetUserDto(
                                fk_u,
                                sqlReader.GetString(10),
                                sqlReader.GetString(11),
                                sqlReader.GetString(12),
                                sqlReader.GetString(13),
                                sqlReader.GetBoolean(14));
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

        public PermissionDTO GetPermissionDto(int pk, int fk_p, int fk_u)
        {
            var permission = new PermissionDTO
            {
                Pk = pk,
                FK_P = fk_p,
                FK_U = fk_u
            };
            return permission;
        }
    }
}
