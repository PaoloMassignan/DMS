using DMSGrpc;
using DMS.Kernel;
using DMS.Kernel.Interfaces;
using System;
using System.Threading;
using Unity;

namespace DMS.Service
{
    class Program
    {
        static void Main(string[] args)
        {
            int port = 20122;
            string nodeName = "node";
            if (args.Length > 0)
                nodeName = args[0];
            if (args.Length > 1)
                port = Int32.Parse(args[1]);

            UnityContainer container = new UnityContainer();

            ManualResetEvent closeApplication = new ManualResetEvent(false);

            // Communication
            gRPCCommunicationLayer gRPC = new gRPCCommunicationLayer(container,closeApplication);
            container.RegisterInstance<ICommunicationLayer>(nodeName,gRPC);

            // Persistence
            InMemoryPersistence memory = new InMemoryPersistence(nodeName);
            container.RegisterInstance<IPersistenceLayer>(nodeName,memory);

            // Logger
            container.RegisterInstance<ILogger>(new ConsoleLogger());


            Console.WriteLine();

            DMSNode node = new DMSNode(container,nodeName, port);
            node.HeartBeatFrequency = 3000; //ms
            node.Setup(closeApplication).Start();
            Console.ReadKey();

            closeApplication.Set();
        }
    }
}
