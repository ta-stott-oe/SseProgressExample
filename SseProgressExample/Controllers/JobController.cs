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
        /// <summary>
        /// In a full application this should be injected as a singleton (instead of using a static field).
        /// </summary>
        private static IJobRepository<SomeResult, SomeProgress> repository = new InMemoryJobRepository<SomeResult, SomeProgress>();

        private readonly SomeJobService service;

        public JobController()
        {
            // This would normally be injected.
            this.service = new SomeJobService(repository);
        }

        /// <summary>
        /// Creates a new job using the given parameters and returns the job's id.
        /// </summary>
        /// <param name="parameters"></param>
        /// <returns></returns>
        [Route("")]
        public HttpResponseMessage Put(SomeParameters parameters)
        {
            // var job = this.service.DoJobAsync(parameters);
            var job = this.service.DoJob(parameters);
            return Request.CreateResponse(job.Id);
        }

        /// <summary>
        /// Locates an existing job by id, waits for it to complete and returns the job result.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Route("{id}")]
        public async Task<HttpResponseMessage> Get(string id)
        {
            var job = this.service.GetJob(id);
            if (job == null)
            {
                return Request.CreateErrorResponse(HttpStatusCode.NotFound, $"Job {id} does not exist.");
            }

            // Should handle errors here

            await job.WaitUntilComplete();

            return Request.CreateResponse(job.Result);
        }

        /// <summary>
        /// Locates an existing job by id and returns its progress messages as an event stream until it completes.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Route("{id}/progress")]
        public HttpResponseMessage GetProgressStream(string id)
        {
            var job = this.service.GetJob(id);
            if (job == null)
            {
                return Request.CreateErrorResponse(HttpStatusCode.NotFound, $"Job {id} does not exist.");
            }

            var response = Request.CreateResponse();

            // Set response content as event stream
            response.Content = new PushStreamContent((stream, content, context) =>
            {
                // This almost certainly needs better error handling to ensure things are always disposed.
                var writer = new StreamWriter(stream);

                // Write a message to the event stream each time the progress is updated.
                // A real-life implementation may want to throttle messages here if they occur very frequently.
                job.ProgressUpdated += (sender, progress) =>
                {
                    // Event stream spec expects {field}: {value}\n\n where "data" field can be some arbitrary data.
                    // Here data is a JSON string of the progress object.
                    // There are other event stream fields which we may or may not want to use.
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
