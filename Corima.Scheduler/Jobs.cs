using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Corima.Scheduler.Shared;
using Corima.Scheduler.Shared.Jobs;
using Corima.Scheduler.Shared.Triggers;
using Corima.Services;
using Quartz;

namespace Corima.Scheduler
{
    [PersistJobDataAfterExecution]
    public class FirstJob : CorimaJob
    {
        private readonly RepositoryService _repositoryService;
        public override string JobName { get; } = "FirstJob";
        public override ITrigger Trigger { get; }
        
        private static int count = 0;
        public FirstJob(RepositoryService repositoryService)
        {
            _repositoryService = repositoryService;
            Trigger = new BuiltinTriggers().RepeatedTrigger(TriggerName, 10);
        }
    
        protected override Task DoWork(IJobExecutionContext context)
        {
            count++;
            var data = context.MergedJobDataMap;
            Console.WriteLine($"{JobName} - [{count}]");
            return Task.CompletedTask;
        }

        protected override Task OnError(IJobExecutionContext context, Exception exception)
        {
            throw new NotImplementedException();
        }
    }

     class OneTimeJob : SingleRunOnInitJob
     {
         private static int count = 0;
         public override string JobName { get; } = "OneTimeJob";
         protected override Task DoWork(IJobExecutionContext context)
         {
             count++;
             Console.WriteLine($"{JobName} - [{count}]");
             return Task.CompletedTask;
         }
     
         protected override Task OnError(IJobExecutionContext context, Exception exception)
         {
             throw new NotImplementedException();
         }
     }

     class LongJob : CorimaJob
     {
         public override string JobName { get; } = "LongJob";
         public override ITrigger Trigger { get; }
         
         private static int count = 0;
     
         public LongJob(RepositoryService repositoryService)
         {
             Trigger = new BuiltinTriggers().RepeatedTrigger(TriggerName, 10);
         }
         
         protected override Task DoWork(IJobExecutionContext context)
         {
             count++;
             Console.WriteLine($"{JobName} - [{count}]");
             return Task.CompletedTask;
         }
     
         protected override Task OnError(IJobExecutionContext context, Exception exception)
         {
             throw new NotImplementedException();
         }
     }
     
     public class FirstManualJob: ManualJob
     {
         private static int count = 0;
         public override string JobName { get; } = "FirstManualJob";
         protected override Task DoWork(IJobExecutionContext context)
         {
             count++;
             Console.WriteLine($"{JobName} - [{count}]");
             return Task.CompletedTask;
         }

         protected override Task OnError(IJobExecutionContext context, Exception exception)
         {
             throw new NotImplementedException();
         }
     }
}