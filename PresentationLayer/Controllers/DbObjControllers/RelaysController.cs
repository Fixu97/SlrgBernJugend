using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using BusinessLayer.DbHandler;
using PresentationLayer.Models;
using Shared.Models;
using Shared.Models.db;

namespace PresentationLayer.Controllers.DbObjControllers
{
    public class RelaysController : DbObjController<RelayDTO>
    {
        private DbObjHandler<RelayDTO> _relayHandler = new DbRelayHandler();

        //
        // GET: /Relays/

        protected override string ControllerName
        {
            get { return "Relays"; }
        }
        public override DbObjHandler<RelayDTO> BusinessLayer { protected get { return _relayHandler; } set { _relayHandler = value; } }

        public override ActionResult Index()
        {
            var relays = BusinessLayer.GetAll();

            var prop = new IndexModel()
            {
                Title = "Relays",
                ControllerName = this.ControllerName,
                Headers = new List<string>() { "Relay"}
            };

            foreach (var relay in relays)
            {
                prop.PkList.Add(relay.Pk);
                prop.Values.Add(new List<string>() {relay.Relay});
            }

            return View("~/Views/DbObjViews/Shared/Index.cshtml", prop);
        }

        //
        // GET: /Relays/Details/5

        public override ActionResult Details(int id)
        {
            var relay = BusinessLayer.Select(id);
            var prop = GetDetailsModelFromDTO(relay);

            return View("~/Views/DbObjViews/Shared/Details.cshtml", prop);
        }

        //
        // GET: /Relays/Edit/5

        public override ActionResult Edit(int id)
        {
            var relay = BusinessLayer.Select(id);
            var prop = GetDetailsModelFromDTO(relay);

            return View("~/Views/DbObjViews/Shared/Edit.cshtml", prop);
        }

        //
        // POST: /Relays/Delete/5

        [HttpPost]
        public override ActionResult Delete(int id)
        {
            try
            {
                BusinessLayer.Delete(new RelayDTO() {Pk = id});
                return Json("Successfully deleted relay!");
            }
            catch (Exception e)
            {
                throw new Exception("An error occoured while trying to delete relay", e);
            }
        }

        //
        // POST: /Relays/Create/

        [HttpPost]
        public override ActionResult Create(FormCollection collection)
        {
            try
            {
                var dto = GetDtoFromCollection(0, collection);
                BusinessLayer.Insert((RelayDTO)dto);
                var allObjects = BusinessLayer.GetAll();
                var insertedObject = allObjects.OrderBy(o => o.Pk).LastOrDefault(); // last object (by pk) from database => must be the latest inserted

                return Json(insertedObject);
            }
            catch (FormatException e)
            {
                throw new Exception("Could not convert form-data to object. Please check HTML form!" + e.Message, e);
            }
            catch (Exception e)
            {
                HttpContext.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                return Json(e.Message);
            }
        }

        //
        // POST: /Relays/Edit/

        [HttpPost]
        public override ActionResult Edit(int id, FormCollection collection)
        {
            try
            {
                var dto = GetDtoFromCollection(id, collection);
                BusinessLayer.Update((RelayDTO)dto);
                var editedObject = BusinessLayer.Select(dto.Pk);

                return Json(editedObject);
            }
            catch (FormatException e)
            {
                throw new Exception("Could not convert form-data to object. Please check HTML form!" + e.Message, e);
            }
            catch (Exception e)
            {
                HttpContext.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                return Json(e.Message);
            }
        }

        #region AJAX requests

        [HttpPost]
        public override ActionResult GetAll()
        {
            var relays = BusinessLayer.GetAll();

            return Json(relays);
        }

        #endregion


        protected override DetailsModel GetDetailsModelFromDTO(DbObjDTO dto)
        {
            var relay = dto as RelayDTO;
            var model = new DetailsModel()
            {
                Title = relay.Relay,
                DisplayName = relay.Relay,
                ControllerName = ControllerName,
                DbObj = relay
            };

            return model;
        }

        protected override DbObjDTO GetDtoFromCollection(int id, FormCollection collection)
        {

            var relay = new RelayDTO()
            {
                Pk = id,
                Relay = collection["Relay"]
            };

            return relay;
        }
    }
}
