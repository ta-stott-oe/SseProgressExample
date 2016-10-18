using SseProgressExample.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SseProgressExample.Repository
{
    public interface IJobRepository<TResult, TProgress>
    {
        void Add(Job<TResult, TProgress> job);
        Job<TResult, TProgress> Get(string id);
    }
}