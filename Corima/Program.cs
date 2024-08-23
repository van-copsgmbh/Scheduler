using System;
using System.Threading.Tasks;
using Corima.Scheduler;
using Corima.Scheduler.Shared;
using Corima.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Quartz;
using Quartz.Impl.AdoJobStore;

namespace Corima
{
    internal class Program
    {
        public static async Task Main(string[] args)
        {
            // dynamic job registration
            // logging to database
            // start job only at service startup, only one run and then never again,  
            // manual restart
            // prioritization of jobs - if it takes a long time, it's stuck the queue



            // how to delete job (from code and database)?
            //-------------------------------------------------
            // must be checked if the job is already registered (loaded from database). How to change job/delete job?
            // 1. whenever the server is started, we can delete all job and register again
            // 2. for update job we can use RescheduleJob (https://stackoverflow.com/a/76948032/6157936)
            // exception is throwed when job exists in database, but not in code


            //COMMANDS
            //------------------------
            // run JOBNAME [-d key=value, key2=value] - doesnt work for one-time jobs
            // run JOBNAME, COUNT [-d key=value, key2=value] - run specified job COUNT times
            // run-safe JOBNAME, COUNT [-d key=value, key2=value] - prevent to multiple runs earlier than the scheduled start time
            // run-dependent - run FirstJob and than OneTimeJob


            var host = GetBuilder();
            var schedulerInitializer = host.Services.GetService<JobSchedulerInitializer>();
            await schedulerInitializer.Initialize();

            var jobScheduler = host.Services.GetService<IJobScheduler>();


            //await jobScheduler.QueueJob(ManualJob.Key);
            //await Task.Delay(TimeSpan.FromSeconds(10));
            //await jobScheduler.QueueJob(ManualJob.Key);
            //await Task.Delay(TimeSpan.FromSeconds(10));
            //await jobScheduler.QueueJob(ManualJob.Key);

            Console.WriteLine();
        }

        private static IHost GetBuilder()
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
                    services.AddSingleton<JobSchedulerInitializer>();
                    services.AddSingleton<IJobScheduler, JobScheduler>();
                    services.AddSingleton<ICorimaAutoJob, ManualJob>();
                    services.AddSingleton<ICorimaAutoJob, ChainedJob1>();
                    services.AddSingleton<ICorimaAutoJob, ChildJob>();

                })
                .Build();
        }
    }
}