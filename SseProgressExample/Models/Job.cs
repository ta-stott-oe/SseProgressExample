using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace SseProgressExample.Models
{
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
            var progressListener = new Progress<TProgress>(progress =>
            {
                this.ProgressUpdated?.Invoke(this, progress);
            });

            this.task = Task.Run(() =>
            {
                this.Result = doWork(progressListener);
                this.Completed?.Invoke(this, EventArgs.Empty);
            });
        }

        public TResult Result { get; private set; }

        public event EventHandler<TProgress> ProgressUpdated;
        public event EventHandler Completed;

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