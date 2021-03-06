﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Web.Mvc;
using BusinessLayer;
using BusinessLayer.DbHandler;
using PresentationLayer.Extensions;
using Shared.Models.Chart;
using Shared.Models.db;

namespace PresentationLayer.Controllers
{
    public class ChartController : JsonDataContractController
    {

        private readonly DbPersonHandler _personHandler = new DbPersonHandler();
        private readonly DbDisciplineHandler _disciplineHandler = new DbDisciplineHandler();
        private readonly DbTimeHandler _timeHandler = new DbTimeHandler();

        [HttpGet]
        public ActionResult GetChartsForPerson(int id)
        {
            try {
                var factory = new ChartFactory();
                var person = _personHandler.Select(id);
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
                var person = _personHandler.Select(personId);
                var discipline = _disciplineHandler.Select(disciplineId);
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
                var discipline = _disciplineHandler.Select(id);
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
        public ActionResult GetChartsForDisciplineForPeople(int id, string pIds)
        {
            try
            {
                var ids = pIds.Split(',');
                var people = new List<PersonDTO>();
                foreach(var pId in ids)
                {
                    people.Add(_personHandler.Select(int.Parse(pId)));
                }
                var discipline = _disciplineHandler.Select(id);
                var times = _timeHandler.GetTimesByPeople(people, discipline);
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
                var discipline = _disciplineHandler.Select(id);
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