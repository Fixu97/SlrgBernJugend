using System;
using System.Collections.Generic;
using System.Net;
using System.Web.Mvc;
using BusinessLayer;
using BusinessLayer.DbHandler;
using PresentationLayer.Models;
using Shared.Models.db;
using System.Linq;

namespace PresentationLayer.Controllers.DbObjControllers
{
    public class TimesController : DbObjController<TimeDTO>
    {
        private DbObjHandler<TimeDTO> _timeHandler = new DbTimeHandler();
        private DbObjHandler<DisciplineDTO> _disciplineHandler = new DbDisciplineHandler();
        private DbObjHandler<PersonDTO> _personHandler = new DbPersonHandler();

        protected override string ControllerName
        {
            get { return "Times"; }
        }

        public override DbObjHandler<TimeDTO> BusinessLayer { protected get { return _timeHandler; } set { _timeHandler = value; } }
        public DbObjHandler<PersonDTO> PersonHandler { private get { return _personHandler; } set { _personHandler = value; } }
        public DbObjHandler<DisciplineDTO> DisciplineHandler { private get { return _disciplineHandler; } set { _disciplineHandler = value; } }

        [HttpGet]
        public override ActionResult Index()
        {
            var dbObjs = BusinessLayer.GetAll();
            var times = new List<TimeDTO>();
            dbObjs.ForEach(t => times.Add(t));

            var prop = new IndexModel()
            {
                ControllerName = ControllerName,
                Headers = new List<string>() { "Person","Discipline","Date","Seconds"},
                Title = ControllerName
            };

            foreach (var time in times)
            {
                prop.PkList.Add(time.Pk);
                prop.Values.Add(new List<string>(){time.Person.DisplayName,time.Discipline.DisplayName,time.Date.ToString("dd.MM.yyyy"),time.Seconds.ToString("00.00")});
            }

            return View("~/Views/DbObjViews/Shared/Index.cshtml", prop);
        }
        [HttpGet]
        public override ActionResult Details(int id)
        {
            var time = BusinessLayer.Select(id);
            var model = GetDetailsModelFromDTO(time);

            return View("~/Views/DbObjViews/Shared/Details.cshtml", model);
        }
        [HttpGet]
        public override ActionResult Edit(int id)
        {
            var time = BusinessLayer.Select(id);
            var model = GetDetailsModelFromDTO(time);
            return View("~/Views/DbObjViews/Shared/Edit.cshtml", model);
        }
        [HttpPost]
        public override ActionResult Delete(int id)
        {
            try
            {
                var time = new TimeDTO() { Pk = id };

                BusinessLayer.Delete(time);
                return Json("Successfully deleted entry!");
            }
            catch (Exception e)
            {
                throw new Exception("An error occoured while trying to delete this entry!", e);
            }
        }

        [HttpGet]
        public ActionResult Wizard(int id)
        {
            var discipline = DisciplineHandler.Select(id);
            var people = PersonHandler.GetAll();

            var model = new InsertTimesByDisciplineModel
            {
                ControllerName = "Disciplines",
                Title = "Disciplines",
                Discipline = discipline,
                People = people
            };

            ViewBag.Title = "Wizard, magic n'shit";
            return View("~/Views/DbObjViews/Times/CreateWizard.cshtml", model);
        }

        [HttpPost]
        public override ActionResult GetAll()
        {
            var times = BusinessLayer.GetAll();
            return Json(times);
        }

        [HttpGet]
        public ActionResult CreateByDiscipline(int discId, string people, string date)
        {

            var peopleIds = people.Split(',').ToList().Select(x => int.Parse(x));

            var dateTime = DateTime.Parse(date);
            var discipline = _disciplineHandler.Select(discId);
            var peopleObjects = _personHandler.GetAll().Where(p => peopleIds.Any(pId => pId == p.Pk)).ToList();

            var model = new InsertTimesByDisciplineModel
            {
                ControllerName = "Disciplines",
                Title = "Disciplines",
                Discipline = discipline,
                People = peopleObjects,
                Date = dateTime,
            };

            ViewBag.Title = "Insert by discipline";
            return View("~/Views/DbObjViews/Times/CreateByDiscipline.cshtml", model);
        }
        
        [HttpPost]
        public ActionResult BulkInsert()
        {
            var discId = int.Parse(Request.Form["discId"]);
            var discipline = _disciplineHandler.Select(discId);

            var times = new List<TimeDTO>();
            
            foreach (string key in Request.Form.AllKeys.Where( k => k.StartsWith("time")))
            {
                var strVal = Request.Form[key];
                decimal value;

                // Don't insert empty times
                if(string.IsNullOrEmpty(strVal) || !decimal.TryParse(strVal, out value))
                {
                    continue;
                }

                var parts = key.Split('|');

                var persId = int.Parse(parts[1]);
                var person = _personHandler.Select(persId);

                var date = DateTime.Parse(parts[2]);

                times.Add(new TimeDTO
                    {
                        Person = person,
                        FK_P = person.Pk,
                        Discipline = discipline,
                        FK_D = discipline.Pk,
                        Date = date,
                        Seconds = value
                    });
            }

            // insert all times
            times.ForEach(t => BusinessLayer.Insert(t));

            return RedirectToAction("Wizard", new { id = discId });
        }

        [HttpPost]
        public ActionResult GetTimesByPerson(int id)
        {
            var person = PersonHandler.Select(id);
            var db = BusinessLayer as DbTimeHandler;
            var times = db.GetTimesByPeople(new List<PersonDTO>() { person });
            return Json(times);
        }

        [HttpGet]
        public ActionResult GetChartsForPerson(int id)
        {
            var factory = new ChartFactory();
            var person = PersonHandler.Select(id);
            var db = BusinessLayer as DbTimeHandler;
            var times = db.GetTimesByPeople(new List<PersonDTO>() { person });
            var charts = factory.CreateChartDataModels_Person(times);

            return Json(charts, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult GetHighScoreForPersonForDiscipline(int personId, int disciplineId)
        {
            try
            {
                var person = PersonHandler.Select(personId);
                var discipline = DisciplineHandler.Select(disciplineId);
                var highscore = ((DbTimeHandler)BusinessLayer).GetHighscoreForPersonForDiscipline(person, discipline);
                return Json(highscore, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                HttpContext.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                return Json(e.Message, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
        public ActionResult GetTopNForDiscipline(int n, int disciplineId)
        {
            try
            {
                var discipline = DisciplineHandler.Select(disciplineId);
                var topN = ((DbTimeHandler)BusinessLayer).GetTopNPeopleForDiscipline(n, discipline);
                return Json(topN, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                HttpContext.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                return Json(e.Message, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
        public ActionResult GetTopNForDisciplineByGender(int n, int disciplineId, bool male)
        {
            try
            {
                var discipline = DisciplineHandler.Select(disciplineId);
                var topN = ((DbTimeHandler)BusinessLayer).GetTopNPeopleForDiscipline(n, discipline, male);
                return Json(topN, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                HttpContext.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                return Json(e.Message, JsonRequestBehavior.AllowGet);
            }
        }

        protected override DetailsModel GetDetailsModelFromDTO(DbObjDTO dto)
        {
            var time = dto as TimeDTO;
            var model = new DetailsModel
            {
                ControllerName = ControllerName,
                Title = time.Person.DisplayName + " at " + time.Discipline.DisplayName + " on " + time.Date.ToString("dd. MMM yy"),
                DisplayName = "Entry nr. " + time.Pk,
                DbObj = time
            };

            return model;
        }

        protected override DbObjDTO GetDtoFromCollection(int id, FormCollection collection)
        {
            var time = new TimeDTO
            {
                Pk = id,
                FK_D = int.Parse(collection["FK_D"]),
                FK_P = int.Parse(collection["FK_P"]),
                Date = Convert.ToDateTime(collection["Date"]),
                Seconds = decimal.Parse(collection["Seconds"])
            };

            time.Person = PersonHandler.Select(time.FK_P);
            time.Discipline = DisciplineHandler.Select(time.FK_D);

            return time;
        }
    }
}
