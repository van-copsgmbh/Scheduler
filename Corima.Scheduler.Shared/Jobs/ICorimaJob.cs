using Quartz;

namespace Corima.Scheduler.Shared.Jobs
{
    public interface ICorimaJob : IJob
    {
        string JobName { get; }
        string JobGroup { get; }
        string TriggerName { get; }
        JobKey Key { get; }
        ITrigger Trigger { get; }
        bool DisallowConcurrentExecution { get; set; }
        bool PersistJobDataAfterExecution { get; set; }
        bool StoreDurably { get; set; }
    }
}