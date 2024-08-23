using Quartz;
using System;
using System.Reflection;
using System.Threading.Tasks;

namespace Corima.Scheduler.Shared
{
    public abstract class RepeatedJob : CorimaJob, ICorimaAutoJob
    {
        public JobKey Key { get; }
        public ITrigger Trigger { get; }

        protected RepeatedJob(TimeSpan fromSeconds)
        {
            this.Trigger = new Triggers().RepeatedTrigger(this.JobName + "Trigger", (int)fromSeconds.TotalSeconds);
            this.Key = new JobKey(this.JobName, "Group1");
        }
    }


    public class PeriodicJob2 : RepeatedJob
    {
        private static int counter = 1;

        public PeriodicJob2() : base(TimeSpan.FromHours(2))
        {
        }

        protected override string JobName { get; } = "PeriodicJob2";

        protected override Task OnError(IJobExecutionContext context, Exception exception)
        {
            return Task.CompletedTask;
        }

        protected override async Task DoWork(IJobExecutionContext context)
        {
            Console.WriteLine($"PeriodicJob executed {counter++}");
        }
    }

    public class PeriodicJob : RepeatedJob, ICorimaAutoJob
    {
        protected override string JobName => "PeriodicJob";

        public PeriodicJob() : base(TimeSpan.FromSeconds(50))
        {
            
        }
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