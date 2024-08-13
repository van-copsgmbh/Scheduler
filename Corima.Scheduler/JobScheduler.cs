using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Corima.Scheduler.Shared;
using Corima.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Quartz;
using Quartz.Impl;
using TreasuryBrowser;

namespace Corima.Scheduler
{
    public class JobScheduler
    {
        public IScheduler Scheduler { get; set; }
        public IHost Builder { get; set; }
        public async Task Init()
        {
            Builder = GetBuilder();
            Scheduler = await GetScheduler(Builder);
            
            // IEnumerable<Type> jobs = FindJobs();
            //
            // foreach (var job in jobs)
            // {
            //     IJobDetail jobDetail = new JobDetailImpl(job.ToString(), "group1", job);
            //     
            //     var t = ((CorimaJob)Activator.CreateInstance(job)).Trigger;
            //     await Scheduler.ScheduleJob(jobDetail, t);
            // }
            
            IEnumerable<Type> jobs = FindJobs();

            foreach (var job in jobs)
            {
                CorimaJob jobInstance = (CorimaJob)Activator.CreateInstance(job);
                
                IJobDetail jobDetail = JobBuilder.Create<JobProxy<CorimaJob>>()
                    .WithIdentity(job.Name, "group1")
                    .Build();
                jobDetail.JobDataMap.Put("JobInstance", jobInstance);
                await Scheduler.ScheduleJob(jobDetail, jobInstance.Trigger);
            }
            
            await Builder.RunAsync();
        }

        private IHost GetBuilder()
        {
            return Host.CreateDefaultBuilder()
                .ConfigureServices((cxt, services) =>
                {
                    services.AddQuartz(q =>
                    {
                        q.UseMicrosoftDependencyInjectionJobFactory();
                    });
                    services.AddQuartzHostedService(opt =>
                    {
                        opt.WaitForJobsToComplete = true;
                    });
                    services.AddTransient<RepositoryService>();
                })
                .Build();
        }

        private async Task<IScheduler> GetScheduler(IHost builder)
        {
            var schedulerFactory = builder.Services.GetRequiredService<ISchedulerFactory>(); 
            var scheduler = await schedulerFactory.GetScheduler();
             scheduler.ListenerManager.AddJobListener(new JobListener());
            return scheduler;
        }
        
        private IEnumerable<Type> FindJobs()
        {
            var jobType = typeof(CorimaJob);
            return AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(x => x.GetTypes())
                .Where(x => jobType.IsAssignableFrom(x) && !x.IsInterface && !x.IsAbstract);
        }
    }
    
    class JobListener : IJobListener
    {
        public Task JobToBeExecuted(IJobExecutionContext context, CancellationToken cancellationToken = new CancellationToken())
        {
            Console.WriteLine($"[LOG] Job {context.JobDetail.Key} to be executed");
            return Task.CompletedTask;
        }

        public Task JobExecutionVetoed(IJobExecutionContext context, CancellationToken cancellationToken = new CancellationToken())
        {
            Console.WriteLine($"[LOG] Job {context.JobDetail.Key} vetoed");
            return Task.CompletedTask;
        }

        public Task JobWasExecuted(IJobExecutionContext context, JobExecutionException jobException,
            CancellationToken cancellationToken = new CancellationToken())
        {
            Console.WriteLine($"[LOG] Job {context.JobDetail.Key} executed");
            return Task.CompletedTask;
        }

        public string Name { get; } = "LISTENER";
    }

    class JobProxy<T> : IJob where T : CorimaJob
    {
        private readonly RepositoryService _repositoryService;
        public JobProxy(RepositoryService repositoryService)
        {
            _repositoryService = repositoryService;
        }
        
        public Task Execute(IJobExecutionContext context)
        {
            
            var job = (T)context.JobDetail.JobDataMap.Get("JobInstance");
            // _repositoryService.Save("JOB STARTED");
            var result = job.Execute(context);
            // _repositoryService.Save("JOB ENDED");
            return result;
        }
    }
}