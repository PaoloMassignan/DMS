using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DMS.Kernel.Interfaces
{
    public interface ICommunicationLayer
    {
        DMSNode Node { get; set; }
        ManualResetEvent ExitEvent { get; set; }
        void SendUpdateRequest(UpdateRequest updateRequest);
        void SendHeartBeat(HeartBeat heartBeat);
        void SendUpdateResponse(UpdateResponse updateResponse);
        void Start(int localPort);
        void AddClientTo(string remoteNodeName, string remoteAddress);
        void Close();
    }
}
