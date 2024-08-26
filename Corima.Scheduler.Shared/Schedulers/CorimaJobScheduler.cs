using System.Threading.Tasks;
using Quartz;

namespace Corima.Scheduler.Shared.Schedulers
{
    public class CorimaJobScheduler : IJobScheduler
    {
        private readonly ISchedulerFactory _schedulerFactory;

        public CorimaJobScheduler(ISchedulerFactory schedulerFactory)
        {
            _schedulerFactory = schedulerFactory;
        }

        public async Task QueueJob(JobKey jobKey)
        {
            var scheduler = await _schedulerFactory.GetScheduler();
            await scheduler.TriggerJob(jobKey);
        }

        public async Task<bool> CheckJobStatus(JobKey jobKey)
        {
            return await Task.FromResult(true);
        }
    }
}