using System;
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
            string[] parts = commandString.Split(new[] { ' ' }, 2);
            string command = parts[0];
            string[] args = parts[1].Split(new[] { ',' }).Select(x => x.Trim()).ToArray();

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

            if (commandString == "new x")
            {
                await _scenarios.RunNewJob();
                return;
            }

            Console.WriteLine("Unknown command");
        }
    }
}