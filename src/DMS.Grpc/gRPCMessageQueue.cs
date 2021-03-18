using DMSGrpcService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DMSGrpc
{
    public class gRPCMessageQueue:List<NetMessage>
    {
        public NetMessage Pop()
        {
            if (this.Count >0)
            {
                var tmp = this[0];
                this.RemoveAt(0);
                return tmp;
            }
            return null;
        }
    }
}
