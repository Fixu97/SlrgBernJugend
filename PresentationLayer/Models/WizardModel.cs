using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Shared.Models.db;

namespace PresentationLayer.Models
{
    public class WizardModel
    {
        public List<DisciplineDTO> Disciplines { get; set; }
        public List<PersonDTO> People { get; set; }
    }
}