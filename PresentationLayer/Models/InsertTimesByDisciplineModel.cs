using System;
using System.Collections.Generic;
using Shared.Models.db;

namespace PresentationLayer.Models
{
    public class InsertTimesByDisciplineModel : ViewDbModel
    {
        private DateTime _date;

        public DateTime Date
        {
            get
            {
                if (_date.Ticks == new DateTime().Ticks)
                {
                    return DateTime.Today;
                }
                return _date;
            }
            set { _date = value; }
        }
        public DisciplineDTO Discipline { get; set; }
        public IEnumerable<PersonDTO> People { get; set; }

        public string DisplayDate {
            get
            {
                return Date.ToString("dd.MM.yyyy");
            }
        }
    }
}