using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text.RegularExpressions;
using System.Web.Mvc;
using BusinessLayer.DbHandler;
using PresentationLayer.Models;
using Shared.Models.db;

namespace PresentationLayer.Controllers.DbObjControllers
{

    public class PeopleController : DbObjController<PersonDTO>
    {

        private DbObjHandler<PersonDTO> _personHandler = new DbPersonHandler();

        //
        // GET: /People/

        protected override string ControllerName
        {
            get { return "People"; }
        }
        public override DbObjHandler<PersonDTO> BusinessLayer { protected get { return _personHandler; } set { _personHandler = value; } }

        public override ActionResult Index()
        {
            try
            {
                var dbObjs = BusinessLayer.GetAll();
                var people = new List<PersonDTO>();
                dbObjs.ForEach(d => people.Add((PersonDTO)d));
                var prop = _GetIndexModel(people);

                return View("~/Views/DbObjViews/Shared/Index.cshtml", prop);
            }
            catch (InvalidDataException e)
            {
                HttpContext.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                return Json(e.Message, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                HttpContext.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                return Json("An unknown error occoured!", JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
        public ActionResult IndexShowInactive(bool showInactive)
        {
            try
            {
                var businessLayer = (DbPersonHandler)BusinessLayer;
                var dbObjs = businessLayer.GetAll(!showInactive);
                var people = new List<PersonDTO>();
                dbObjs.ForEach(d => people.Add((PersonDTO)d));

                var prop = _GetIndexModel(people);
                prop.ShowInactive = showInactive;

                return PartialView("~/Views/DbObjViews/Shared/Index.cshtml", prop);
            }
            catch (InvalidDataException e)
            {
                HttpContext.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                return Json(e.Message, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                HttpContext.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                return Json("An unknown error occoured!", JsonRequestBehavior.AllowGet);
            }
        }

        //
        // GET: /People/Details/5

        public override ActionResult Details(int id)
        {
            var person = BusinessLayer.Select(id);
            var prop = GetDetailsModelFromDTO(person);

            return View("~/Views/DbObjViews/Shared/Details.cshtml", prop);
        }

        //
        // GET: /People/Edit/5

        public override ActionResult Edit(int id)
        {
            var person = BusinessLayer.Select(id);
            var model = GetDetailsModelFromDTO(person);
            return View("~/Views/DbObjViews/Shared/Edit.cshtml", model);
        }

        //
        // POST: /People/Delete/5

        [HttpPost]
        public override ActionResult Delete(int id)
        {
            try
            {
                BusinessLayer.Delete(new PersonDTO() { Pk = id });
                return Json("Successfully deleted person!");
            }
            catch (Exception e)
            {
                throw new Exception("An error occoured while trying to delete this person!", e);
            }
        }



        #region AJAX requests

        [HttpPost]
        public override ActionResult GetAll()
        {
            var people = BusinessLayer.GetAll();

            return Json(people);
        }

        #endregion

        protected override DetailsModel GetDetailsModelFromDTO(DbObjDTO dto)
        {
            var person = dto as PersonDTO;
            var model = new DetailsModel()
            {
                Title = person.DisplayName,
                DisplayName = person.DisplayName,
                ControllerName = ControllerName,
                DbObj = person
            };

            return model;
        }

        protected override DbObjDTO GetDtoFromCollection(int id, FormCollection collection)
        {
            DateTime birthday;
            DateTime.TryParse(collection["Birthday"], out birthday);
            string phoneNr = collection["PhoneNr"];

            if (phoneNr == "")
            {
                phoneNr = null;
            }
            else
            {
                var inputFormat = new Regex(@"^(\+41 |0)\d\d \d\d\d \d\d \d\d$");
                if (!inputFormat.IsMatch(phoneNr))
                {
                    throw new InvalidDataException("Input format for phone nr. is invalid!");
                }
            }



            var person = new PersonDTO()
            {
                Pk = id,
                Prename = collection["Prename"],
                LastName = collection["Lastname"],
                Birthday = birthday,
                Male = collection["Male"].Contains("true"),
                Active = collection["Active"].Contains("true"),
                Email = collection["Email"],
                PhoneNr = phoneNr
            };

            return person;
        }

        private PeopleIndexModel _GetIndexModel(List<PersonDTO> people)
        {
            var prop = new PeopleIndexModel
            {
                Title = "People",
                ControllerName = "People",
                Headers = new List<string> { "Name", "Age", "Email", "PhoneNr" }
            };

            foreach (var person in people)
            {
                prop.PkList.Add(person.Pk);
                prop.Values.Add(new List<string> { person.DisplayName, person.Age > 100 ? "" : person.Age.ToString(), person.Email ?? "", person.PhoneNr ?? "" });
            }

            return prop;
        }
    }
}
