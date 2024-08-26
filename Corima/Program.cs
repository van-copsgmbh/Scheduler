using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Corima.Scheduler;
using Corima.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Quartz;
using Quartz.Impl;
using Quartz.Impl.AdoJobStore;
using Corima.Scheduler.DbProviders;
using Corima.Scheduler.Shared.Jobs;
using Corima.Scheduler.Shared.Schedulers;

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
          
            IHost host = GetBuilder();
            var schedulerInitializer = host.Services.GetService<JobSchedulerInitializer>();
            await schedulerInitializer.Initialize();
            
            var jobScheduler = host.Services.GetService<IJobScheduler>();
            // await jobScheduler.QueueJob()
            
            
            // StdSchedulerFactory.GetDefaultScheduler().Result.Start();

            string command = "";
            string exitKey = "x";
            while ((command = Console.ReadLine().ToLower()) != exitKey)
            {
                var cmd = new CommandSelector(schedulerInitializer.Scheduler);
                await cmd.Run(command);
            }
            Console.ReadLine();
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
                                sql.UseConnectionProvider<SqlServerConnectionProvider>();
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
                    RegisterJobs(services);
                })
                .Build();
        }
        
        private static void RegisterJobs(IServiceCollection serviceCollection)
        {
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();
            foreach (var assembly in assemblies)
            {
                var types = assembly.GetTypes()
                    .Where(t => typeof(ICorimaJob).IsAssignableFrom(t) && !t.IsAbstract);

                foreach (var type in types)
                {
                    serviceCollection.AddSingleton(typeof(ICorimaJob), type);
                }
            }
        }
    }
}