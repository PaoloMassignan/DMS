using DMSGrpc;
using DMS.Kernel;
using DMS.Kernel.Interfaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading;
using Unity;

namespace DMS.Test
{
    [TestClass]
    public class TestGrpcMemory
    {
        public TestContext TestContext { get; set; }

        [TestMethod]
        public void TestRunTwoNodes()
        {
            UnityContainer container = new UnityContainer();

            ManualResetEvent closeNodeA = new ManualResetEvent(false);
            ManualResetEvent closeNodeB = new ManualResetEvent(false);

            // Logger
            container.RegisterInstance<ILogger>(new TestLogger(TestContext));

            // Communication
            container.RegisterInstance<ICommunicationLayer>("A",new gRPCCommunicationLayer(container,closeNodeA));
            container.RegisterInstance<ICommunicationLayer>("B",new gRPCCommunicationLayer(container,closeNodeB));

            // Persistence
            container.RegisterInstance<IPersistenceLayer>("A", new InMemoryPersistence("A"));
            container.RegisterInstance<IPersistenceLayer>("B", new InMemoryPersistence("B"));



            DMSNode nodeA = new DMSNode(container, "A", 19001);
            nodeA.HeartBeatFrequency = 3000; //ms
            nodeA.Setup(closeNodeA).Start();
            nodeA.AddRemoteNode("B", "http://localhost:19002");

            Thread.Sleep(2000);

            DMSNode nodeB = new DMSNode(container, "B", 19002);
            nodeB.HeartBeatFrequency = 3000; //ms
            nodeB.Setup(closeNodeB).Start();
            nodeB.AddRemoteNode("A", "http://localhost:19001");

            Thread.Sleep(10000);

            closeNodeA.Set();
            closeNodeB.Set();
            
        }


        [TestMethod]
        public void TestRunTwoNodesAWakeUpLater()
        {
            UnityContainer container = new UnityContainer();

            ManualResetEvent closeNodeA = new ManualResetEvent(false);
            ManualResetEvent closeNodeB = new ManualResetEvent(false);

            // Communication
            container.RegisterInstance<ICommunicationLayer>("A", new gRPCCommunicationLayer(container, closeNodeA));
            container.RegisterInstance<ICommunicationLayer>("B", new gRPCCommunicationLayer(container, closeNodeB));

            // Persistence
            container.RegisterInstance<IPersistenceLayer>("A", new InMemoryPersistence("A"));
            container.RegisterInstance<IPersistenceLayer>("B", new InMemoryPersistence("B"));

            // Logger
            container.RegisterInstance<ILogger>(new TestLogger(TestContext));


            DMSNode nodeA = new DMSNode(container, "A", 19001);
            nodeA.HeartBeatFrequency = 3000; //ms
            nodeA.Setup(closeNodeA).Start();
            nodeA.AddRemoteNode("B", "http://localhost:19002");

            Thread.Sleep(5000);

            DMSNode nodeB = new DMSNode(container, "B", 19002);
            nodeB.HeartBeatFrequency = 3000; //ms
            nodeB.Setup(closeNodeB).Start();
            nodeB.AddRemoteNode("A", "http://localhost:19001");

            Thread.Sleep(5000);

            closeNodeA.Set();
            closeNodeB.Set();

        }
    }
}
