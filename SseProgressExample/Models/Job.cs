using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace SseProgressExample.Models
{
    /// <summary>
    /// Represents some long-running task that returns a TResult and emits
    /// progress messages of type TProgress
    /// </summary>
    /// <typeparam name="TResult"></typeparam>
    /// <typeparam name="TProgress"></typeparam>
    public class Job<TResult, TProgress>
    {
        public readonly string Id;

        private Task task;
        private readonly Func<IProgress<TProgress>, TResult> doWork;

        public Job(string id, Func<IProgress<TProgress>, TResult> doWork)
        {
            this.Id = id;
            this.doWork = doWork;
        }

        public void Start()
        {
            // Forward progress events to event handler
            var progressListener = new Progress<TProgress>(progress =>
            {
                this.ProgressUpdated?.Invoke(this, progress);
            });

            // Start doing the work and set the result property when finished
            // Should also handle errors here
            this.task = Task.Run(() =>
            {
                this.Result = doWork(progressListener);
                this.Completed?.Invoke(this, EventArgs.Empty);
            });
        }

        public TResult Result { get; private set; }

        public event EventHandler<TProgress> ProgressUpdated;
        public event EventHandler Completed;

        /// <summary>
        /// Asynchronously wait until the job is complete.
        /// </summary>
        /// <returns></returns>
        public async Task WaitUntilComplete()
        {
            if (this.task == null)
            {
                throw new InvalidOperationException("Cannot wait for completion because job hasn't started yet");
            }
            else if (this.task.IsCompleted)
            {
                return;
            }
            else
            {
                await this.task;
            }
        }
    }
}