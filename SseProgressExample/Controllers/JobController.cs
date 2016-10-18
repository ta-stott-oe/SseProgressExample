using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace SseProgressExample.Controllers
{
    [RoutePrefix("api/job")]
    public class JobController : ApiController
    {
        [Route("")]
        public HttpResponseMessage Put()
        {
            var guid = Guid.NewGuid();
            return Request.CreateResponse(guid.ToString());
        }

        [Route("{id}")]
        public HttpResponseMessage Get(string id)
        {
            return Request.CreateResponse("get");
        }
    }
}
