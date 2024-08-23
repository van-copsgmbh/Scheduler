using Corima.Scheduler.Shared;
using Corima.Services;
using Quartz;
using System.Threading.Tasks;

namespace Corima.Scheduler
{
    class JobProxy<T> : IJob where T : ICorimaAutoJob
    {
        private readonly RepositoryService _repositoryService;
        public JobProxy(RepositoryService repositoryService)
        {
            this._repositoryService = repositoryService;
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