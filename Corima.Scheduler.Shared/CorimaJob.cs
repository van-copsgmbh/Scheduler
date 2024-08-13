using Quartz;

namespace Corima.Scheduler.Shared
{
    public interface CorimaJob : IJob
    {
        
        ITrigger Trigger { get; }
    }
}