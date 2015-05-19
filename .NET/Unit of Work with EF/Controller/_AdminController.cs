using System;
using System.Web.Helpers;
using System.Web.Http;
using STORI.Common.Models;
using STORI.Data;
using STORI.Data.Common.Interfaces;
using STORI.Data.Interfaces;
using STORI.WebServices.Security;

namespace STORI.WebServices.Controllers
{
    [StoriWebAuthorize(Roles = "Admin")]
    public class AdminController : ApiController
    {

        private readonly IStoriUow _uow;
        private readonly IRepository<ApiUser> _apiUsers;
        private readonly IRepository<ApiAppRegistration> _apiAppRegistrations;

        public AdminController(IStoriUow uow)
        {
            _uow = uow;
            _apiUsers = GetRepository<ApiUser>();
            _apiAppRegistrations = GetRepository<ApiAppRegistration>();
        }

        #region ApiUsers

        [Route("api/admin/users")]
        public IHttpActionResult GetUsers()
        {
            var users = _apiUsers.GetAll();
            return Ok(new ActionData { Data = users });
        }

        [Route("api/admin/user/{id}")]
        [HttpGet]
        public IHttpActionResult GetUserById(int id)
        {
            if (id > 0)
            {

                var user = _apiUsers.GetById(id);
                return Ok(new ActionData { Data = user });
            }
            return Ok();
        }

        [Route("api/deleteuser/{id}")]
        [HttpGet]
        public IHttpActionResult DeleteUserById(int id)
        {
            if (id > 0)
            {

                _apiUsers.Delete(id);
                _uow.Commit();
                var users = _apiUsers.GetAll();
                return Ok(new ActionData { Data = users });
            }
            return Ok();
        }
        [HttpPost]
        [Route("api/admin/adduser")]
        public IHttpActionResult SaveUser(ApiUser user)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var isInsert = user.Id == 0;
                    if (isInsert)
                    {
                        user.Password = Crypto.HashPassword(user.Password);
                        _apiUsers.Add(user);
                    }
                    else
                    {
                        var existingUser = _apiUsers.GetById(user.Id);
                        existingUser.UserName = user.UserName;
                        existingUser.Password = Crypto.HashPassword(user.Password);
                        existingUser.Role = user.Role;
                    }
                    _uow.Commit();
                    return Ok(new ActionData());

                }
                catch (Exception ex)
                {
                    return Ok(new ActionData { Data = null, Error = new ActionErrorMessage { Message = ex.Message } });

                }
            }
            return Ok(new ActionData { Data = null, Error = new ActionErrorMessage { Message = "Model invalid" } });
        }

        #endregion

        #region ApiAppRegistrations


        [Route("api/admin/apps")]
        public IHttpActionResult GetApps()
        {
            var apps = _apiAppRegistrations.GetAll();
            return Ok(new ActionData { Data = apps });
        }

        [HttpPost]
        [Route("api/admin/addapp")]
        public IHttpActionResult SaveApp(ApiAppRegistration app)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var isInsert = app.Id == 0;
                    if (isInsert)
                    {
                        app.AppSecret = Crypto.HashPassword(app.AppSecret);
                        _apiAppRegistrations.Add(app);
                    }
                    else
                    {
                        var existingApp = _apiAppRegistrations.GetById(app.Id);
                        if (existingApp != null)
                        {
                            existingApp.AppCode = app.AppCode;
                            existingApp.AppName = app.AppName;
                            existingApp.AppSecret = Crypto.HashPassword(app.AppSecret);
                            existingApp.AppDescription = app.AppDescription;
                        }
                    }
                    _uow.Commit();
                    return Ok(new ActionData());
                }
                catch (Exception ex)
                {
                    return Ok(new ActionData { Data = null, Error = new ActionErrorMessage { Message = ex.Message } });

                }
            }
            return Ok(new ActionData { Data = null, Error = new ActionErrorMessage { Message = "Model invalid" } });
        }

        [Route("api/deleteapp/{id}")]
        [HttpGet]
        //[Authorize]
        public IHttpActionResult DeleteAppById(int id)
        {
            if (id > 0)
            {

                _apiAppRegistrations.Delete(id);
                _uow.Commit();
                var apps = _apiAppRegistrations.GetAll();
                return Ok(new ActionData { Data = apps });
            }
            return Ok();
        }
        [Route("api/admin/app/{id}")]
        [HttpGet]
        //[Authorize]
        public IHttpActionResult GetAppById(int id)
        {
            if (id > 0)
            {

                var app = _apiAppRegistrations.GetById(id);
                return Ok(new ActionData { Data = app });
            }
            return Ok();
        }

        #endregion



        #region Helpers

        protected IRepository<T> GetRepository<T>() where T : class
        {
            try
            {
                var repository = _uow.GetRepository<T>();
                return repository;

            }
            catch (Exception ex)
            {
                // todo: log the errro message
                return null;
            }
        }

        #endregion
    }
}
