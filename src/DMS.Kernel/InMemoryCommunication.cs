using DMS.Kernel.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DMS.Kernel
{
    public class InMemoryCommunication : ICommunicationLayer
    {
        public InMemoryCommunication()
        {
        }
        private static List<IMessage> _net = null;
        public static List<IMessage> Net
        {
            get
            {
                if (_net == null)
                    _net = new List<IMessage>();
                return _net;
            }
        }

        public List<DMSNode> Nodes { get; set; }
        public UpdateResponse LastUpdateResponse { get; private set; }
        public DMSNode Node { get; set; }
        public ManualResetEvent ExitEvent { get; set; }

        public void SendHeartBeat(HeartBeat heartBeat)
        {
            Net.Add(heartBeat);
        }

        public void SendUpdateRequest(UpdateRequest updateRequest)
        {
            Net.Add(updateRequest);
        }

        public void SendUpdateResponse(UpdateResponse updateResponse)
        {
            Net.Add(updateResponse);
        }

        public void ElaborateAllCommands()
        {
            ElaborateCommands(int.MaxValue);
        }
        public void ElaborateCommands(int numberOfCommands)
        {
            for (int i = 0; i < Net.Count && i < numberOfCommands; i++)
            {
                var node = Nodes.FirstOrDefault(x => x.Name == Net[0].To);
                Net[0].OnReceive(node);
                if (Net[0] is UpdateResponse)
                    LastUpdateResponse = Net[0] as UpdateResponse;
                Net.RemoveAt(0);
                i--;
            }
        }

        public void AddClientTo(string remoteNodeName, string remoteAddress)
        {
            
        }

        public void Start(int localPort)
        {
        }

        public void Close()
        {
        }
    }
}
