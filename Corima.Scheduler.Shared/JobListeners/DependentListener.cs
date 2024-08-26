using System.Threading;
using System.Threading.Tasks;
using Quartz;

namespace Corima.Scheduler.Shared.JobListeners
{
    public class DependentListener : IJobListener
    {
        public string Name { get; } = nameof(DependentListener);
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
    }
}