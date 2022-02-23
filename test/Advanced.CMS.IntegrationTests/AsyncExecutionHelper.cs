using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EPiServer.Async;
using EPiServer.Async.Internal;
using EPiServer.Data.Dynamic;
using EPiServer.ServiceLocation;

namespace Advanced.CMS.IntegrationTests
{
    /// <summary>
    /// Provides helper methods to working with async tasks that happens in the background
    /// </summary>
    public class AsyncExecutionHelper
    {
        public static readonly TimeSpan DefaultTimeout = TimeSpan.FromSeconds(60);

        public static void WaitFor(Func<bool> waitFor, TimeSpan? timeout = null)
        {
            var watch = new Stopwatch();
            watch.Start();

            var t = timeout ?? DefaultTimeout;

            for (var i = 1; ; i++)
            {
                if (waitFor())
                {
                    break;
                }
                if (!Debugger.IsAttached && watch.Elapsed > t)
                {
                    throw new TimeoutException("Timeout waiting for async operation");
                }
                Thread.Sleep((int)Math.Pow(2d, i));
            }
        }

        /// <summary>
        /// Allow all running tasks to store results in database
        /// </summary>
        public static void WaitForAllRunning(bool cleanupTaskStorage = false)
        {
            WaitFor(() =>
                {
                    var storage = ServiceLocator.Current.GetInstance<TaskInformationStorage>();
                    return storage.Tasks().Where(t => t.CompletedStatus != TaskStatus.RanToCompletion && t.CompletedStatus != TaskStatus.Faulted && t.CompletedStatus != TaskStatus.Canceled).Count() == 0;
                });
            if (cleanupTaskStorage)
            {
                var taskStore = DynamicDataStoreFactory.Instance.GetStore(typeof(TaskInformation));
                if (taskStore != null)
                {
                    taskStore.DeleteAll();
                }
            }
        }
    }
}
