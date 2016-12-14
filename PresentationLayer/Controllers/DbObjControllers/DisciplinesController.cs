using System;
using System.Collections.Generic;
using System.Web.Mvc;
using BusinessLayer.DbHandler;
using PresentationLayer.Models;
using Shared.Models.db;

namespace PresentationLayer.Controllers.DbObjControllers
{
    public class DisciplinesController : DbObjController<DisciplineDTO>
    {
        private DbObjHandler<DisciplineDTO> _disciplineHandler = new DbDisciplineHandler();

        protected override string ControllerName
        {
            get { return "Disciplines"; }
        }

        public override DbObjHandler<DisciplineDTO> BusinessLayer { protected get { return _disciplineHandler; } set { _disciplineHandler = value; } }

        //
        // GET: /Disciplines/

        public override ActionResult Index()
        {
            var dbObjs = BusinessLayer.GetAll();
            var disciplines = new List<DisciplineDTO>();
            dbObjs.ForEach( d => disciplines.Add((DisciplineDTO) d));

            var prop = new IndexModel()
            {
                Title = "Disciplines",
                ControllerName = "Disciplines",
                Headers = new List<string>() { "Discipline"}
            };

            foreach (var discipline in disciplines)
            {
                prop.PkList.Add(discipline.Pk);
                prop.Values.Add(new List<string>() {discipline.DisplayName});
            }

            return View("~/Views/DbObjViews/Shared/Index.cshtml", prop);
        }

        //
        // GET: /Disciplines/Details/5

        public override ActionResult Details(int id)
        {
            var discipline = BusinessLayer.Select(id);
            var prop = GetDetailsModelFromDTO(discipline);

            return View("~/Views/DbObjViews/Shared/Details.cshtml", prop);
        }

        //
        // GET: /Disciplines/Edit/5

        public override ActionResult Edit(int id)
        {
            var discipline = BusinessLayer.Select(id);
            var prop = GetDetailsModelFromDTO(discipline);

            return View("~/Views/DbObjViews/Shared/Edit.cshtml", prop);
        }

        //
        // POST: /Disciplines/Delete/5

        [HttpPost]
        public override ActionResult Delete(int id)
        {
            try
            {
                BusinessLayer.Delete(new DisciplineDTO() {Pk = id});
                return Json("Successfully deleted discipline!");
            }
            catch (Exception e)
            {
                throw new Exception("An error occoured while trying to delete discipline", e);
            }
        }

        #region AJAX requests

        [HttpPost]
        public override ActionResult GetAll()
        {
            var disciplines = BusinessLayer.GetAll();

            return Json(disciplines);
        }

        #endregion


        protected override DetailsModel GetDetailsModelFromDTO(DbObjDTO dto)
        {
            var discipline = dto as DisciplineDTO;
            var model = new DetailsModel()
            {
                Title = discipline.DisplayName,
                DisplayName = discipline.DisplayName,
                ControllerName = ControllerName,
                DbObj = discipline
            };

            return model;
        }

        protected override DbObjDTO GetDtoFromCollection(int id, FormCollection collection)
        {
            var discipline = new DisciplineDTO()
            {
                Pk = id,
                DiscName = collection["DiscName"],
                Meters = Int32.Parse(collection["Meters"])
            };

            return discipline;
        }
    }
}
