using System.Threading.Tasks;
using Quartz;

namespace Corima.Scheduler.Shared.Schedulers
{
    public interface IJobScheduler
    {
        Task QueueJob(JobKey jobKey);
        Task<bool> CheckJobStatus(JobKey jobKey);
        
        //Task GetWaitTask();
    }
}