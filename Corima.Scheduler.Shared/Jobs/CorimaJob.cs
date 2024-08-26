using System;
using System.Threading.Tasks;
using Quartz;

namespace Corima.Scheduler.Shared.Jobs
{
    public abstract class CorimaJob : ICorimaJob
    {
        public virtual JobKey Key => new JobKey(JobName, JobGroup);
        public bool DisallowConcurrentExecution { get; set; } = true;
        public bool PersistJobDataAfterExecution { get; set; }
        public bool StoreDurably { get; set; }
        public virtual string JobGroup { get; set; } = "Group1";
        public string TriggerName => $"{JobName}Trigger";
        public abstract string JobName { get; }
        public abstract ITrigger Trigger { get; }
        protected bool IsRetriable => true;
        protected int RetryCount { get; } = 5;
        
        public async Task Execute(IJobExecutionContext context)
        {
            if (context.RefireCount > RetryCount)
            {
                await this.OnRetryLimitReached(context);
                return;
            }

            try
            {
                Console.WriteLine($"{JobName} - start");
                await DoWork(context);
                Console.WriteLine($"{JobName} - end");
            }
            catch (Exception ex)
            {
                await OnError(context, ex);
                throw new JobExecutionException(refireImmediately: IsRetriable, cause: ex);
            }
        }
        
        protected virtual Task OnRetryLimitReached(IJobExecutionContext context) => Task.CompletedTask;
        
        protected abstract Task DoWork(IJobExecutionContext context);
        protected abstract Task OnError(IJobExecutionContext context, Exception exception);
    }
}