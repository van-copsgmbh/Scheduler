using System;
using Corima.Scheduler.Shared.Triggers;
using Quartz;

namespace Corima.Scheduler.Shared.Jobs
{
    public abstract class RepeatedJob : CorimaJob
    {
        public override ITrigger Trigger { get; }

        protected RepeatedJob(TimeSpan fromSeconds)
        {
            Trigger = new BuiltinTriggers().RepeatedTrigger(TriggerName, (int)fromSeconds.TotalSeconds);
        }
    }
}