using System;
using System.Web;
using Shared.Models;
using Shared.Models.db;

namespace Shared
{
    public static class SessionFacade
    {
        public static UserDTO User
        {
            get
            {
                try
                {
                    return HttpContext.Current.Session["User"] as UserDTO;
                }
                catch (Exception e)
                {
                    IsLoggedOn = false;
                    return new UserDTO();
                }
            }
            set
            {
                HttpContext.Current.Session.Add("User", value);
                IsLoggedOn = true;
            }
        }

        public static bool IsLoggedOn
        {
            get
            {
                if (HttpContext.Current.Session["IsLoggedOn"] == null)
                {
                    IsLoggedOn = false;
                    return IsLoggedOn;
                }
                return Boolean.Parse(HttpContext.Current.Session["IsLoggedOn"].ToString());
            }
            set { HttpContext.Current.Session.Add("IsLoggedOn", value);}
        }

        public static void DeleteSessionVariable(string name)
        {
            HttpContext.Current.Session.Remove(name);
        }
        public static void DeleteSession()
        {
            IsLoggedOn = false;
            HttpContext.Current.Session.Clear();
            HttpContext.Current.Session.Abandon();
        }
    }
}