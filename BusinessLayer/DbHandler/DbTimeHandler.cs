using System;
using System.Collections.Generic;
using System.Linq;
using Shared.Models.db;

namespace BusinessLayer.DbHandler
{
    public class DbTimeHandler: DbObjHandler<TimeDTO>
    {
        protected override DataLayer.DbHandler.DbObjHandler<TimeDTO> _db => new DataLayer.DbHandler.TimeHandler();


        public List<TimeDTO> GetTimesByPeople(List<PersonDTO> people)
        {
            var db = (DataLayer.DbHandler.TimeHandler)_db;
            var times = db.GetTimesByPeople(people);
            _sortTimesByDate(times);
            return times;
        }
        public List<TimeDTO> GetTimesByPeople(List<PersonDTO> people, DisciplineDTO discipline)
        {
            var db = (DataLayer.DbHandler.TimeHandler)_db;
            return db.GetTimesByPeople(people, discipline);
        }
        public List<TimeDTO> GetTimesByPeople(List<PersonDTO> people, DisciplineDTO discipline, DateTime[] timescope)
        {
            var db = (DataLayer.DbHandler.TimeHandler)_db;
            return db.GetTimesByPeople(people, discipline, _getTimescope(timescope));
        }
        public List<TimeDTO> GetTimesByDiscipline(List<DisciplineDTO> disciplines)
        {
            var db = (DataLayer.DbHandler.TimeHandler)_db;
            return db.GetTimesByDisciplines(disciplines);
        }
        public List<TimeDTO> GetTimesByDiscipline(List<DisciplineDTO> disciplines, bool male)
        {
            var db = (DataLayer.DbHandler.TimeHandler)_db;
            var times = db.GetTimesByDisciplines(disciplines);
            times = times.Where(t => t.Person.Male == male).ToList();

            return times;
        }
        public List<TimeDTO> GetTimesByDiscipline(List<DisciplineDTO> disciplines, PersonDTO person)
        {
            var db = (DataLayer.DbHandler.TimeHandler)_db;
            return db.GetTimesByDisciplines(disciplines, person);
        }
        public List<TimeDTO> GetTimesByDiscipline(List<DisciplineDTO> disciplines, PersonDTO person, DateTime[] timescope)
        {
            var db = (DataLayer.DbHandler.TimeHandler)_db;
            return db.GetTimesByDisciplines(disciplines, person, _getTimescope(timescope));
        }

        /// <summary>
        /// Gets the n fastest people for discipline identified by discId
        /// </summary>
        /// <param name="n">How many people should be displayed</param>
        /// <param name="discipline">The discipline</param>
        /// <returns>The n fastest people for the specified discipline</returns>
        public TimeDTO[] GetTopNPeopleForDiscipline(int n, DisciplineDTO discipline)
        {
            var db = (DataLayer.DbHandler.TimeHandler)_db;
            var allTimes = db.GetTimesByDisciplines(new List<DisciplineDTO> { discipline });
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
            var db = (DataLayer.DbHandler.TimeHandler)_db;
            var allTimes = db.GetTimesByDisciplines(new List<DisciplineDTO> { discipline });
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
            var db = (DataLayer.DbHandler.TimeHandler)_db;
            var allTimes = db.GetTimesByDisciplines(new List<DisciplineDTO> { discipline }, person);
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
    }
}
