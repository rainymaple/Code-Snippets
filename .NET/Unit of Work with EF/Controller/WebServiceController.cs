using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using STORI.Common.Models;
using STORI.Data.Common.Interfaces;
using STORI.Data.Interfaces;

namespace STORI.WebServices.Controllers
{
    public class WebServiceController : ApiController
    {
        public WebServiceController()
        {

        }
        protected IHttpActionResult InvalidModelResult()
        {
            if (!ModelState.IsValid)
            {
                {
                    var post = Ok(new ActionData
                    {
                        Data = null,
                        Error = new ActionErrorMessage { Message = "Invalid data.", ErrorData = ModelState.Keys.ToList() }
                    });
                    return post;
                }
            }
            return null;
        }
    }
}
