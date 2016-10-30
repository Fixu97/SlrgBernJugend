using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using DataLayer;
using Shared;
using Shared.Models;
using Shared.Models.db;

namespace BusinessLayer
{
    public class ZeitenanalyseBusinessLayer
    {

        private ZeitenanalyseDataLayer dataLayer = new ZeitenanalyseDataLayer();

        #region Write methods
        public void Insert(DbObjDTO dbObjDto)
        {
            dataLayer.Insert(dbObjDto);
        }
        public void Update(DbObjDTO dbObjDto)
        {
            dataLayer.Update(dbObjDto);
        }
        public void Delete(DbObjDTO dbObjDto)
        {
            dataLayer.Delete(dbObjDto);
        }
        #endregion

        #region Discipline

        public DisciplineDTO GetDisciplineDto(int pK)
        {
            return dataLayer.GetDisciplineByPk(pK);
        }
        public List<DisciplineDTO> GetAllDisciplines()
        {
            return dataLayer.GetAllDisciplines();
        }

        #endregion

        #region Person

        public PersonDTO GetPersonDto(int pK)
        {
            return dataLayer.GetPersonByPk(pK);
        }

        public List<PersonDTO> GetAllPeople(bool showInactives = false)
        {
            return dataLayer.GetAllPeople(showInactives);
        }

        #endregion

        #region Time

        public List<TimeDTO> GetAllTimes(DateTime[] timescope = null)
        {
            return dataLayer.GetAllTimes(_getTimescope(timescope));
        } 
        public TimeDTO GetTimeDto(int pK)
        {
            return dataLayer.GetTimeByPk(pK);
        }
        public List<TimeDTO> GetTimesByPeople(List<PersonDTO> people)
        {
            var times =  dataLayer.GetTimesByPeople(people);
            _sortTimesByDate(times);
            return times;
        }
        public List<TimeDTO> GetTimesByPeople(List<PersonDTO> people, DisciplineDTO discipline)
        {
            return dataLayer.GetTimesByPeople(people, discipline);
        }
        public List<TimeDTO> GetTimesByPeople(List<PersonDTO> people, DisciplineDTO discipline, DateTime[] timescope)
        {
            return dataLayer.GetTimesByPeople(people, discipline, _getTimescope(timescope));
        }
        public List<TimeDTO> GetTimesByDiscipline(List<DisciplineDTO> disciplines)
        {
            return dataLayer.GetTimesByDisciplines(disciplines);
        }
        public List<TimeDTO> GetTimesByDiscipline(List<DisciplineDTO> disciplines, bool male)
        {
            var times = dataLayer.GetTimesByDisciplines(disciplines);
            times = times.Where(t => t.Person.Male == male).ToList();

            return times;
        }
        public List<TimeDTO> GetTimesByDiscipline(List<DisciplineDTO> disciplines, PersonDTO person)
        {
            return dataLayer.GetTimesByDisciplines(disciplines, person);
        }
        public List<TimeDTO> GetTimesByDiscipline(List<DisciplineDTO> disciplines, PersonDTO person, DateTime[] timescope)
        {
            return dataLayer.GetTimesByDisciplines(disciplines, person, _getTimescope(timescope));
        }

        /// <summary>
        /// Gets the n fastest people for discipline identified by discId
        /// </summary>
        /// <param name="n">How many people should be displayed</param>
        /// <param name="discipline">The discipline</param>
        /// <returns>The n fastest people for the specified discipline</returns>
        public TimeDTO[] GetTopNPeopleForDiscipline(int n, DisciplineDTO discipline)
        {
            var allTimes = dataLayer.GetTimesByDisciplines(new List<DisciplineDTO> { discipline });
            return GetTopNUniquePeople(n, allTimes);
        }
        /// <summary>
        /// Gets the n fastest people for discipline identified by discId
        /// </summary>
        /// <param name="n">How many people should be displayed</param>
        /// <param name="discipline">The discipline</param>
        /// <returns>The n fastest people for the specified discipline</returns>
        public TimeDTO[] GetTopNPeopleForDiscipline(int n, DisciplineDTO discipline, bool male)
        {
            var allTimes = dataLayer.GetTimesByDisciplines(new List<DisciplineDTO> { discipline });
            var genderSeparatedTimes = allTimes.Where(t => t.Person.Male == male).ToList();
            return GetTopNUniquePeople(n, genderSeparatedTimes);
        }

        private TimeDTO[] GetTopNUniquePeople(int n, List<TimeDTO> allTimes)
        {
            var sortedTimes = allTimes.OrderBy(x => x.Seconds).ThenByDescending(x => x.Date);
            var topN = new TimeDTO[n];

            int j = 0;
            for (var i = 0; i < n; i++)
            {
                var curTime = sortedTimes.ElementAtOrDefault(i);

                // if no elements are available, the list has ended.
                if (curTime == null)
                {
                    return topN;
                }
                if (topN.Any(t => t != null && t.FK_P == curTime.FK_P))
                {
                    // if a top time is already available for this person, skip the entry (because we want the n fastest swimmer)
                    n++;
                    continue;
                }
                topN[j] = curTime;
                j++;
            }

            return topN;
        }

        /// <summary>
        /// Get the highscore of a person of a discipline
        /// </summary>
        /// <param name="person">The person</param>
        /// <param name="discipline">The discipline</param>
        /// <returns>A single <see cref="TimeDTO"/> object</returns>
        public TimeDTO GetHighscoreForPersonForDiscipline(PersonDTO person, DisciplineDTO discipline)
        {
            var allTimes = dataLayer.GetTimesByDisciplines(new List<DisciplineDTO> { discipline }, person);
            var sortedTimes = allTimes.OrderBy(x => x.Seconds).ThenByDescending(x => x.Date);

            return sortedTimes.FirstOrDefault();
        }
        private List<TimeDTO> _sortTimesBySeconds(List<TimeDTO> times)
        {
            times.Sort((x, y) => decimal.Compare(x.Seconds, y.Seconds));
            return times;
        }
        private List<TimeDTO> _sortTimesByDate(List<TimeDTO> times)
        {
            times.Sort((x, y) => DateTime.Compare(x.Date, y.Date));
            return times;
        }

        private DateTime[] _getTimescope(DateTime[] timescope = null)
        {
            if (timescope == null)
            {
                timescope = new[] { new DateTime(), DateTime.Now };
            }
            return timescope;
        }

        #endregion

        #region User

        public UserDTO GetUserDto(int pK)
        {
            return dataLayer.GetUserByPk(pK);
        }
        public UserDTO GetUserDtoByCredentials(string username, string password)
        {
            return dataLayer.GetUserByCredentials(username, password);
        }

        public List<UserDTO> GetAllUsers()
        {
            return dataLayer.GetAllUsers();
        }

        #endregion

        #region Permission

        public PermissionDTO GetPermissionDto(int pK)
        {
            return dataLayer.GetPermissionByPk(pK);
        }

        public List<PermissionDTO> GetAllPermissions()
        {
            return dataLayer.GetAllPermissions();
        }

        #endregion
    }
}
