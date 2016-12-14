using System.Web.Mvc;
using Microsoft.Ajax.Utilities;
using BusinessLayer.DbHandler;
using Shared;
using PresentationLayer.Models;

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
            return View("Login", new LoginModel());
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
            return View("Login", new LoginModel { CurrentMode = LoginModel.PageMode.Erroneous });
            return RedirectToAction("Login", "Login");
        }

    }
}
