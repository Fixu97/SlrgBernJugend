using System;
using System.Collections.Generic;
using System.Web.Mvc;
using BusinessLayer.DbHandler;
using PresentationLayer.Models;
using Shared.Models.db;

namespace PresentationLayer.Controllers.DbObjControllers
{
    public class RelaysDisciplinesController : DbObjController<RelayDisciplineDTO>
    {

        private DbObjHandler<RelayDisciplineDTO> _relayDisciplineHAndler = new DbRelayDisciplineHandler();

        //
        // GET: /RelaysDisciplines/

        protected override string ControllerName
        {
            get { return "RelaysDisciplines"; }
        }
        public override DbObjHandler<RelayDisciplineDTO> BusinessLayer { protected get { return _relayDisciplineHAndler; } set { _relayDisciplineHAndler = value; } }

        public override ActionResult Index()
        {
            // RelaysDisciplines should not be handled directly => use relays
            throw new NotImplementedException();

            var relaysDisciplines = BusinessLayer.GetAll();

            var prop = new IndexModel()
            {
                Title = "RelaysDisciplines",
                ControllerName = this.ControllerName,
                Headers = new List<string>() { "Relay", "Discipline"}
            };

            foreach (var relayDiscipline in relaysDisciplines)
            {
                prop.PkList.Add(relayDiscipline.Pk);
                prop.Values.Add(new List<string>() {relayDiscipline.Relay.Relay});
            }

            return View("~/Views/DbObjViews/Shared/Index.cshtml", prop);
        }

        [HttpGet]
        public new ActionResult Create()
        {
            // RelaysDisciplines should not be handled directly => use relays
            throw new NotImplementedException();
        }

        //
        // GET: /RelaysDisciplines/Details/5
        [HttpGet]
        public override ActionResult Details(int id)
        {
            // RelaysDisciplines should not be handled directly => use relays
            throw new NotImplementedException();
            var relayDiscipline = BusinessLayer.Select(id);
            var prop = GetDetailsModelFromDTO(relayDiscipline);

            return View("~/Views/DbObjViews/Shared/Details.cshtml", prop);
        }

        //
        // GET: /RelaysDisciplines/Edit/5
        [HttpGet]
        public override ActionResult Edit(int id)
        {
            // RelaysDisciplines should not be handled directly => use relays
            throw new NotImplementedException();
            var relayDiscipline = BusinessLayer.Select(id);
            var prop = GetDetailsModelFromDTO(relayDiscipline);

            return View("~/Views/DbObjViews/Shared/Edit.cshtml", prop);
        }

        //
        // POST: /RelaysDisciplines/Delete/5

        [HttpPost]
        public override ActionResult Delete(int id)
        {
            try
            {
                // RelaysDisciplines should not be handled directly => use relays
                throw new NotImplementedException();

                BusinessLayer.Delete(new RelayDisciplineDTO() {Pk = id});
                return Json("Successfully deleted relayDiscipline!");
            }
            catch (Exception e)
            {
                throw new Exception("An error occoured while trying to delete relayDiscipline", e);
            }
        }

        #region AJAX requests

        [HttpPost]
        public override ActionResult GetAll()
        {
            var relaysDisciplines = BusinessLayer.GetAll();

            return Json(relaysDisciplines);
        }

        #endregion


        protected override DetailsModel GetDetailsModelFromDTO(DbObjDTO dto)
        {
            var relayDiscipline = dto as RelayDisciplineDTO;
            var model = new DetailsModel()
            {
                Title = relayDiscipline.Relay.Relay,
                DisplayName = relayDiscipline.Relay.Relay,
                ControllerName = ControllerName,
                DbObj = relayDiscipline
            };

            return model;
        }

        protected override DbObjDTO GetDtoFromCollection(int id, FormCollection collection)
        {

            var relayDiscipline = new RelayDisciplineDTO()
            {
                Pk = id,
                FK_R = int.Parse(collection["FK_R"]),
                FK_D = int.Parse(collection["FK_D"]),
                Position = int.Parse(collection["Position"])
            };

            return relayDiscipline;
        }
    }
}
