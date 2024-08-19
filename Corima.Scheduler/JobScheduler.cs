using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Corima.Scheduler.Shared;
using Corima.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Quartz;
using Quartz.Impl;
using Quartz.Impl.AdoJobStore;
using Quartz.Impl.AdoJobStore.Common;
using Quartzmin;
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
                JobKey jobKey = new JobKey(job.Name, "group1");
                
               
                CorimaJob jobInstance = (CorimaJob)Activator.CreateInstance(job);
                IJobDetail jobDetail = new JobDetailImpl(job.Name, "group1", job);
                // IJobDetail jobDetail = JobBuilder.Create<JobProxy<CorimaJob>>()
                //     .WithIdentity(job.Name, "group1")
                //     .Build();
                if (await Scheduler.CheckExists(jobKey))
                {
                    await Scheduler.RescheduleJob(new TriggerKey(jobInstance.Trigger.Key.Name, "group1"), jobInstance.Trigger);
                }
                else
                {
                    await Scheduler.ScheduleJob(jobDetail, jobInstance.Trigger);
                }
                
            }
            
            Builder.Run();
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
                                sql.ConnectionString =
                                    "Data Source=CZNBHOME7;Database=quartz;Integrated Security=false;User ID=quartz_user;Password=123456;";
                                sql.ConnectionStringName = "Quartz";
                                sql.UseDriverDelegate<SqlServerDelegate>();
                                sql.UseConnectionProvider<CustomSqlServerConnectionProvider>();   
                            }, "MSSQLSQLSERVER");
                            x.UseProperties = false;
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
    
    //can be used for logging
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

    //can be used for logging, for only one-time-running jobs
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
    
    public class CustomSqlServerConnectionProvider : IDbProvider
    {
        public CustomSqlServerConnectionProvider()
        {
            Metadata = new DbMetadata
            {
                AssemblyName = typeof(SqlConnection).AssemblyQualifiedName,
                BindByName = true,
                CommandType = typeof(SqlCommand),
                ConnectionType = typeof(SqlConnection),
                DbBinaryTypeName = "VarBinary",
                ExceptionType = typeof(SqlException),
                ParameterDbType = typeof(SqlDbType),
                ParameterDbTypePropertyName = "SqlDbType",
                ParameterNamePrefix = "@",
                ParameterType = typeof(SqlParameter),
                UseParameterNamePrefixInParameterCollection = true
            };
            Metadata.Init();
        }

        public void Initialize()
        {
        }

        public DbCommand CreateCommand()
        {
            return new SqlCommand();
        }

        public DbConnection CreateConnection()
        {
            return new SqlConnection(ConnectionString);
        }

        public string ConnectionString
        {
            get =>
                "Data Source=CZNBHOME7;Database=quartz;Integrated Security=false;User ID=quartz_user;Password=123456;";
            set => throw new NotImplementedException();
        }

        public DbMetadata Metadata { get; }

        public void Shutdown()
        {
        }
    }
}