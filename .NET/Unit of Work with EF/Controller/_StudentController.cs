using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using STORI.Common.Models;
using STORI.Data;
using STORI.WebServices.Security;
using STORI.WebServices.WebService;

namespace STORI.WebServices.Controllers
{
    [StoriApiAuthorize]
    public class StudentController : WebServiceController, IWebServiceController<StudentModel>
    {
        private readonly IStoriWebService<Student> _webService;

        public StudentController(IStoriWebService<Student> webService)
        {
            _webService = webService;
        }

        [HttpGet]
        [Route("api/student")]
        public IHttpActionResult GetAll()
        {
            return Ok(_webService.GetAllData());
        }

        [HttpGet]
        [Route("api/student/{id}")]
        public IHttpActionResult Get(int id)
        {
            return Ok(_webService.GetDataById(id));
        }

        [HttpPost]
        [Route("api/student")]
        public IHttpActionResult Post(IEnumerable<StudentModel> students)
        {
            return InvalidModelResult() ?? Ok(_webService.ServiceResult(students, ServiceActionType.Insert));
        }


        [HttpPut]
        [Route("api/student")]
        public IHttpActionResult Put(IEnumerable<StudentModel> students)
        {
            return InvalidModelResult() ?? Ok(_webService.ServiceResult(students, ServiceActionType.Update));
        }


        [HttpPost]
        [Route("api/student/delete")]
        public IHttpActionResult Delete(IEnumerable<StudentModel> students)
        {
            return Ok(_webService.ServiceResult(students, ServiceActionType.Delete));
        }

        //protected IHttpActionResult InvaliModelResult()
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        {
        //            var post = Ok(new ActionData
        //            {
        //                Data = null,
        //                Error = new ActionErrorMessage { Message = "Invalid data.", ErrorData = ModelState.Keys.ToList() }
        //            });
        //            return post;
        //        }
        //    }
        //    return null;
        //}
    }


}
