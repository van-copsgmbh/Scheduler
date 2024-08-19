using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Corima.Scheduler;
using Corima.Services;
using Microsoft.Extensions.DependencyInjection;
using Quartz;
using Quartz.Impl;
using TreasuryBrowser;

namespace Corima
{
    class Command
    {
        private JobScenarios _scenarios;

        public Command(IScheduler scheduler)
        {
            _scenarios = new JobScenarios(scheduler);
        }
        
        public async Task Run(string commandString)
        {
            string[] parts = commandString.Split(new[] {' '}, 2);
            string command = parts[0];
            string[] args = parts[1].Split(new[]{','}).Select(x => x.Trim()).ToArray();

            if (Regex.IsMatch(commandString, @"^run\s\w+$"))
            {
                await _scenarios.RunJobManually(args[0]);
                return;
            }

            if (Regex.IsMatch(commandString, @"run\s\w+,\s\d+"))
            {
                await _scenarios.RunJobManyTimes(args[0], int.Parse(args[1]));
                return;
            }

            if (Regex.IsMatch(commandString, @"run-safe\s\w+,\s\d+"))
            {
                await _scenarios.PreventMultipleStarts(args[0], int.Parse(args[1]));
                return;
            }

            Console.WriteLine("Unknown command");
        }
    }
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
            // run JOBNAME - doesnt work for one-time jobs
            // run JOBNAME, COUNT - run specified job COUNT times
            // run-safe JOBNAME, COUNT - prevent to multiple runs earlier than the scheduled start time
            
            
            var services = CreateServices();
            services.GetRequiredService<MyService>().Save();
            new tb();
            var scheduler = new JobScheduler();
            scheduler.Init();
            StdSchedulerFactory.GetDefaultScheduler().Result.Start();

            string command = "";
            string exitKey = "x";
            while ((command = Console.ReadLine().ToLower()) != exitKey)
            {
                var cmd = new Command(scheduler.Scheduler);
                await cmd.Run(command);
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