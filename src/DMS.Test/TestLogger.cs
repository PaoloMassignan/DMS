using DMS.Kernel.Interfaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DMS.Test
{
    public class TestLogger : ILogger
    {
        private TestContext testContext;

        public TestLogger(TestContext testContext)
        {
            this.testContext = testContext;
        }

        public void Log(string msg)
        {
            testContext.WriteLine(msg);
        }
    }
}
