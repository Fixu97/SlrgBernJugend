using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Web.UI.WebControls;
using BusinessLayer;
using BusinessLayer.DbHandler;
using BusinessLayer.Excel;
using PresentationLayer.Extensions;
using Shared.Models.db;

namespace PresentationLayer.Controllers
{
    public class ImportController : JsonDataContractController
    {
        private readonly DbPersonHandler<PersonDTO> _personHandler = new DbPersonHandler<PersonDTO>();
        private readonly DbDisciplineHandler<DisciplineDTO> _disciplineHandler = new DbDisciplineHandler<DisciplineDTO>();
        [HttpGet]
        public ActionResult Index()
        {

            #region Disciplines
            var disciplines = _disciplineHandler.GetAll();
            ViewBag.disciplines = new List<ListItem>();
            foreach (DisciplineDTO discipline in disciplines)
            {
                ViewBag.disciplines.Add(new ListItem {Text = discipline.DisplayName, Value = discipline.Pk.ToString()});
            }
            #endregion

            #region People
            var people = _personHandler.GetAll();
            ViewBag.people = new List<ListItem>();
            foreach (PersonDTO person in people)
            {
                ViewBag.people.Add(new ListItem { Text = person.DisplayName, Value = person.Pk.ToString(), Selected = true});
            }
            #endregion

            return View();
        }

        [HttpPost]
        public ActionResult Import()
        {
            try
            {
                var file = Request.Files[0];

                if (file == null)
                {
                    throw new InvalidDataException("You must upload a file!");
                }

                //Save to a file
                var pathToFile = ConfigurationManager.AppSettings.Get("ImportExcelPath");
                var fullFilePath = pathToFile + "Import.xls";
                file.SaveAs(fullFilePath);
                
                var importer = new ExcelReader(fullFilePath);

                return Json(importer.GetResult());

            }
            catch (Exception e)
            {
                HttpContext.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                return Json(e);
            }
        }

        [HttpGet]
        public FileResult GetImportFile()
        {
            var disciplineId = Request.Params["discipline"];
            var discipline = (DisciplineDTO) _disciplineHandler.Select(int.Parse(disciplineId));

            var peopleIdarr = Request.Params["people"].Split(',');
            
            var people = new List<PersonDTO>();
            for (var i = 0; i < peopleIdarr.Length; i++)
            {
                var personId = int.Parse(peopleIdarr[i]);
                var person = (PersonDTO) _personHandler.Select(personId);
                people.Add(person);
            }
            
            var writer = new ExcelWriter(discipline, people, new List<DateTime>());
            var filePath = writer.GetExcelFilePath();

            var fileBytes = System.IO.File.ReadAllBytes(filePath);
            var fileName = "import.xls";
            return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, fileName);
        }
    }
}