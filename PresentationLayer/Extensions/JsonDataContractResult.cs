using System;
using System.Web;
using System.Web.Mvc;
using Newtonsoft.Json;

namespace PresentationLayer.Extensions
{
    /// <summary>
    /// This class implements the ASP.NET MVC JSON action results. The JSON results are serialized with Json.NET
    /// </summary>
    public class JsonDataContractResult : JsonResult
    {
        public override void ExecuteResult(ControllerContext context)
        {
            if (context == null)
                throw new ArgumentNullException("context");

            if (JsonRequestBehavior == JsonRequestBehavior.DenyGet && string.Equals(context.HttpContext.Request.HttpMethod, "GET", StringComparison.OrdinalIgnoreCase))
                throw new InvalidOperationException("Get is not allowed");

            HttpResponseBase response = context.HttpContext.Response;

            if (!string.IsNullOrEmpty(ContentType))
                response.ContentType = ContentType;
            else
                response.ContentType = "application/json";

            if (ContentEncoding != null)
                response.ContentEncoding = ContentEncoding;

            if (Data != null)
            {
                //Using Json.NET instead of DataContractJsonSerializer because it's much faster
                response.Write(JsonConvert.SerializeObject(Data));
            }
        }
    }
}