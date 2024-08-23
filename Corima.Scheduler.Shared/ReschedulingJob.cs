using Quartz;
using System;
using System.Threading.Tasks;

namespace Corima.Scheduler.Shared
{
    [PersistJobDataAfterExecution]
    public class ReschedulingJob : CorimaJob, ICorimaAutoJob
    {
        private static int counter = 1;

        protected override string JobName => "ReschedulingJob";
        public JobKey Key { get; } = new JobKey("ReschedulingJob", "Group1");
        public ITrigger Trigger { get; } = new Triggers().OneTimeTrigger("ReschedulingJob" + "Trigger");

        protected override Task OnError(IJobExecutionContext context, Exception exception)
        {
            return Task.CompletedTask;
        }

        protected override async Task DoWork(IJobExecutionContext context)
        {
            await Task.Delay(TimeSpan.FromSeconds(5));
            Console.WriteLine($"{this.JobName} executed {counter++}");

            Console.WriteLine($"{this.JobName} didn't finish in time, rescheduling");


            if (counter == 3)
            {
                await context.Scheduler.UnscheduleJob(context.Trigger.Key);
                Console.WriteLine($"{this.JobName} Was unscheduled");
            }
            else
            {
                var oldTrigger = context.Trigger;
                var newTrigger = TriggerBuilder.Create()
                    .ForJob(this.Key)
                    .WithIdentity($"{oldTrigger.Key.Name}-retry", oldTrigger.Key.Group)
                    .StartAt(DateTimeOffset.UtcNow.AddSeconds(30))
                    .Build();

                await context.Scheduler.ScheduleJob(newTrigger);
            }
        }
    }
}