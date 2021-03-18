using DMSGrpc;
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
    [Binding]
    class ClusterSteps
    {
        private static ClusterContext context;

        [BeforeFeature]
        public static void BeforeFeature(FeatureContext featureContext)
        {
            context = new ClusterContext(featureContext);
        }

        [Given(@"Nodes of the cluster \(grpc,memory\) are")]
        public void GivenNodesOfTheClusterAre(Table table)
        {
            context.Nodes.Clear();
            context.CloseEvents.Clear();

            for (int i = 0; i < table.RowCount; i++)
            {
                int port = Int32.Parse(table.Rows[i]["Port"]);
                string name = table.Rows[i]["Name"];

                DMSNode node = new DMSNode(context.Container,name,port);
                context.Nodes.Add(node);
                
                ManualResetEvent closeNode = new ManualResetEvent(false);
                context.CloseEvents.Add(name,closeNode);
                
                context.Container.RegisterInstance<ICommunicationLayer>(name,new gRPCCommunicationLayer(context.Container, closeNode));
                context.Container.RegisterInstance<IPersistenceLayer>(name, new InMemoryPersistence(name));
            }
            Thread.Sleep(100);
        }

        [When(@"'(.*)' starts")]
        public void WhenStarts(string nodeName)
        {

            DMSNode node = context.Nodes.FirstOrDefault(x => x.Name == nodeName);

            var closeNode = context.CloseEvents[nodeName];
            closeNode.Reset();
            node.HeartBeatFrequency = 500; //ms
            node.Setup(closeNode).Start();
            foreach (var n in context.Nodes)
            {
                if (n != node)
                {
                    node.AddRemoteNode(n.Name, String.Format("localhost:{0}",n.LocalPort));
                }
            }
            Thread.Sleep(100);
        }

        [When(@"'(.*)' connect to '(.*)'")]
        public void WhenConnectTo(string nodeName, string remoteName)
        {
            DMSNode node = context.Nodes.FirstOrDefault(x => x.Name == nodeName);
            
            DMSNode nodeRemote = context.Nodes.FirstOrDefault(x => x.Name == remoteName);
            node.AddRemoteNode(remoteName, String.Format("localhost:{0}", nodeRemote.LocalPort));
            Thread.Sleep(100);
        }

        [When(@"'(.*)' connect to address (.*)")]
        public void WhenConnectToAddress(string nodeName, string remoteAddress)
        {
            DMSNode node = context.Nodes.FirstOrDefault(x => x.Name == nodeName);
            node.AddRemoteNode(remoteAddress, remoteAddress);
            Thread.Sleep(100);
        }


        [When(@"'(.*)' starts server")]
        public void WhenStartsServer(string nodeName)
        {
            DMSNode node = context.Nodes.FirstOrDefault(x => x.Name == nodeName);

            var closeNode = context.CloseEvents[nodeName];
            closeNode.Reset();
            node.HeartBeatFrequency = 500; //ms
            node.Setup(closeNode).Start();

            Thread.Sleep(100);
        }


        [When(@"'(.*)' ends")]
        public void WhenEnds(string nodeName)
        {
            DMSNode node = context.Nodes.FirstOrDefault(x => x.Name == nodeName);
            var closeEvent = context.CloseEvents[nodeName];
            closeEvent.Set();
            Thread.Sleep(100);
        }



        [When(@"'(.*)' writes (.*) messages")]
        public void WhenWritesMessages(string nodeName, int numberOfMessages)
        {
            DMSNode node = context.Nodes.FirstOrDefault(x => x.Name == nodeName);
            for (int i =0; i < numberOfMessages; i++)
            {
                node.Persistence.NewMessage("test", "test");
            }

            Thread.Sleep(100);
        }

        [When(@"'(.*)' offset of '(.*)' is (.*)")]
        public void WhenOffsetOfIs(string nodeName, string nodeReference, int offset)
        {
            DMSNode node = context.Nodes.FirstOrDefault(x => x.Name == nodeName);
            Assert.AreEqual(node.Persistence.GetCurrentPersistedOffset()[nodeReference],offset);
        }

        [When(@"'(.*)' see '(.*)' down")]
        public void WhenSeeDown(string firstNode, string secondNode)
        {
            DMSNode node = context.Nodes.FirstOrDefault(x => x.Name == firstNode);
            Assert.IsTrue(node.RemoteNodes.FirstOrDefault(x => x.Name == secondNode).Status != NodeStatus.Online); 
        }


        [When(@"Wait (.*) ms")]
        public void WhenWaitMs(int ms)
        {
            Thread.Sleep(100);
        }

        [Then(@"test ends")]
        public void ThenTestEnds()
        {
            foreach (var ce in context.CloseEvents.Values)
            {
                ce.Set();
            }
        }

    }
}
