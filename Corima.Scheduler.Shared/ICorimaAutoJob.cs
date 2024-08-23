using Quartz;

namespace Corima.Scheduler.Shared
{
    public interface ICorimaAutoJob : IJob
    {
        JobKey Key { get; }
        ITrigger Trigger { get; }
    }
}