using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Shared.Models.db;

namespace PresentationLayer.Models
{
    public class InsertModel : ViewDbModel
    {
        public string Message;
        public History hist;
        public enum History { None, Error, Message}

        public DbObjDTO InsertedObject;

        public InsertModel(History hist = History.None, string msg = null, DbObjDTO insertedObject = null)
        {
            Message = msg;
            this.hist = hist;
            InsertedObject = insertedObject;
        }
    }
}