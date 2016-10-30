using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Shared.Models;

namespace PresentationLayer.Models
{
    public class IndexModel : ViewDbModel
    {
        public List<string> Headers;
        public List<List<string>> Values = new List<List<string>>();
        public List<int> PkList = new List<int>();
    }
}