using System;
using System.Collections.Generic;
using System.Web.Mvc;
using BusinessLayer.DbHandler;
using Microsoft.Ajax.Utilities;
using PresentationLayer.Models;
using Shared;
using Shared.Models.db;

namespace PresentationLayer.Controllers.DbObjControllers
{
    public class UsersController : DbObjController<UserDTO>
    {

        private DbObjHandler<UserDTO> _userHandler = new DbUserHandler();

        protected override string ControllerName
        {
            get { return "Users"; }
        }

        public override DbObjHandler<UserDTO> BusinessLayer { protected get { return _userHandler; } set { _userHandler = value; } }

        [HttpGet]
        public override ActionResult Index()
        {
            var dbObjs = BusinessLayer.GetAll();
            var users = new List<UserDTO>();
            dbObjs.ForEach(u => users.Add(u));

            var prop = new IndexModel()
            {
                ControllerName = ControllerName,
                Headers = new List<string>() { "Username", "Email", "Administrator" },
                Title = ControllerName
            };

            foreach (var user in users)
            {
                prop.PkList.Add(user.Pk);
                prop.Values.Add(new List<string>() { user.Username, user.Email, user.Admin.ToString()});
            }

            return View("~/Views/DbObjViews/Shared/Index.cshtml", prop);
        }
        [HttpGet]
        public override ActionResult Details(int id)
        {
            var user = BusinessLayer.Select(id);
            var model = GetDetailsModelFromDTO(user);

            return View("~/Views/DbObjViews/Shared/Details.cshtml", model);
        }
        [HttpGet]
        public override ActionResult Edit(int id)
        {
            var user = BusinessLayer.Select(id);
            var model = GetDetailsModelFromDTO(user);
            return View("~/Views/DbObjViews/Shared/Edit.cshtml", model);
        }
        [HttpPost]
        public override ActionResult Delete(int id)
        {
            try
            {
                var user = new UserDTO() { Pk = id };

                BusinessLayer.Delete(user);
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
            var users = BusinessLayer.GetAll();
            return Json(users);
        }

        protected override DetailsModel GetDetailsModelFromDTO(DbObjDTO dto)
        {
            var user = dto as UserDTO;
            var model = new DetailsModel()
            {
                ControllerName = ControllerName,
                Title = user.Username,
                DisplayName = "Entry nr. " + user.Pk,
                DbObj = user
            };

            return model;
        }

        protected override DbObjDTO GetDtoFromCollection(int id, FormCollection collection)
        {

            var user = new UserDTO()
            {
                Pk = id,
                Username = collection["Username"],
                Admin = collection["Admin"].Contains("true"),
                Email = collection["Email"]
            };

            /* Edit with no password or salt will not yet work, because the function 
             * Update in DBHandler will try to insert null values as Password and Salt
             */
            if (!collection["Password"].IsNullOrWhiteSpace() && !collection["ConfPassword"].IsNullOrWhiteSpace())
            {
                if (collection["Password"] == collection["ConfPassword"])
                {
                    user.Salt = ZeitenanalyseHelper.CreateSalt();
                    user.Password = ZeitenanalyseHelper.Sha256Hash(collection["Password"] + user.Salt);
                }
            }

            return user;
        }

    }
}
