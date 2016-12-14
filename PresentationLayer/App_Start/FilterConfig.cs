using System.Web.Mvc;
using PresentationLayer.Config;

namespace PresentationLayer
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
            filters.Add(new PermissionCheck());
        }
    }
}