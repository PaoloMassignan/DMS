using DMS.Kernel.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TechTalk.SpecFlow;

namespace DMS.Test
{
    public class FeatureLogger:ILogger
    {
        public FeatureLogger(FeatureContext context)
        {
            this.Context = context;
        }

        public FeatureContext Context { get; }

        public void Log(string msg)
        {
            Console.WriteLine(msg);
            Thread.Sleep(10);
        }
    }
}
