using DMSGrpcService;
using DMS.Kernel;
using DMS.Kernel.Interfaces;
using Grpc.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Unity;

namespace DMSGrpc
{
    public class gRPCClient : DMSNodeService.DMSNodeServiceClient
    {
        //private ILogger Logger { get; set; }
        //public ManualResetEvent ExitEvent { get; }
        //public string RemoteNode { get; }
        public DMSNode Node { get; set; }

        public gRPCClient(DMSNode node, ChannelBase channel) :base(channel)
        {
            Node = node;
            //RemoteNode = remoteNode;
            //Logger = container.Resolve<ILogger>();
            //OutQueue = new gRPCMessageQueue();
            //ExitEvent = exit;
        }

        //internal bool Send(NetMessage msg)
        //{
        //    var remoteNode = Node.RemoteNodes.FirstOrDefault(x => x.Name == this.RemoteNode);
        //    if (remoteNode != null && remoteNode.Status == NodeStatus.Online)
        //    {
        //        lock (OutQueue)
        //        {
        //            OutQueue.Add(msg);

        //        }
        //        return true;
        //    }
        //    return false;
        //}
    }
}
