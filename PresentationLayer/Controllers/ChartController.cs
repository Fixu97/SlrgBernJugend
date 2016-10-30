using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using BusinessLayer;
using BusinessLayer.DbHandler;
using PresentationLayer.Extensions;
using Shared.Models;
using Shared.Models.Chart;
using Shared.Models.db;

namespace PresentationLayer.Controllers
{
    public class ChartController : JsonDataContractController
    {

        private readonly DbPersonHandler<PersonDTO> _personHandler = new DbPersonHandler<PersonDTO>();
        private readonly DbDisciplineHandler<DisciplineDTO> _disciplineHandler = new DbDisciplineHandler<DisciplineDTO>();
        private readonly DbTimeHandler<TimeDTO> _timeHandler = new DbTimeHandler<TimeDTO>();

        [HttpGet]
        public ActionResult GetChartsForPerson(int id)
        {
            try {
                var factory = new ChartFactory();
                var person = (PersonDTO) _personHandler.Select(id);
                var times = _timeHandler.GetTimesByPeople(new List<PersonDTO> { person });
                var charts = factory.CreateChartDataModels_Person(times);

                return Json(charts, JsonRequestBehavior.AllowGet);
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
        public ActionResult GetChartsForPersonForDiscipline(int personId, int disciplineId)
        {
            try {
                var factory = new ChartFactory();
                var person = (PersonDTO)_personHandler.Select(personId);
                var discipline = (DisciplineDTO) _disciplineHandler.Select(disciplineId);
                var times = _timeHandler.GetTimesByPeople(new List<PersonDTO> { person }, discipline);
                var charts = factory.CreateChartDataModels_Person(times);

                return Json(charts, JsonRequestBehavior.AllowGet);
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
        public ActionResult GetChartsForDiscipline(int id)
        {
            try {
                var discipline = (DisciplineDTO)_disciplineHandler.Select(id);
                var times = _timeHandler.GetTimesByDiscipline(new List<DisciplineDTO> { discipline });
                var charts = new List<ChartDataModel> { GetChartFromTimes(times) };

                return Json(charts, JsonRequestBehavior.AllowGet);
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
        public ActionResult GetChartsForDisciplineByGender(int id, bool male)
        {
            try
            {
                var discipline = (DisciplineDTO) _disciplineHandler.Select(id);
                var times = _timeHandler.GetTimesByDiscipline(new List<DisciplineDTO> {discipline}, male);
                var charts = new List<ChartDataModel> {GetChartFromTimes(times)};

                return Json(charts, JsonRequestBehavior.AllowGet);
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

        private ChartDataModel GetChartFromTimes(List<TimeDTO> times)
        {
            var factory = new ChartFactory();
            var chart = factory.CreateChartDataModelDiscipline(times);
            return chart;
        }
    }
}