using System;
using System.Threading;
using System.Threading.Tasks;
using Quartz;

namespace Corima.Scheduler.Shared.JobListeners
{
    public class LoggingListener
    {
        public string Name { get; } = nameof(LoggingListener);
        
        public Task JobToBeExecuted(IJobExecutionContext context, CancellationToken cancellationToken = new CancellationToken())
        {
            Console.WriteLine($"[LOG] Job {context.JobDetail.Key} to be executed");
            return Task.CompletedTask;
        }

        public Task JobExecutionVetoed(IJobExecutionContext context, CancellationToken cancellationToken = new CancellationToken())
        {
            Console.WriteLine($"[LOG] Job {context.JobDetail.Key} vetoed");
            return Task.CompletedTask;
        }

        public Task JobWasExecuted(IJobExecutionContext context, JobExecutionException jobException,
            CancellationToken cancellationToken = new CancellationToken())
        {
            Console.WriteLine($"[LOG] Job {context.JobDetail.Key} executed");
            return Task.CompletedTask;
        }
    }
}