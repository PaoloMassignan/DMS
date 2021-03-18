using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DMS.Kernel
{
    public class RemoteNode
    {


        public string Name { get; set; }
        public string Address { get; set; }
        public NodeStatus Status { get; set; }
        public MessageOffset Offset { get; set; }
    }
}
