using System;
using System.Threading.Tasks;
using Corima.Scheduler.Shared;
using Quartz;

namespace TreasuryBrowser
{
    
    //public class TreasuryBrowserJob : ICorimaAutoJob
    //{
    //    public ITrigger Trigger { get; } = new Triggers().RepeatedTrigger("TreasuryBrowserJobTrigger", 20);
        
    //    private static int i = 0;
        
    //    public async Task Execute(IJobExecutionContext context)
    //    {
    //        Console.WriteLine("Treasury browser job running " + i);
    //        await Task.Delay(10000);
    //        Console.WriteLine("Treasury browser job finished" + i);
    //        i++;
    //    }
    //}
}