using System;
using System.Web.Mvc;
using BusinessLayer.DbHandler;
using Microsoft.Ajax.Utilities;
using PresentationLayer.Extensions;
using PresentationLayer.Models;
using Shared.Models.db;

namespace PresentationLayer.Controllers.DbObjControllers
{
    public abstract class DbObjController<T> : JsonDataContractController where T : DbObjDTO
    {
        protected abstract string ControllerName { get; }

        public abstract DbObjHandler<T> BusinessLayer { protected get; set; }

        #region Public methods
        [HttpGet]
        public virtual ActionResult Create()
        {
            if (ControllerName.IsNullOrWhiteSpace())
            {
                return RedirectToAction("Index");
            }

            var model = new InsertModel()
            {
                ControllerName = ControllerName,
                Title = ControllerName
            };
            return View("~/Views/DBObjViews/Shared/Create.cshtml", null, model);
        }

        [HttpPost]
        public virtual ActionResult Create(FormCollection collection)
        {
            try
            {
                var dto = GetDtoFromCollection(0, collection);
                try
                {
                    BusinessLayer.Insert((T)dto);

                    var model = new InsertModel(InsertModel.History.Message, "Successfully inserted data!", dto)
                    {
                        ControllerName = ControllerName,
                        Title = ControllerName
                    };

                    return View("~/Views/DbObjViews/Shared/Create.cshtml", null, model);

                }
                catch (Exception e)
                {
                    var model = new InsertModel(InsertModel.History.Error, e.Message, dto)
                    {
                        ControllerName = ControllerName,
                        Title = ControllerName
                    };
                    return View("~/Views/DbObjViews/Shared/Create.cshtml", null, model);
                }
            }
            catch (FormatException e)
            {
                throw new Exception("Could not convert form-data to object. Please check HTML form!" + e.Message, e);
            }
            catch (Exception e)
            {

                var model = new InsertModel(InsertModel.History.Error, e.Message)
                {
                    ControllerName = ControllerName,
                    Title = ControllerName
                };
                return View("~/Views/DbObjViews/Shared/Create.cshtml", null, model);
            }
        }
        [HttpPost]
        public virtual ActionResult Edit(int id, FormCollection collection)
        {
            try
            {
                var dto = GetDtoFromCollection(id, collection);
                BusinessLayer.Update((T)dto);
                return RedirectToAction("Index");
            }
            catch (Exception e)
            {
                return RedirectToAction("Edit");
            }

        }

        #endregion

        #region Abstract methods

        [HttpGet]
        public abstract ActionResult Index();
        [HttpGet]
        public abstract ActionResult Details(int id);
        [HttpGet]
        public abstract ActionResult Edit(int id);
        [HttpPost]
        public abstract ActionResult Delete(int id);
        [HttpPost]
        public abstract ActionResult GetAll();
        protected abstract DetailsModel GetDetailsModelFromDTO(DbObjDTO dto);
        protected abstract DbObjDTO GetDtoFromCollection(int id, FormCollection collection);

        #endregion

    }
}