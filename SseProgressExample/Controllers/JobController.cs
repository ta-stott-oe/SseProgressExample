using SseProgressExample.Repository;
using SseProgressExample.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

namespace SseProgressExample.Controllers
{
    [RoutePrefix("api/job")]
    public class JobController : ApiController
    {
        private static IJobRepository<SomeResult, SomeProgress> repository = new InMemoryJobRepository<SomeResult, SomeProgress>();

        private readonly SomeJobService service;

        public JobController()
        {
            this.service = new SomeJobService(repository);
        }

        [Route("")]
        public HttpResponseMessage Put(SomeParameters parameters)
        {
            var job = this.service.DoJob(parameters);
            return Request.CreateResponse(job.Id);
        }

        [Route("{id}")]
        public async Task<HttpResponseMessage> Get(string id)
        {
            var job = this.service.GetJob(id);
            if (job == null)
            {
                return Request.CreateErrorResponse(HttpStatusCode.NotFound, $"Job {id} does not exist.");
            }

            await job.WaitUntilComplete();

            return Request.CreateResponse(job.Result);
        }

        [Route("{id}/progress")]
        public HttpResponseMessage GetProgressStream(string id)
        {
            var job = this.service.GetJob(id);
            if (job == null)
            {
                return Request.CreateErrorResponse(HttpStatusCode.NotFound, $"Job {id} does not exist.");
            }

            var response = Request.CreateResponse();

            response.Content = new PushStreamContent((stream, content, context) =>
            {
                var writer = new StreamWriter(stream);

                job.ProgressUpdated += (sender, progress) =>
                {
                    writer.Write($"data: {Newtonsoft.Json.JsonConvert.SerializeObject(progress)}\n\n");
                    writer.Flush();
                };

                job.Completed += (sender, args) =>
                {
                    writer.Dispose();
                    stream.Dispose();
                };

            }, "text/event-stream");

            return response;
        }
    }
}
