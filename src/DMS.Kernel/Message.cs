using System;
using System.Collections.Generic;
using System.Text;

namespace DMS.Kernel
{
    public class Message
    {
        public Int64 SequenceId { get; set; }
        public string Topic { get; set; }
        public string Node { get; set; }
        public string Data { get; set; }

        public override string ToString()
        {
            return String.Format("ID: {0} Node:'{1}' Topic '{2}", SequenceId, Node, Topic);
        }

        internal bool IsEqualTo(Message message)
        {
            return SequenceId == message.SequenceId && Topic == message.Topic && Node == message.Node && Data == message.Data;
        }
    }
}
