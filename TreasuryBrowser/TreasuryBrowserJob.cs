using System;
using System.Threading.Tasks;
using Corima.Scheduler.Shared.Jobs;
using Quartz;

namespace TreasuryBrowser
{
    
    public class TreasuryBrowserJob : RepeatedJob
    {
        private int count = 0;
        public override string JobName { get; } = "TreasuryBrowserJob";
        public TreasuryBrowserJob() : base(TimeSpan.FromSeconds(5))
        {
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
}