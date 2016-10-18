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

        public Job<SomeResult, SomeProgress> DoJob(SomeParameters parameters)
        {
            var job = new Job<SomeResult, SomeProgress>(Guid.NewGuid().ToString(), progressListener =>
            {
                return Library.DoLongRunningThing(parameters, progressListener);
            });

            this.repository.Add(job);
            job.Start();
            return job;
        }

        public Job<SomeResult, SomeProgress> GetJob(string id)
        {
            return this.repository.Get(id);
        }
    }

    
   
}