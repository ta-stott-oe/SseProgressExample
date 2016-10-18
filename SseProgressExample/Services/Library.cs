using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Web;

namespace SseProgressExample.Services
{
    /// <summary>
    /// This is the library class which does something that takes a while.
    /// </summary>
    public class Library
    {
        /// <summary>
        /// Takes some parameters and a progress listener and returns a result synchronously.
        /// </summary>
        /// <param name="parameters"></param>
        /// <param name="progressListener"></param>
        /// <returns></returns>
        public static SomeResult DoLongRunningThing(SomeParameters parameters, IProgress<SomeProgress> progressListener)
        {
            for(var i = 0; i < 10; i++)
            {
                progressListener.Report(new SomeProgress(i / 10m, $"Doing Step {i + 1} of 10"));
                Thread.Sleep(500);
            }

            return new SomeResult($"{parameters.Foo} has been foobarred");
        } 
    }

    public class SomeParameters
    {
        public int Foo { get; set; }
    }

    public class SomeResult
    {
        public string FooBar { get; private set; }

        public SomeResult(string fooBar)
        {
            this.FooBar = fooBar;
        }
    }

    public class SomeProgress
    {
        public readonly decimal Progress;
        public readonly string Message;

        public SomeProgress(decimal progress, string message)
        {
            this.Progress = progress;
            this.Message = message;
        }
    }
}