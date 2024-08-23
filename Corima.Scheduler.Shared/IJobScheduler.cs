using Quartz;
using System.Threading.Tasks;

namespace Corima.Scheduler.Shared
{
    public interface IJobScheduler
    {
        Task QueueJob(JobKey jobKey);
        Task<bool> CheckJobStatus(JobKey jobKey);

        //Task GetWaitTask();
    }
}
