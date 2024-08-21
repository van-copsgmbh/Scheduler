using System;
using System.Threading.Tasks;
using Quartz;
using TreasuryBrowser;

namespace Corima
{
    public class JobScenarios
    {
        private IScheduler _scheduler;

        public JobScenarios(IScheduler scheduler)
        {
            _scheduler = scheduler;
        }

        public async Task RunNewJob()
        {
            var job = JobBuilder.Create<X>()
                .WithIdentity("XXX", "group")
                .Build();

            var trigger = TriggerBuilder.Create()
                .WithIdentity("XXX", "group")
                .StartNow()
                .Build();

            await _scheduler.ScheduleJob(job, trigger);
        }
        
        public async Task RunJobManually(string jobName)
        {
            ITrigger trigger = await GetTrigger(jobName);
            await _scheduler.TriggerJob(trigger.JobKey);
        }

        public async Task RunJobManyTimes(string jobName, int numberOfShots)
        {
            ITrigger trigger = await GetTrigger(jobName);
            for (int i = 0; i < numberOfShots; i++)
            {
                await _scheduler.TriggerJob(trigger.JobKey);
            }
        }

        public async Task PreventMultipleStarts(string jobName, int numberOfShots)
        {
            ITrigger trigger = await GetTrigger(jobName);

            if (trigger.GetNextFireTimeUtc() <= DateTime.UtcNow)
            {
                await _scheduler.TriggerJob(trigger.JobKey);
            }
        }
        
        private async Task<ITrigger> GetTrigger(string jobName)
        {
            return await _scheduler.GetTrigger(new TriggerKey($"{jobName}Trigger", "group1"));
        }
    }

    class X : IJob
    {
        public Task Execute(IJobExecutionContext context)
        {
            Console.WriteLine("NEW JOB***********");
            
            return Task.CompletedTask;
        }
    }
}