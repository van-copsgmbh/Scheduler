using Quartz;

namespace Corima.Scheduler.Shared.Triggers
{
    public class BuiltinTriggers
    {
        public ITrigger OneTimeTrigger(string name)
        {
            return TriggerBuilder.Create()
                .WithIdentity(name, "group1")
                .StartNow()
                .Build();
        }

        public ITrigger RepeatedTrigger(string name, int seconds)
        {
            return  TriggerBuilder.Create()
                .WithIdentity(name, "group1")
                .StartNow()
                .WithSimpleSchedule(x => x.WithIntervalInSeconds(seconds).RepeatForever())
                .Build();
        }
    }
}