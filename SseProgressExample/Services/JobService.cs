using SseProgressExample.Models;
using SseProgressExample.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace SseProgressExample.Services
{
    public class SomeJobService
    {
        private readonly IJobRepository<SomeResult, SomeProgress> repository;

        public SomeJobService(IJobRepository<SomeResult, SomeProgress> repository)
        {
            this.repository = repository;
        }

        /// <summary>
        /// Creates and starts a job using supplied parameters.
        /// </summary>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public Job<SomeResult, SomeProgress> DoJob(SomeParameters parameters)
        {
            // Set up each job to invoke the library method
            var job = new Job<SomeResult, SomeProgress>(Guid.NewGuid().ToString(), progressListener =>
            {
                return Library.DoLongRunningThing(parameters, progressListener);
            });

            // Add it to repository so we can get it later
            this.repository.Add(job);

            // Start job now, but don't wait for it to finish
            job.Start();
            return job;
        }

        /// <summary>
        /// Creates and starts a job using supplied parameters.
        /// </summary>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public Job<SomeResult, SomeProgress> DoJobAsync(SomeParameters parameters)
        {
            // Set up each job to invoke the library method
            var job = new Job<SomeResult, SomeProgress>(Guid.NewGuid().ToString(), progressListener =>
            {
                return Library.DoLongRunningThingAsync(parameters, progressListener);
            });

            // Add it to repository so we can get it later
            this.repository.Add(job);

            // Start job now, but don't wait for it to finish
            job.Start();
            return job;
        }

        /// <summary>
        /// Gets an existing job.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>

        public Job<SomeResult, SomeProgress> GetJob(string id)
        {
            return this.repository.Get(id);
        }
    }

    
   
}