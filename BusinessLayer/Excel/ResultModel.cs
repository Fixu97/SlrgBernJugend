using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shared.Models.db;

namespace BusinessLayer.Excel
{
    public class ResultModel
    {

        public List<string> Warnings = new List<string>();
        public List<Exception> Errors = new List<Exception>();

        public List<TimeDTO> TimesInserted = new List<TimeDTO>(); 

        public bool SuccessfullyRead;
        public bool SuccessfullyInserted;

        public bool Success => SuccessfullyRead && SuccessfullyInserted;
    }
}
