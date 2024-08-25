using Quartz;

namespace Corima.Scheduler.Shared.Jobs
{
    public abstract class ManualJob : CorimaJob
    {
        public sealed override ITrigger Trigger => null;
    }
}