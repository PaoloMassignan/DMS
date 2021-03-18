using DMS.Kernel.Interfaces;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Unity;

namespace DMS.Kernel.Test.Acceptance_Tests
{
    public class InitNodeContext
    {
        public UnityContainer Container { get; }
        public UpdateResponse LastUpdateResponse { get; internal set; }
        public List<DMSNode> Nodes { get; internal set; }
        public Dictionary<string,int> NodeEventFrequency { get; set; }
        public List<SimulationEvent> Simulations { get; set; }
        public int CurrentTime { get; private set; }
        public InMemoryCommunication Communication { get; private set; }

        public InitNodeContext()
        {
            Container = new UnityContainer();
            NodeEventFrequency = new Dictionary<string, int>();
            Simulations = new List<SimulationEvent>();
        }

        internal void AddMockForRemoteNode(string nodeName)
        {
            var mockLogger = new Mock<ILogger>();
            mockLogger.Setup(x => x.Log(It.IsAny<string>()));

            Communication = new InMemoryCommunication();

            Container.RegisterInstance<IPersistenceLayer>(nodeName,new InMemoryPersistence(nodeName));
            Container.RegisterInstance<ICommunicationLayer>(nodeName,Communication);
            Container.RegisterInstance<ILogger>(mockLogger.Object);
        }

        public T Resolve<T>()
        {
            return Container.Resolve<T>();
        }

        internal void RegisterMessagesFor(string nodeName, List<Message> messages)
        {
            var node = Nodes.FirstOrDefault(x => x.Name == nodeName);
            node.Persistence.AddMessageRange(messages);
        }

        internal void StartSimulation(int end)
        {
            bool[] nodeStarted = new bool[Nodes.Count];

            for (CurrentTime = 0; CurrentTime <= end; CurrentTime++)
            {
                var eventsAtThisTime = Simulations.Where(x => x.T == CurrentTime);
                foreach (var e in eventsAtThisTime)
                {
                    if (e.Type == SimulationEvent.SimulationEventType.Start)
                    {
                        nodeStarted[Nodes.FindIndex(x => x.Name == e.Node.Name)] = true;
                    }
                    if (e.Type == SimulationEvent.SimulationEventType.Stop)
                    {
                        nodeStarted[Nodes.FindIndex(x => x.Name == e.Node.Name)] = false;
                    }
                }

                for (int i = 0; i < Nodes.Count; i++)
                {
                    if (CurrentTime % NodeEventFrequency[Nodes[i].Name] ==0 )
                    {
                        if (nodeStarted[i])
                        {
                            // Save the message in the persistence layer
                            Nodes[i].Persistence.NewMessage("topic", "data");
                        }
                    }
                }
                for (int i = 0; i < Nodes.Count; i++)
                {
                    if (CurrentTime % Nodes[i].HeartBeatFrequency == 0)
                    {
                        // Save the message in the persistence layer
                        Nodes[i].SendHeartBeat();
                    }
                }
                Communication.ElaborateCommands(int.MaxValue);
            }
        }

        internal void ContinueSimulation(int end)
        {
            for (; CurrentTime <= end; CurrentTime++)
            {
                for (int i = 0; i < Nodes.Count; i++)
                {
                    if (CurrentTime % Nodes[i].HeartBeatFrequency == 0)
                    {
                        // Save the message in the persistence layer
                        Nodes[i].SendHeartBeat();
                    }
                }
                Communication.ElaborateCommands(int.MaxValue);
            }
        }
    }

    #region Simulation
    public class SimulationEvent
    {
        public enum SimulationEventType
        {
            Start,
            Stop,
            NodeIsolated,
            NodeRestarted,
        }
        public SimulationEventType Type { get; set; }
        public DMSNode Node { get; set; }
        public int T { get; set; }
    }
    #endregion
}
