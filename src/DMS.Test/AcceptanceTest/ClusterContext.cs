using DMS.Kernel;
using DMS.Kernel.Interfaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TechTalk.SpecFlow;
using Unity;

namespace DMS.Test.AcceptanceTest
{
    public class ClusterContext
    {
        public UnityContainer Container { get; }
        public Dictionary<string,ManualResetEvent> CloseEvents { get; internal set; }

        public List<DMSNode> Nodes { get; internal set; }

        public ClusterContext(FeatureContext context)
        {
            Container = new UnityContainer();
            Nodes = new List<DMSNode>();
            CloseEvents = new Dictionary<string, ManualResetEvent>();
            // Logger
            Container.RegisterInstance<ILogger>(new FeatureLogger(context));
        }
    }
}
