using System;
using System.Threading.Tasks;
using Corima.Scheduler.Shared;
using Quartz;

namespace Corima.Scheduler
{
    [PersistJobDataAfterExecution]
    class FirstJob : CorimaJob
    {
        public ITrigger Trigger { get; } = new Triggers().RepeatedTrigger("FirstJObTrigger", 1);
        
        private static int count = 0;
    
        // public int Value1 { private get; set; } = 0;
    
        public Task Execute(IJobExecutionContext context)
        {
            count++;
            Console.WriteLine($"First Job running [{count}]: ");
    
            return Task.CompletedTask;
        }
    }
    
    [PersistJobDataAfterExecution]
    class OneTimeJob : CorimaJob
    {
        
        public ITrigger Trigger { get; } = new Triggers().OneTimeTrigger("OneTimeJob");
        private static int count = 0;
    
        // public int Value1 { private get; set; } = 0;
        
        public Task Execute(IJobExecutionContext context)
        {
            count++;
            Console.WriteLine($"OneTime Job running [{count}]: ");
    
            return Task.CompletedTask;
        }
    }
    
    
    // [DisallowConcurrentExecution]
    // class LongJob : CorimaJob
    // {
    //     public ITrigger Trigger { get; } = new Triggers().RepeatedTrigger("LongJob", 5);
    //     
    //     private static int count = 0;
    //
    //     // public int Value1 { private get; set; } = 0;
    //
    //     public async Task Execute(IJobExecutionContext context)
    //     {
    //         count++;
    //         Console.WriteLine($"Long Job running [{count}]: ");
    //         await Task.Delay(20000);
    //     }
    // }
    //
    // [PersistJobDataAfterExecution]
    // class BJob : CorimaJob
    // {
    //     public ITrigger Trigger { get; } = new Triggers().RepeatedTrigger("B", 5);
    //     
    //     private static int count = 0;
    //
    //     // public int Value1 { private get; set; } = 0;
    //
    //     public Task Execute(IJobExecutionContext context)
    //     {
    //         count++;
    //         Console.WriteLine($"B Job running [{count}]: ");
    //
    //         return Task.CompletedTask;
    //     }
    // }
    //
    // [PersistJobDataAfterExecution]
    // class CJob : CorimaJob
    // {
    //     public ITrigger Trigger { get; } = new Triggers().RepeatedTrigger("C", 5);
    //     
    //     private static int count = 0;
    //
    //     // public int Value1 { private get; set; } = 0;
    //
    //     public Task Execute(IJobExecutionContext context)
    //     {
    //         count++;
    //         Console.WriteLine($"C Job running [{count}]: ");
    //
    //         return Task.CompletedTask;
    //     }
    // }
    //
    // [PersistJobDataAfterExecution]
    // class DJob : CorimaJob
    // {
    //     public ITrigger Trigger { get; } = new Triggers().RepeatedTrigger("D", 5);
    //     
    //     private static int count = 0;
    //
    //     // public int Value1 { private get; set; } = 0;
    //
    //     public Task Execute(IJobExecutionContext context)
    //     {
    //         count++;
    //         Console.WriteLine($"D Job running [{count}]: ");
    //
    //         return Task.CompletedTask;
    //     }
    // }
    //
    // [PersistJobDataAfterExecution]
    // class EJob : CorimaJob
    // {
    //     public ITrigger Trigger { get; } = new Triggers().RepeatedTrigger("E", 5);
    //     
    //     private static int count = 0;
    //
    //     // public int Value1 { private get; set; } = 0;
    //
    //     public Task Execute(IJobExecutionContext context)
    //     {
    //         count++;
    //         Console.WriteLine($"E Job running [{count}]: ");
    //
    //         return Task.CompletedTask;
    //     }
    // }
    //
    // [PersistJobDataAfterExecution]
    // class FJob : CorimaJob
    // {
    //     public ITrigger Trigger { get; } = new Triggers().RepeatedTrigger("F", 5);
    //     
    //     private static int count = 0;
    //
    //     // public int Value1 { private get; set; } = 0;
    //
    //     public Task Execute(IJobExecutionContext context)
    //     {
    //         count++;
    //         Console.WriteLine($"F Job running [{count}]: ");
    //
    //         return Task.CompletedTask;
    //     }
    // }
    //
    // [PersistJobDataAfterExecution]
    // class GJob : CorimaJob
    // {
    //     public ITrigger Trigger { get; } = new Triggers().RepeatedTrigger("G", 5);
    //     
    //     private static int count = 0;
    //
    //     // public int Value1 { private get; set; } = 0;
    //
    //     public Task Execute(IJobExecutionContext context)
    //     {
    //         count++;
    //         Console.WriteLine($"G Job running [{count}]: ");
    //
    //         return Task.CompletedTask;
    //     }
    // }
    //
    // [PersistJobDataAfterExecution]
    // class HJob : CorimaJob
    // {
    //     public ITrigger Trigger { get; } = new Triggers().RepeatedTrigger("H", 5);
    //     
    //     private static int count = 0;
    //
    //     // public int Value1 { private get; set; } = 0;
    //
    //     public Task Execute(IJobExecutionContext context)
    //     {
    //         count++;
    //         Console.WriteLine($"H Job running [{count}]: ");
    //
    //         return Task.CompletedTask;
    //     }
    // }
    //
    // [PersistJobDataAfterExecution]
    // class JJob : CorimaJob
    // {
    //     public ITrigger Trigger { get; } = new Triggers().RepeatedTrigger("I", 5);
    //     
    //     private static int count = 0;
    //
    //     // public int Value1 { private get; set; } = 0;
    //
    //     public Task Execute(IJobExecutionContext context)
    //     {
    //         count++;
    //         Console.WriteLine($"J Job running [{count}]: ");
    //
    //         return Task.CompletedTask;
    //     }
    // }
    //
    // [PersistJobDataAfterExecution]
    // class KJob : CorimaJob
    // {
    //     public ITrigger Trigger { get; } = new Triggers().RepeatedTrigger("J", 5);
    //     
    //     private static int count = 0;
    //
    //     // public int Value1 { private get; set; } = 0;
    //
    //     public Task Execute(IJobExecutionContext context)
    //     {
    //         count++;
    //         Console.WriteLine($"K Job running [{count}]: ");
    //
    //         return Task.CompletedTask;
    //     }
    // }
}