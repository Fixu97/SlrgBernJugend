using System;
using System.Collections.Generic;
using System.Web.Mvc;
using BusinessLayer.DbHandler;
using PresentationLayer.Models;
using Shared.Models.db;

namespace PresentationLayer.Controllers.DbObjControllers
{
    public class PermissionsController : DbObjController<PermissionDTO>
    {
        protected override string ControllerName
        {
            get { return "Permissions"; }
        }
        protected override DbObjHandler<PermissionDTO> BusinessLayer => new DbPermissionHandler<PermissionDTO>();

        public override ActionResult Index()
        {
            var dbObjs = BusinessLayer.GetAll();
            var permissions = new List<PermissionDTO>();
            dbObjs.ForEach(p => permissions.Add(p));

            var model = new IndexModel()
            {
                ControllerName = ControllerName,
                Title = ControllerName,
                Headers = new List<string>() { "User","Permission"}
            };

            foreach (var permission in permissions)
            {
                model.PkList.Add(permission.Pk);
                model.Values.Add(new List<string>() { permission.User.Username, permission.Person.DisplayName });
            }

            return View("~/Views/DbObjViews/Shared/Index.cshtml", model);
        }

        //
        // GET: /Permissions/Details/5

        public override ActionResult Details(int id)
        {
            var permission = BusinessLayer.Select(id);
            var prop = GetDetailsModelFromDTO(permission);

            return View("~/Views/DbObjViews/Shared/Details.cshtml", prop);
        }

        //
        // GET: /Permissions/Edit/5

        public override ActionResult Edit(int id)
        {
            var permission = BusinessLayer.Select(id);

            var model = GetDetailsModelFromDTO(permission);
            return View("~/Views/DbObjViews/Shared/Edit.cshtml", model);
        }

        //
        // POST: /Permissions/Delete/5

        [HttpPost]
        public override ActionResult Delete(int id)
        {
            try
            {
                BusinessLayer.Delete(new PermissionDTO() { Pk = id });
                return Json("Successfully deleted permission!");
            }
            catch (Exception e)
            {
                throw new Exception("An error occoured while trying to delete this permission!",e);
            }
        }



        #region AJAX requests

        [HttpPost]
        public override ActionResult GetAll()
        {
            var permissions = BusinessLayer.GetAll();

            return Json(permissions);
        }

        #endregion

        protected override DetailsModel GetDetailsModelFromDTO(DbObjDTO dto)
        {
            var permission = dto as PermissionDTO;
            var model = new DetailsModel()
            {
                Title = permission.User.Username + " - " + permission.Person.DisplayName,
                DisplayName = permission.User.Username + " - " + permission.Person.DisplayName,
                ControllerName = ControllerName,
                DbObj = permission
            };

            return model;
        }

        protected override DbObjDTO GetDtoFromCollection(int id, FormCollection collection)
        {

            var permission = new PermissionDTO()
            {
                Pk = id,
                FK_P = int.Parse(collection["FK_P"]),
                FK_U = int.Parse(collection["FK_U"])
            };

            var personHandler = new DbPersonHandler<PersonDTO>();
            var userHandler = new DbUserHandler<UserDTO>();

            permission.Person = (PersonDTO) personHandler.Select(permission.FK_P);
            permission.User = (UserDTO) userHandler.Select(permission.FK_U);

            return permission;
        }
    }
}
