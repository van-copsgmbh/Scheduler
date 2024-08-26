using Corima.Scheduler.Shared.Triggers;
using Quartz;

namespace Corima.Scheduler.Shared.Jobs
{
    public abstract class SingleRunOnInitJob : CorimaJob
    {
        public sealed override ITrigger Trigger { get; }

        protected SingleRunOnInitJob()
        {
            Trigger = new BuiltinTriggers().OneTimeTrigger(TriggerName);
            StoreDurably = true;
        }
    }
}