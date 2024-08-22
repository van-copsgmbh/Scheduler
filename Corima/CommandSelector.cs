using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Quartz;

namespace Corima
{
    class CommandSelector
    {
        private JobScenarios _scenarios;

        public CommandSelector(IScheduler scheduler)
        {
            _scenarios = new JobScenarios(scheduler);
        }

        public async Task Run(string commandString)
        {
            Dictionary<string, object> parameters = new Dictionary<string, object>();
            var splitCommand = Regex.Split(commandString, @"-d\s(.+)$");
            
            if (splitCommand.Length > 1)
            {
                string paramPart = splitCommand[1];
                string[] paramPairs = paramPart.Split(new[] { ',' }).Select(x => x.Trim()).ToArray();

                foreach (var pair in paramPairs)
                {
                    string[] keyValue = pair.Split(new[] { '=' }, 2);
                    if (keyValue.Length == 2)
                    {
                        parameters[keyValue[0].Trim()] = keyValue[1].Trim();
                    }
                }
            }
            
            string[] parts = splitCommand[0].Split(new[] { ' ' }, 2);
            string[] args = new string[]{};
            
            if (parts.Length > 1)
            {
                args = parts[1].Split(new[] { ',' })?.Select(x => x.Trim()).ToArray();
            }
            

            if (Regex.IsMatch(splitCommand[0], @"^run\s\w+(\s)*$"))
            {
                await _scenarios.RunJobManually(args[0], parameters);
                return;
            }
           
            if (Regex.IsMatch(commandString, @"run\s\w+,\s\d+"))
            {
                await _scenarios.RunJobManyTimes(args[0], int.Parse(args[1]), parameters);
                return;
            }

            if (Regex.IsMatch(commandString, @"run-safe\s\w+,\s\d+"))
            {
                await _scenarios.PreventMultipleStarts(args[0], int.Parse(args[1]), parameters);
                return;
            }

            if (commandString == "new x")
            {
                await _scenarios.RunNewJob();
                return;
            }

            if (commandString == "run-dependent")
            {
                await _scenarios.RunDependentJobs();
                return;
            }
            
            Console.WriteLine("Unknown command");
        }
    }
}