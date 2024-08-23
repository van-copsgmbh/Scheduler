using Quartz;
using Quartz.Util;
using System.Threading.Tasks;
using System;
using System.Reflection;

namespace Corima.Scheduler.Shared
{
    public abstract class CorimaJob : IJob
    {
        protected abstract string JobName { get; }
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
                await Task.Delay(100);
                await DoWork(context);
                Console.WriteLine($"{JobName} - end");
            }
            catch (Exception ex)
            {
                await OnError(context, ex);
                throw new JobExecutionException(msg: ErrorMessage, refireImmediately: IsRetriable, cause: ex);
            }
        }

        protected abstract Task OnError(IJobExecutionContext context, Exception exception);

        protected virtual int RetryCount { get; } = 5;
        protected virtual bool IsRetriable { get; } = true;
        protected virtual string ErrorMessage { get; } = "Task ... failed";

        protected abstract Task DoWork(IJobExecutionContext context);

        protected virtual Task OnRetryLimitReached(IJobExecutionContext context) => Task.CompletedTask;
    }



    //public class CancelledJob : CorimaJob, ICorimaAutoJob
    //{
    //    protected override string JobName => "CancelledJob";
    //    public JobKey Key { get; } = new JobKey("CancelledJob", "Group1");
    //    public ITrigger Trigger { get; } = new Triggers().RepeatedTrigger("CancelledJob" + "Trigger", 5);

    //    protected override Task OnError(IJobExecutionContext context, Exception exception)
    //    {
    //        return Task.CompletedTask;
    //    }

    //    protected override async Task DoWork(IJobExecutionContext context)
    //    {
    //        context.CancellationToken. = true;
    //        Console.WriteLine("One time start up job running");
    //        await Task.Delay(TimeSpan.FromSeconds(5));
    //        Console.WriteLine("One time start up job finished");
    //    }
    //}

    public class ChainedJob1 : CorimaJob, ICorimaAutoJob
    {
        protected override string JobName => "ChainedJob1";

        public JobKey Key { get; } = new JobKey("ChainedJob1", "Group1");
        public ITrigger Trigger { get; } = new Triggers().RepeatedTrigger("ChainedJob1" + "ChainedJob1", 5);

        protected override Task OnError(IJobExecutionContext context, Exception exception)
        {
            return Task.CompletedTask;
        }

        protected override async Task DoWork(IJobExecutionContext context)
        {
            var scheduler = context.Scheduler;
            //var job = JobBuilder.Create<ChildJob>()
            //    .WithIdentity(ChildJob.Key)
            //    .UsingJobData("batch-size", "50")
            //    .Build();

            //await scheduler.AddJob(job, replace: true, storeNonDurableWhileAwaitingScheduling: true);

            // Trigger 1
            var jobData1 = new JobDataMap { { "CustomerId", 1 } };
            await scheduler.TriggerJob(ChildJob.Key, jobData1);

            // Trigger 2
            var jobData2 = new JobDataMap { { "CustomerId", 2 } };
            await scheduler.TriggerJob(ChildJob.Key, jobData2);
        }
    }


    public class ChildJob : CorimaJob, ICorimaAutoJob
    {
        protected override string JobName => "ChildJob";

        public static JobKey Key { get; } = new JobKey("ChildJob", "Group1");
        JobKey ICorimaAutoJob.Key { get; } = Key;
        ITrigger ICorimaAutoJob.Trigger => null;

        protected override Task OnError(IJobExecutionContext context, Exception exception)
        {
            return Task.CompletedTask;
        }

        protected override async Task DoWork(IJobExecutionContext context)
        {
            await Task.Delay(TimeSpan.FromSeconds(5));
            var customerId = context.MergedJobDataMap.GetString("CustomerId");
            Console.WriteLine($"CustomerId={customerId}");
        }
    }
}