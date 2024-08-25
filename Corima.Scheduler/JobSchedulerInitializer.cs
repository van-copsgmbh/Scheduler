using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Corima.Scheduler.Shared.Jobs;
using Quartz;

namespace Corima.Scheduler
{
    public sealed class JobSchedulerInitializer
    {
        private readonly ISchedulerFactory _schedulerFactory;
        private readonly IEnumerable<ICorimaJob> _jobs;
        
        public IScheduler Scheduler { get; private set; }

        public JobSchedulerInitializer(ISchedulerFactory schedulerFactory, IEnumerable<ICorimaJob> jobs)
        {
            _schedulerFactory = schedulerFactory;
            _jobs = jobs;
        }

        public async Task Initialize()
        {
            var Scheduler = await _schedulerFactory.GetScheduler();
            Dictionary<IJobDetail, IReadOnlyCollection<ITrigger>> jobs = _jobs.Where(x => x.Trigger != null)
                .ToDictionary(
                    CreateJobDetail,
                    x => new HashSet<ITrigger> { x.Trigger } as IReadOnlyCollection<ITrigger>);
            
            await Scheduler.ScheduleJobs(jobs, replace: true);
            await Scheduler.Start();
        }

        private IJobDetail CreateJobDetail(ICorimaJob job)
        {
            return JobBuilder.Create(job.GetType())
                    .WithIdentity(job.Key)
                    .DisallowConcurrentExecution(job.DisallowConcurrentExecution)
                    .PersistJobDataAfterExecution(job.PersistJobDataAfterExecution)
                    .StoreDurably(job.StoreDurably)
                    .Build();
        }
    }
}