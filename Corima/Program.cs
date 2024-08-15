using System;
using System.Linq;
using System.Threading.Tasks;
using Corima.Scheduler;
using Corima.Services;
using Microsoft.Extensions.DependencyInjection;
using Quartz;
using TreasuryBrowser;

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

            var services = CreateServices();
            services.GetRequiredService<MyService>().Save();
            new tb();
            var scheduler = new JobScheduler();
            scheduler.Init();
            
            
            string command = Console.ReadLine();
            if ( command == "A")
            {
                var x = (await scheduler.Scheduler.GetCurrentlyExecutingJobs()).ToList();
                ITrigger trigger = await scheduler.Scheduler.GetTrigger(new TriggerKey("OneTimeJob", "group1"));
                
                // await scheduler.Scheduler.RescheduleJob(new TriggerKey("OneTimeJob", "group1"), 
                //     new Scheduler.Shared.Triggers().OneTimeTrigger("OneTimeJob2"));
                await scheduler.Scheduler.TriggerJob(new JobKey("A", "group1"));
                Console.WriteLine("OK");
            }

            if (command == "S")
            {
                var jobs = await scheduler.Scheduler.GetCurrentlyExecutingJobs();
                foreach (var j in jobs)
                {
                    Console.WriteLine($"STOPPING {j.JobDetail.Key.Name}");
                    scheduler.Scheduler.Interrupt((j.JobDetail.Key));
                    scheduler.Scheduler.UnscheduleJob((j.Trigger.Key));
                    scheduler.Scheduler.DeleteJob(j.JobDetail.Key);
                }

                scheduler.Scheduler.Clear();
            }
                
            Console.ReadLine();
        }
        
        private static ServiceProvider CreateServices()
        {
            var serviceProvider = new ServiceCollection()
                .AddSingleton<RepositoryService>()
                .AddSingleton<MyService>()
                .BuildServiceProvider();

            return serviceProvider;
        }
    }
}