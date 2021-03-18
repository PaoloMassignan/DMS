using System;
using System.Collections.Generic;
using System.Text;

namespace DMS.Kernel.Interfaces
{
    public interface IMessage
    {
        string From { get; set; }
        string To { get; set; }

        void OnReceive(DMSNode node);
    }
}
