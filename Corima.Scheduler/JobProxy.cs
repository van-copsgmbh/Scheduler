using System.Threading.Tasks;
using Corima.Scheduler.Shared.Jobs;
using Corima.Services;
using Quartz;

namespace Corima.Scheduler
{
    //can be used for logging, for only one-time-running jobs
    class JobProxy<T> : IJob where T : ICorimaJob
    {
        private readonly RepositoryService _repositoryService;
        public JobProxy(RepositoryService repositoryService)
        {
            _repositoryService = repositoryService;
        }
        
        public Task Execute(IJobExecutionContext context)
        {
            
            var job = (T)context.JobDetail.JobDataMap.Get("JobInstance");
            // _repositoryService.Save("JOB STARTED");
            var result = job.Execute(context);
            // _repositoryService.Save("JOB ENDED");
            return result;
        }
    }
}