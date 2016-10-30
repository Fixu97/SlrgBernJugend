using System.Web.Mvc;
using BusinessLayer;
using Shared;

namespace PresentationLayer.Controllers
{
    public class HomeController : Controller
    {

        [HttpGet]
        public ActionResult Index()
        {
            ViewBag.Title = "Home";

            return View();
        }

        [HttpGet]
        public ActionResult Logout()
        {
            SessionFacade.DeleteSession();

            return RedirectToAction("Login","Login");

        }

        
    }
}
