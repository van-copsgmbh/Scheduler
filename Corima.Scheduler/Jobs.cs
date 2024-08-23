using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Corima.Scheduler.Shared;
using Corima.Services;
using Quartz;

namespace Corima.Scheduler
{
    [PersistJobDataAfterExecution]
    class FirstJob : CorimaJob
    {
        private readonly RepositoryService _repositoryService;
        public FirstJob(RepositoryService repositoryService)
        {
            _repositoryService = repositoryService;
        }
        public ITrigger Trigger { get; } = new Triggers().RepeatedTrigger("FirstJobTrigger", 10);
        
        private static int count = 0;
        public Task Execute(IJobExecutionContext context)
        {
            count++;
            var data = context.MergedJobDataMap;
            Console.WriteLine($"First Job running [{count}]: ");
            return Task.CompletedTask;
        }
    }
    
    [PersistJobDataAfterExecution]
    class OneTimeJob : CorimaJob
    {
        public ITrigger Trigger { get; } = new Triggers().OneTimeTrigger("OneTimeJobTrigger");
        private static int count = 0;
        public Task Execute(IJobExecutionContext context)
        {
            count++;
            Console.WriteLine($"OneTime Job running [{count}]: ");
    
            return Task.CompletedTask;
        }
    }
    
    [DisallowConcurrentExecution]
    class LongJob : CorimaJob
    {
        public ITrigger Trigger { get; } = new Triggers().RepeatedTrigger("LongJob", 10);

        
        private static int count = 0;
        public async Task Execute(IJobExecutionContext context)
        {
            count++;
            Console.WriteLine($"Long Job running [{count}]: ");
            await Task.Delay(20000);
        }
    }
}