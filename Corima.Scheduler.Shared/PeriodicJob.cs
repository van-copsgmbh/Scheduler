using Quartz;
using System;
using System.Threading.Tasks;

namespace Corima.Scheduler.Shared
{
    public class PeriodicJob : CorimaJob, ICorimaAutoJob
    {
        public JobKey Key { get; } = new JobKey("PeriodicJob", "Group1");
        public ITrigger Trigger { get; } = new Triggers().RepeatedTrigger("PeriodicJob" + "Trigger", 60);

        protected override string JobName => "PeriodicJob";
        private static int counter = 1;

        protected override Task OnError(IJobExecutionContext context, Exception exception)
        {
            return Task.CompletedTask;
        }

        protected override async Task DoWork(IJobExecutionContext context)
        {
            Console.WriteLine($"PeriodicJob executed {counter++}");
            
        }
    }
}