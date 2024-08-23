using Corima.Scheduler.Shared;
using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Corima.Scheduler
{
    public class JobSchedulerInitializer
    {
        private readonly ISchedulerFactory _schedulerFactory;
        private readonly IEnumerable<ICorimaAutoJob> _jobs;

        public JobSchedulerInitializer(ISchedulerFactory schedulerFactory, IEnumerable<ICorimaAutoJob> jobs)
        {
            this._schedulerFactory = schedulerFactory;
            this._jobs = jobs;
        }

        public async Task Initialize()
        {
            var scheduler = await _schedulerFactory.GetScheduler();
            var jobs = this._jobs; //load manual and trigger jobs seperately by different interface as they differ in initialization
            
            Dictionary<IJobDetail, IReadOnlyCollection<ITrigger>> jobsDictionary = new Dictionary<IJobDetail, IReadOnlyCollection<ITrigger>>();
            foreach (var corimaJob in jobs)
            {
                IJobDetail job = JobBuilder.Create(corimaJob.GetType())
                    //.DisallowConcurrentExecution() // get from interface - that way it won't need quartz atribute knowledge 
                    .PersistJobDataAfterExecution()
                    .StoreDurably() // this should be set for one time tasks, but not for periodic one time tasks.
                    //.WithDescription()
                    .WithIdentity(corimaJob.Key)
                    .Build();

                var triggerSet = new HashSet<ITrigger>();
                if (corimaJob.Trigger != null)
                {
                    triggerSet.Add(corimaJob.Trigger);
                }

                jobsDictionary.Add(job, triggerSet);
            }

            await scheduler.ScheduleJobs(jobsDictionary, replace: true);

            await scheduler.Start();
        }

        //private IEnumerable<ICorimaAutoJob> FindJobs()  //it would be much better if we register them in DI container and inject all of them into this class ctor.
        //{
        //    //var jobType = typeof(ICorimaAutoJob);
        //    //var jobTypes = AppDomain.CurrentDomain.GetAssemblies()
        //    //    .SelectMany(x => x.GetTypes())
        //    //    .Where(x => jobType.IsAssignableFrom(x) && !x.IsInterface && !x.IsAbstract);
        //    //return jobTypes.Select(c => (ICorimaAutoJob)Activator.CreateInstance(c));
        //}
    }
}