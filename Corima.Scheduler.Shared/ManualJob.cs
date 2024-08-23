using Quartz;
using System;
using System.Threading.Tasks;

namespace Corima.Scheduler.Shared
{
    public class ManualJob : CorimaJob, ICorimaAutoJob //todo make it manual job
    {

        public ManualJob(ISchedulerFactory aa)
        {
        }

        protected override string JobName => "ManualJob";

        public static JobKey Key { get; } = new JobKey("ManualJob", "Group1");

        JobKey ICorimaAutoJob.Key { get; } = Key;
        ITrigger ICorimaAutoJob.Trigger => null;

        protected override Task OnError(IJobExecutionContext context, Exception exception)
        {
            return Task.CompletedTask;
        }

        protected override async Task DoWork(IJobExecutionContext context)
        {
            await Task.Delay(TimeSpan.FromSeconds(5));
            throw new Exception();
        }

        protected override bool IsRetriable { get; } = false;
    }

    //public class ManualJob2 : CorimaJob, ICorimaAutoJob //todo m
    //{
    //    protected override string JobName { get; }
    //    protected override Task OnError(IJobExecutionContext context, Exception exception)
    //    {
    //        return Task.CompletedTask;
    //    }

    //    protected override Task DoWork(IJobExecutionContext context)
    //    {
    //        return Task.CompletedTask;
    //    }

    //    public static JobKey Key { get; } = new JobKey("ManualJob", "Group1");

    //    JobKey ICorimaAutoJob.Key { get; } = Key;
    //    ITrigger ICorimaAutoJob.Trigger => null;
    //}
}