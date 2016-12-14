using Shared.Models.db;

namespace PresentationLayer.Models
{
    public class DetailsModel : ViewDbModel
    {
        public string DisplayName;
        public DbObjDTO DbObj;
    }
}