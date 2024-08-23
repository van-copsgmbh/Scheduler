using Corima.Scheduler.Shared;
using Quartz;
using System;
using System.Threading.Tasks;

namespace Corima.Scheduler
{
    public class JobScheduler : IJobScheduler
    {
        private readonly ISchedulerFactory _schedulerFactory;

        public JobScheduler(ISchedulerFactory schedulerFactory)
        {
            this._schedulerFactory = schedulerFactory;
        }

        public async Task QueueJob(JobKey jobKey)
        {
            var scheduler = await this._schedulerFactory.GetScheduler();
            await scheduler.TriggerJob(jobKey);
        }

        public async Task<bool> CheckJobStatus(JobKey jobKey)
        {
            return true;
            //var scheduler = await this._schedulerFactory.GetScheduler();
            //var jobs = await scheduler.GetCurrentlyExecutingJobs();
            ////should not be 1st of defualt 
            //jobs.FirstOrDefault(c=>c.JobDetail.Key == jobKey)
            //    .Trigger.
        }

        //public Task GetWaitTask(JobKey key, TimeSpan checkInterval)
        //{
        //    return Task.Run(() =>
        //    {
        //        while (true)
        //        {
        //            return CheckJobStatus == finished
        //        }
        //    });
        //}
    }
}