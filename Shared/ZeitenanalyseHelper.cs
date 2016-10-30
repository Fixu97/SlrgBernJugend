using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Shared.Models;
using Shared.Models.db;

namespace Shared
{
    public static class ZeitenanalyseHelper
    {

        public static String Sha256Hash(string value)
        {
            StringBuilder Sb = new StringBuilder();

            using (SHA256 hash = SHA256Managed.Create())
            {
                Encoding enc = Encoding.UTF8;
                Byte[] result = hash.ComputeHash(enc.GetBytes(value));

                foreach (Byte b in result)
                    Sb.Append(b.ToString("x2"));
            }

            return Sb.ToString();
        }

        public static string DatetimeToDate(DateTime date)
        {
            return date.ToString("dd.MM.yyyy");
        }

        public static readonly List<string> DisciplinesAttr = new List<string>(){ "Discipline", "Meters" };
        public static readonly List<string> PeopleAttr = new List<string>() { "Prename", "Lastname", "Birthday", "Male", "Active", "PhoneNr", "Email" };
        public static readonly List<string> PermissionsAttr = new List<string>() { "FK_U", "FK_P" };
        public static readonly List<string> TimesAttr = new List<string>() { "FK_P", "FK_D", "Seconds", "Date" };
        public static readonly List<string> UsersAttr = new List<string>() { "Username", "Password", "Salt", "Email", "Admin" };

        public static List<string> GetDbList(string[] array)
        {
            List<string> retList = new List<string>();
            int ubArr = array.Length - 1;

            for(int i = 1; i <= ubArr; i++)
            {
                retList.Add(array[i]);
            }

            return retList;
        }



        /// <summary>
        /// Checks if the given UserDto represents a valid user by assuring that the necessary properties are set.
        /// </summary>
        /// <param name="userDto">UserDto</param>
        /// <returns>True if valid, false if not.</returns>
        public static bool CheckIfUserDtoIsValid(UserDTO userDto)
        {
            return !(userDto.Pk == 0 || 
                String.IsNullOrWhiteSpace(userDto.Username) ||
                String.IsNullOrWhiteSpace(userDto.Password) || 
                String.IsNullOrWhiteSpace(userDto.Salt) ||
                String.IsNullOrWhiteSpace(userDto.Email)
                );
        }

        /// <summary>
        /// Checks if the given PersonDto represents a valid person by assuring that the necessary properties are set.
        /// </summary>
        /// <param name="personDto">PersonDto</param>
        /// <returns>True if valid, false if not.</returns>
        public static bool CheckIfPersonDtoIsValid(PersonDTO personDto)
        {
            return !(
                personDto.Pk == 0 || 
                String.IsNullOrWhiteSpace(personDto.Prename) ||
                String.IsNullOrWhiteSpace(personDto.LastName)
            );
        }


        /// <summary>
        /// Creates salt <see href="https://crackstation.net/hashing-security.htm">for salted pw encryption</see>
        /// </summary>
        /// <returns>salt</returns>
        public static string CreateSalt()
        {
            byte[] saltBytes = new byte[64];
            var rand = new RNGCryptoServiceProvider();

            rand.GetBytes(saltBytes);
            var salt = saltBytes.Aggregate("", (current, item) => current + item.ToString());
            salt = ZeitenanalyseHelper.Sha256Hash(salt);

            return salt;
        }
    }
}
