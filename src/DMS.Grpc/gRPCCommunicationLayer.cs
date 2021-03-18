using DMSGrpcService;
using DMS.Kernel;
using DMS.Kernel.Interfaces;
using Grpc.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Unity;

namespace DMSGrpc
{
    public class gRPCCommunicationLayer : ICommunicationLayer
    {
        private Server server;

        public DMSNode Node { get; set; }
        public Dictionary<string, DMSNodeService.DMSNodeServiceClient> Clients { get; }
        public ManualResetEvent ExitEvent { get; set; }
        public UnityContainer Container { get; set; }
        public ILogger Logger => Container.Resolve<ILogger>();

        public gRPCCommunicationLayer(UnityContainer container, ManualResetEvent closeCommunication)
        {
            Clients = new Dictionary<string, DMSNodeService.DMSNodeServiceClient>();
            ExitEvent = closeCommunication;
            Container = container;
        }


        public gRPCNodeService Server { get; private set; }

        public void SendHeartBeat(HeartBeat heartBeat)
        {
            new Task(() =>
            {
                NetMessage msg = new NetMessage();
                msg.MsgType = (int)gRPCMessageType.HeartBeat;
                msg.MsgContent = JsonSerializer.Serialize(heartBeat);

                DMSNodeService.DMSNodeServiceClient client = null;
                if (Clients.TryGetValue(heartBeat.To, out client))
                {
                    try
                    {
                        client.DSMMessage(msg, deadline: DateTime.UtcNow.AddSeconds(1));
                        Logger.Log(String.Format("[{0}] sent an heartbeat to '{1}'", heartBeat.From, heartBeat.To));
                    }
                    catch (Exception exc)
                    {

                    }
                }
            }).RunSynchronously();
        }
        public void SendUpdateRequest(UpdateRequest updateRequest)
        {
            NetMessage msg = new NetMessage();
            msg.MsgType = (int)gRPCMessageType.HeartBeat;
            msg.MsgContent = JsonSerializer.Serialize(updateRequest);

            DMSNodeService.DMSNodeServiceClient client = null;
            if (Clients.TryGetValue(updateRequest.To, out client))
            {
                try
                {
                    client.DSMMessage(msg, deadline: DateTime.UtcNow.AddSeconds(3));
                    Logger.Log(String.Format("[{0}] sending updateRequest to '{1}'", updateRequest.From, updateRequest.To));
                }
                catch (Exception exc)
                {

                }
            }
        }
        public void SendUpdateResponse(UpdateResponse updateResponse)
        {
            NetMessage msg = new NetMessage();
            msg.MsgType = (int)gRPCMessageType.HeartBeat;
            msg.MsgContent = JsonSerializer.Serialize(updateResponse);

            DMSNodeService.DMSNodeServiceClient client = null;
            if (Clients.TryGetValue(updateResponse.To, out client))
            {
                try
                {
                    client.DSMMessage(msg, deadline: DateTime.UtcNow.AddSeconds(3));
                    Logger.Log(String.Format("[{0}] sending updateResponse to '{1}'", updateResponse.From, updateResponse.To));
                }
                catch (Exception exc)
                {

                }
            }
        }

        public void Start(int localPort)
        {
            Server = new gRPCNodeService(Container) { Node = Node };

            try
            {
                server = new Server
                {
                    Services = { DMSGrpcService.DMSNodeService.BindService(Server) },
                    Ports = { new ServerPort("localhost", localPort, ServerCredentials.Insecure) },

                };
                server.Start();
            }
            catch (Exception exc)
            {
                Logger.Log(exc.Message);
            }
        }

        public void AddClientTo(string remoteNodeName, string remoteAddress)
        {
            Channel channel = new Channel(remoteAddress, ChannelCredentials.Insecure);
            gRPCClient client = new gRPCClient(Node,channel);

            Clients.Remove(remoteNodeName);
            Clients.Add(remoteNodeName, client);
        }

        public void Close()
        {
            if (server != null)
                server.ShutdownAsync().Wait();
        }
    }


}
