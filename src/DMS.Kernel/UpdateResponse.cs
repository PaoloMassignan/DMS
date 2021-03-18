using DMS.Kernel.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace DMS.Kernel
{
    public class UpdateResponse : IMessage
    {
        public List<Message> Messages { get; set; }
        public string From { get; set; }
        public string To { get; set; }

        public void OnReceive(DMSNode node)
        {
            node.OnUpdateResponse(this);
        }
    }
}
