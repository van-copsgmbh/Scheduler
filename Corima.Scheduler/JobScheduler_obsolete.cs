using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Corima.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Quartz;
using Quartz.Impl;
using Quartz.Impl.AdoJobStore;
using Corima.Scheduler.DbProviders;
using Corima.Scheduler.Shared.Jobs;

namespace Corima.Scheduler
{
    [Obsolete]
    public class JobSchedulerObsolete
    {
        private readonly ServiceProvider _serviceProvider;

        public JobSchedulerObsolete(ServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public IScheduler Scheduler { get; set; }
        public IHost Builder { get; set; }
        public async Task Init()
        {
            Builder = GetBuilder();
            Scheduler = await GetScheduler(Builder);
            
            IEnumerable<Type> jobs = FindJobs();

            foreach (var job in jobs)
            {
                JobKey jobKey = new JobKey(job.Name, "group1");
                // CorimaJob jobInstance = (CorimaJob)Activator.CreateInstance(job);
                ICorimaJob jobInstance = _serviceProvider.GetRequiredService(job) as ICorimaJob;
                IJobDetail jobDetail = new JobDetailImpl(job.Name, "group1", job);
                if (await Scheduler.CheckExists(jobKey))
                {
                    await Scheduler.RescheduleJob(new TriggerKey(jobInstance.Trigger.Key.Name, "group1"), jobInstance.Trigger);
                }
                else
                {
                    await Scheduler.ScheduleJob(jobDetail, jobInstance.Trigger);
                }
            }
            
            Scheduler.Start();
        }

        private IHost GetBuilder()
        {
            return Host.CreateDefaultBuilder()
                .ConfigureServices((cxt, services) =>
                {
                    services.AddQuartz(q =>
                    {
                        q.UseMicrosoftDependencyInjectionJobFactory();
                        q.UsePersistentStore(x =>
                        {
                            x.UseSqlServer(sql =>
                            {
                                sql.ConnectionStringName = "Quartz";
                                sql.UseDriverDelegate<SqlServerDelegate>();
                                sql.UseConnectionProvider<SqlServerConnectionProvider>();
                            }, "MSSQLSQLSERVER");
                            x.UseProperties = true;
                            x.UseSystemTextJsonSerializer();
                        });
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
            return scheduler;
        }
        
        private IEnumerable<Type> FindJobs()
        {
            var jobType = typeof(ICorimaJob);
            return AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(x => x.GetTypes())
                .Where(x => jobType.IsAssignableFrom(x) && !x.IsInterface && !x.IsAbstract);
        }
    }
    
    //can be used for logging, for only one-time-running jobs
    // class JobProxy<T> : IJob where T : ICorimaJob
    // {
    //     private readonly RepositoryService _repositoryService;
    //     public JobProxy(RepositoryService repositoryService)
    //     {
    //         _repositoryService = repositoryService;
    //     }
    //     
    //     public Task Execute(IJobExecutionContext context)
    //     {
    //         
    //         var job = (T)context.JobDetail.JobDataMap.Get("JobInstance");
    //         // _repositoryService.Save("JOB STARTED");
    //         var result = job.Execute(context);
    //         // _repositoryService.Save("JOB ENDED");
    //         return result;
    //     }
    // }
}