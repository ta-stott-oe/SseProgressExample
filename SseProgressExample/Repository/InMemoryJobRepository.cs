using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SseProgressExample.Models;

namespace SseProgressExample.Repository
{
    public class InMemoryJobRepository<TResult, TProgress> : IJobRepository<TResult, TProgress>
    {
        private IDictionary<string, Job<TResult, TProgress>> jobs;

        public InMemoryJobRepository()
        {
            this.jobs = new Dictionary<string, Job<TResult, TProgress>>();
        }

        public void Add(Job<TResult, TProgress> job)
        {
            this.jobs[job.Id] = job;
        }

        public Job<TResult, TProgress> Get(string id)
        {
            Job<TResult, TProgress> job;
            if (this.jobs.TryGetValue(id, out job))
            {
                return job;
            }
            else
            {
                return null;
            }
        }
    }
}