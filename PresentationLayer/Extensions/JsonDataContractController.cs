using System.Runtime.Serialization;
using System.Text;
using System.Web.Mvc;

namespace PresentationLayer.Extensions
{
    /// <summary>
    /// This class replaces the standard JSON serializier of the ASP.NET MVC. Thus, the objects
    /// are serialized using the <see cref="System.Runtime.Serialization.Json.DataContractJsonSerializer"/>. This 
    /// makes the <see cref="DataMemberAttribute"/> on the objects working.
    /// </summary>
    public class JsonDataContractController : Controller
    {
        protected override JsonResult Json(object data, string contentType, Encoding contentEncoding, JsonRequestBehavior behavior)
        {
            return new JsonDataContractResult
                       {
                           Data = data,
                           ContentType = contentType,
                           ContentEncoding = contentEncoding,
                           JsonRequestBehavior = behavior
                       };
        }
    }
}