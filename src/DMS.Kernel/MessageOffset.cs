using System;
using System.Collections.Generic;
using System.Text;

namespace DMS.Kernel
{
    public class MessageOffset : Dictionary<string, Int64>
    {
        public MessageOffset()
        {

        }
        public MessageOffset(List<Message> msgs)
        {
            msgs.ForEach(x => Set(x.Node, x.SequenceId));
        }

        internal void Set(string nodeName, long sequenceId)
        {
            this[nodeName] = sequenceId;
        }

        internal bool GraterThan(MessageOffset messageOffset)
        {
            foreach (var k in Keys)
            {
                if (!messageOffset.ContainsKey(k))
                    return true;
                if (this[k] > messageOffset[k])
                    return true;
            }
            return false;
        }

        public override string ToString()
        {
            String result = "[";
            foreach (var k in this.Keys)
            {
                result += String.Format("{0}:{1}|", k, this[k]);
            }
            result += "]";
            return result;
        }
    }
}
