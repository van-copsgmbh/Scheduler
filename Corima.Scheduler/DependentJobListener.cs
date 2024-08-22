using System.Threading;
using System.Threading.Tasks;
using Quartz;
using Quartz.Impl;
using Quartz.Impl.Triggers;

namespace Corima.Scheduler
{
    public class DependentJobListener : IJobListener
    {
        public Task JobToBeExecuted(IJobExecutionContext context, CancellationToken cancellationToken = new CancellationToken())
        {
            return Task.CompletedTask;
        }

        public Task JobExecutionVetoed(IJobExecutionContext context, CancellationToken cancellationToken = new CancellationToken())
        {
            return Task.CompletedTask;
        }

        public async Task JobWasExecuted(IJobExecutionContext context, JobExecutionException jobException,
            CancellationToken cancellationToken = new CancellationToken())
        {
            var nextJobName = context.MergedJobDataMap.GetString("nextJobName") ?? "";
            ITrigger trigger = await context.Scheduler.GetTrigger(new TriggerKey($"{nextJobName}Trigger", "group1"));
            if (trigger != null)
            {
                await context.Scheduler.TriggerJob(trigger.JobKey);
            }
        }

        public string Name { get; } = "DependentJobListener";
    }
}