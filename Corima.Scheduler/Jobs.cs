using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Corima.Scheduler.Shared;
using Quartz;

namespace Corima.Scheduler
{
    //[PersistJobDataAfterExecution]
    //class FirstJob : ICorimaAutoJob
    //{
    //    public ITrigger Trigger { get; } = new Triggers().RepeatedTrigger("FirstJobTrigger", 10);
        
    //    private static int count = 0;
    //    public Task Execute(IJobExecutionContext context)
    //    {
    //        count++;
    //        var data = context.MergedJobDataMap;
    //        Console.WriteLine($"Recurring Job running [{count}]: ");
    //        return Task.CompletedTask;
    //    }
    //}
    
    //[PersistJobDataAfterExecution]
    //class OneTimeJob : ICorimaAutoJob
    //{
    //    public ITrigger Trigger { get; } = new Triggers().OneTimeTrigger("OneTimeJobTrigger");
    //    private static int count = 0;
    //    public Task Execute(IJobExecutionContext context)
    //    {
    //        count++;
    //        Console.WriteLine($"OneTime Job running [{count}]: ");
    
    //        return Task.CompletedTask;
    //    }
    //}
    
    //class LongJob : ICorimaAutoJob
    //{
    //    public ITrigger Trigger { get; } = new Triggers().RepeatedTrigger("LongJob", 10);

    //    private static int count = 0;
    //    public async Task Execute(IJobExecutionContext context)
    //    {
    //        count++;
    //        Console.WriteLine($"Long Job running [{count}]: ");
    //        await Task.Delay(20000);
    //    }
    //}
}