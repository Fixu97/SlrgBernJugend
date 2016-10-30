using System;
using System.Collections.Generic;
using System.Net;
using System.Web.Mvc;
using BusinessLayer;
using BusinessLayer.DbHandler;
using PresentationLayer.Models;
using Shared.Models.db;

namespace PresentationLayer.Controllers.DbObjControllers
{
    public class TimesController : DbObjController<TimeDTO>
    {
        protected override string ControllerName
        {
            get { return "Times"; }
        }

        protected override DbObjHandler<TimeDTO> BusinessLayer => new DbTimeHandler<TimeDTO>();

        [HttpGet]
        public override ActionResult Index()
        {
            var dbObjs = BusinessLayer.GetAll();
            var times = new List<TimeDTO>();
            dbObjs.ForEach(t => times.Add((TimeDTO) t));

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
        [HttpPost]
        public override ActionResult GetAll()
        {
            var times = BusinessLayer.GetAll();
            return Json(times);
        }

        [HttpPost]
        public ActionResult GetTimesByPerson(int id)
        {
            var personHandler = new DbPersonHandler<PersonDTO>();
            var person = personHandler.Select(id);
            var db = BusinessLayer as DbTimeHandler<TimeDTO>;
            var times = db.GetTimesByPeople(new List<PersonDTO>() { person });
            return Json(times);
        }

        [HttpGet]
        public ActionResult GetChartsForPerson(int id)
        {
            var factory = new ChartFactory();
            var personHandler = new DbPersonHandler<PersonDTO>();
            var person = personHandler.Select(id);
            var db = BusinessLayer as DbTimeHandler<TimeDTO>;
            var times = db.GetTimesByPeople(new List<PersonDTO>() { person });
            var charts = factory.CreateChartDataModels_Person(times);

            return Json(charts, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult GetHighScoreForPersonForDiscipline(int personId, int disciplineId)
        {
            try
            {
                var personHandler = new DbPersonHandler<PersonDTO>();
                var disciplineHandler = new DbDisciplineHandler<DisciplineDTO>();
                var timesHandler = new DbTimeHandler<TimeDTO>();

                var person = (PersonDTO) personHandler.Select(personId);
                var discipline = (DisciplineDTO) disciplineHandler.Select(disciplineId);
                var highscore = timesHandler.GetHighscoreForPersonForDiscipline(person, discipline);
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
                var disciplineHandler = new DbDisciplineHandler<DisciplineDTO>();
                var timesHandler = new DbTimeHandler<TimeDTO>();

                var discipline = (DisciplineDTO) disciplineHandler.Select(disciplineId);
                var topN = timesHandler.GetTopNPeopleForDiscipline(n, discipline);
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
                var disciplineHandler = new DbDisciplineHandler<DisciplineDTO>();
                var timesHandler = new DbTimeHandler<TimeDTO>();

                var discipline = (DisciplineDTO)disciplineHandler.Select(disciplineId);
                var topN = timesHandler.GetTopNPeopleForDiscipline(n, discipline, male);
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

            var personHandler = new DbPersonHandler<PersonDTO>();
            var disciplineHandler = new DbDisciplineHandler<DisciplineDTO>();

            time.Person = (PersonDTO) personHandler.Select(time.FK_P);
            time.Discipline = (DisciplineDTO) disciplineHandler.Select(time.FK_D);

            return time;
        }
    }
}
