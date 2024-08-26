using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Corima.Scheduler;
using Corima.Scheduler.Shared.JobListeners;
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
            var job = JobBuilder.Create<RuntimeJob>()
                .WithIdentity("NewJob", "group1")
                .Build();

            var trigger = TriggerBuilder.Create()
                .WithIdentity("NewJob", "group1")
                .StartNow()
                .Build();

            await _scheduler.ScheduleJob(job, trigger);
        }
        
        public async Task RunJobManually(string jobName, Dictionary<string, object> data)
        {
            ITrigger trigger = await GetTrigger(jobName);
            await _scheduler.TriggerJob(trigger.JobKey, CreateDataMap(data));
        }

        public async Task RunJobManyTimes(string jobName, int numberOfShots, Dictionary<string, object> data)
        {
            ITrigger trigger = await GetTrigger(jobName);
            for (int i = 0; i < numberOfShots; i++)
            {
                await _scheduler.TriggerJob(trigger.JobKey, CreateDataMap(data));
            }
        }

        public async Task PreventMultipleStarts(string jobName, int numberOfShots, Dictionary<string, object> data)
        {
            ITrigger trigger = await GetTrigger(jobName);

            if (trigger.GetNextFireTimeUtc() <= DateTime.UtcNow)
            {
                await _scheduler.TriggerJob(trigger.JobKey, CreateDataMap(data));
            }
        }

        public async Task RunDependentJobs()
        {
            _scheduler.ListenerManager.AddJobListener(new DependentListener());
            ITrigger trigger = await GetTrigger("FirstJob");
            var map = new JobDataMap { { "nextJobName", "OneTimeJob" } };
            await _scheduler.TriggerJob(trigger.JobKey, map);
        }
        
        private async Task<ITrigger> GetTrigger(string jobName)
        {
            return await _scheduler.GetTrigger(new TriggerKey($"{jobName}Trigger", "group1"));
        }

        private JobDataMap CreateDataMap(Dictionary<string, object> data)
        {
            var dataMap = new JobDataMap();
            foreach (var pair in data)
            {
                dataMap.Add(pair.Key, pair.Value);
            }
            return dataMap;
        }
    }

    class RuntimeJob : IJob
    {
        public Task Execute(IJobExecutionContext context)
        {
            Console.WriteLine("RuntimeJob running...");
            
            return Task.CompletedTask;
        }
    }
}