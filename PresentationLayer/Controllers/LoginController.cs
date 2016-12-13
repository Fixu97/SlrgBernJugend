using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.Ajax.Utilities;
using System.Security.Cryptography;
using System.Text;
using BusinessLayer;
using BusinessLayer.DbHandler;
using PresentationLayer.Config;
using Shared;
using Shared.Models.db;

namespace PresentationLayer.Controllers
{
    public class LoginController : Controller
    {
        private DbUserHandler BusinessLayer = new DbUserHandler();
        //
        // GET: /Login/
        [HttpGet]
        public ActionResult Login()
        {
            return View("Login");
        }


        [HttpPost]
        [ActionName("Login")]
        public ActionResult Logon()
        {
            var frmData = Request.Form;

            if (!frmData["Username"].IsNullOrWhiteSpace() || !frmData["Password"].IsNullOrWhiteSpace())
            {
                var userDto = BusinessLayer.GetUserByCredentials(frmData["Username"], frmData["Password"]);
                if (ZeitenanalyseHelper.CheckIfUserDtoIsValid(userDto))
                {
                    SessionFacade.User = userDto;
                    return RedirectToAction("Index", "Home");
                }
            }
            return RedirectToAction("Login", "Login");
        }

    }
}
