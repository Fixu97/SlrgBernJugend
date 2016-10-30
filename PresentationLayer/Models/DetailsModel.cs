using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Shared.Models;
using Shared.Models.db;

namespace PresentationLayer.Models
{
    public class DetailsModel : ViewDbModel
    {
        public string DisplayName;
        public DbObjDTO DbObj;
    }
}