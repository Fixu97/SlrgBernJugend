using System.Web.Mvc;
using System.Web.Routing;
using Shared;
using Shared.Models.db;

namespace PresentationLayer.Config
{
    public class PermissionCheck : FilterAttribute, IAuthorizationFilter
    {
        public void OnAuthorization(AuthorizationContext filterContext)
        {

            RouteValueDictionary redirectTargetDictionary = new RouteValueDictionary();

            if (filterContext.HttpContext.Session == null)
            {
                redirectTargetDictionary = _handleUnauthorized();
            }
            else if (SessionFacade.User == null)
            {
                var actionName = filterContext.ActionDescriptor.ActionName;
                if (actionName == "Login" || actionName == "Register")
                {
                    return;
                }
                redirectTargetDictionary = _handleUnauthorized();
            }
            else
            {
                try
                {
                    UserDTO sessionUserDto = filterContext.HttpContext.Session["User"] as UserDTO;

                    if (sessionUserDto != null && sessionUserDto.Pk > 0)
                    {
                        return;
                    }

                    redirectTargetDictionary = _handleUnauthorized();
                }
                catch
                {
                    redirectTargetDictionary = _handleUnauthorized();
                }
            }
            filterContext.Result = new RedirectToRouteResult(redirectTargetDictionary);
        }

        private RouteValueDictionary _handleUnauthorized()
        {
            RouteValueDictionary redirectTargetDictionary = new RouteValueDictionary();

            redirectTargetDictionary.Add("controller", "Login");
            redirectTargetDictionary.Add("action", "Login");

            return redirectTargetDictionary;
        }
    }
}