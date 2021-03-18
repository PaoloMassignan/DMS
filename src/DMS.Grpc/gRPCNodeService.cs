using DMSGrpcService;
using DMS.Kernel;
using DMS.Kernel.Interfaces;
using Grpc.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Unity;

namespace DMSGrpc
{
    public class gRPCNodeService : DMSNodeService.DMSNodeServiceBase
    {
        public UnityContainer Container { get; }
        public ILogger Logger => Container.Resolve<ILogger>();
        public DMSNode Node { get; set; }

        public gRPCNodeService(UnityContainer container)
        {
            Container = container;
        }

        public override Task<Null> DSMMessage(
                NetMessage message, ServerCallContext context)
        {
            lock (Node.MessageQueue)
            {
                try
                {
                    switch (message.MsgType)
                    {
                        case (int)gRPCMessageType.HeartBeat:
                            HeartBeat hb = JsonSerializer.Deserialize<HeartBeat>(message.MsgContent);
                            Node.MessageQueue.Add(hb);
                            break;
                        case (int)gRPCMessageType.UpdateRequest:
                            UpdateRequest updRequest = JsonSerializer.Deserialize<UpdateRequest>(message.MsgContent);
                            Node.MessageQueue.Add(updRequest);
                            break;
                        case (int)gRPCMessageType.UpdateResponse:
                            UpdateResponse updResponse = JsonSerializer.Deserialize<UpdateResponse>(message.MsgContent);
                            Node.MessageQueue.Add(updResponse);
                            break;
                    }
                }
                catch (Exception exc)
                {
                    Logger.Log(exc.Message);
                }
            }
            return Task.FromResult(new Null());
        }


    }
}
