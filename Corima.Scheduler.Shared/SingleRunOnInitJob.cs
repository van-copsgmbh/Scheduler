using Quartz;
using System;
using System.Threading.Tasks;

namespace Corima.Scheduler.Shared
{
    public class SingleRunOnInitJob : CorimaJob, ICorimaAutoJob
    {
        public SingleRunOnInitJob()
        {
            this.Trigger = new Triggers().OneTimeTrigger(this.JobName+"Trigger");
            this.Key = new JobKey(this.JobName, "Group1");
        }

        protected override string JobName => "AutoJob1";

        public JobKey Key { get; }
        public ITrigger Trigger { get; }

        protected override Task OnError(IJobExecutionContext context, Exception exception)
        {
            return Task.CompletedTask;
        }

        protected override async Task DoWork(IJobExecutionContext context)
        {
            Console.WriteLine("One time start up job running");
            await Task.Delay(TimeSpan.FromSeconds(5));
            Console.WriteLine("One time start up job finished");
        }
    }
}