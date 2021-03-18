using DMS.Kernel.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Unity;

namespace DMS.Kernel
{
    public class DMSNode
    {
        public UnityContainer Container { get; }
        public string Name { get; }
        public IPersistenceLayer Persistence => Container.Resolve<IPersistenceLayer>(Name);
        public ICommunicationLayer Communication => Container.Resolve<ICommunicationLayer>(Name);
        public ILogger Logger => Container.Resolve<ILogger>();
        public List<RemoteNode> RemoteNodes { get; set; }
        public List<IMessage> MessageQueue { get; set; }
        public int LocalPort { get; }
        public int HeartBeatFrequency { get; set; }


        public DMSNode(UnityContainer container, string name, int localPort)
        {
            Container = container;
            MessageQueue = new List<IMessage>();
            this.Name = name;
            this.RemoteNodes = new List<RemoteNode>();
            this.LocalPort = localPort;
        }

        public DMSNode()
        {
        }

        public Task Setup(ManualResetEvent closeEvent)
        {
            Communication.Node = this;
            return new Task(() =>
            {
                Logger.Log(String.Format("Starting DMS Node '{0}' at port '{1}...", Name, LocalPort));
                Communication.Start(LocalPort);
                DateTime lastHeartBeat = DateTime.Now;

                try
                {

                    while (true)
                    {
                        if (DateTime.Now.Subtract(lastHeartBeat).TotalMilliseconds > HeartBeatFrequency)
                        {
                            Logger.Log(String.Format("HeartBeat...{0} {1}", Name, Persistence.GetCurrentPersistedOffset()));
                            SendHeartBeat();
                            lastHeartBeat = DateTime.Now;
                        }

                        lock (MessageQueue)
                        {
                            for (int i = 0; i < MessageQueue.Count; i++)
                            {
                                switch (MessageQueue[i])
                                {
                                    case HeartBeat heartBeat:
                                        OnHeartBeat(heartBeat);
                                        break;
                                    case UpdateRequest updateRequest:
                                        OnUpdateRequest(updateRequest);
                                        break;
                                    case UpdateResponse updateResponse:
                                        OnUpdateResponse(updateResponse);
                                        break;
                                }
                            }
                            MessageQueue.Clear();
                        }



                        if (closeEvent.WaitOne(100))
                            break;
                    }
                }
                catch(Exception e)
                {
                    Logger.Log(String.Format("********'{0}'",e.Message));

                }
                Communication.Close();
                Logger.Log(String.Format("Closing DMS Node '{0}' at port '{1}...", Name, LocalPort));
            });
        }

        public void OnUpdateResponse(UpdateResponse updateResponse)
        {
            Logger.Log(String.Format("[{0}] received UpdateResponse from '{1}' with {2} messages", updateResponse.To, updateResponse.From, updateResponse.Messages.Count));
            Persistence.AddMessageRange(updateResponse.Messages);
        }

        public void OnHeartBeat(HeartBeat heartBeat)
        {
            Logger.Log(String.Format("HeartBeat received from '{0}' with offset {1}", heartBeat.From, heartBeat.NodeOffset));
            var remoteNode = RemoteNodes.FirstOrDefault(x => x.Name == heartBeat.From);
            if (remoteNode == null)
            {
                Logger.Log(String.Format("HeartBeat received from unknown node"));
            }
            else
            {
                remoteNode.Offset = heartBeat.NodeOffset;
            }

            var currentOffset = Persistence.GetCurrentPersistedOffset();
            if (heartBeat.NodeOffset != null)
            {

                if (heartBeat.NodeOffset.GraterThan(currentOffset))
                {
                    UpdateRequest updateRequest = new UpdateRequest();
                    updateRequest.Offset = currentOffset;

                    updateRequest.From = this.Name;
                    updateRequest.To = heartBeat.From;

                    SendUpdateRequest(updateRequest);
                }
            }

        }

        public NodeStatus Status { get; set; }


        public void OnNodeStatus(string nodeName, NodeStatus status)
        {
            var remoteNode = RemoteNodes.FirstOrDefault(x => x.Name == nodeName);
            if (remoteNode != null)
            {
                if (remoteNode.Status != status)
                {
                    Logger.Log(String.Format("[{0}] Remote Node '{1}' new status '{2}'", Name, nodeName, status));
                    remoteNode.Status = status;
                }
            }
            else
                Logger.Log(String.Format("{0}] unknown remote node '{1}'", Name, nodeName));
        }

        public void OnUpdateRequest(UpdateRequest updateRequest)
        {
            Logger.Log(String.Format("[{0}] received UpdateRequest  from '{1}' with offset {2}", updateRequest.To, updateRequest.From, updateRequest.Offset));
            var newMessages = Persistence.GetMessagesFrom(updateRequest.Offset);
            var response = new UpdateResponse() { Messages = newMessages, From = Name, To = updateRequest.From };
            Logger.Log(String.Format("[{0}] answer with UpdateResponse Msg Count {1}", updateRequest.To, response.Messages.Count));
            Communication.SendUpdateResponse(response);
        }

        public void SendUpdateRequest(UpdateRequest updateRequest)
        {
            Communication.SendUpdateRequest(updateRequest);
        }

        public void AddRemoteNode(RemoteNode remoteNode)
        {
            RemoteNodes.Add(remoteNode);
        }
        public void AddRemoteNode(string remoteNodeName, string remoteAddress)
        {
            var remNode = new RemoteNode() { Name = remoteNodeName, Address = remoteAddress };
            RemoteNodes.Add(remNode);
            Communication.AddClientTo(remoteNodeName, remoteAddress);
        }
        public UpdateRequest CreateUpdateRequest(MessageOffset offset, string to)
        {
            return new UpdateRequest() { Offset = offset, From = Name, To = to };
        }
        public UpdateRequest CreateUpdateRequest(string to)
        {
            return CreateUpdateRequest(Persistence.GetCurrentPersistedOffset(),to);
        }
        public void SendHeartBeat()
        {
            var currentOffset = Persistence.GetCurrentPersistedOffset();

            foreach (var remoteNode in RemoteNodes)
            {
                HeartBeat heartBeat = new HeartBeat();
                heartBeat.From = Name;
                heartBeat.To = remoteNode.Name;
                heartBeat.NodeOffset = currentOffset;

                Communication.SendHeartBeat(heartBeat);
            }
        }
    }
}
