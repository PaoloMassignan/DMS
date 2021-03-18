using System;
using System.Collections.Generic;
using System.Text;

namespace DMS.Kernel
{
    public static class Utils
    {
        public static Dictionary<string,Int64> CreateOffset(this List<Message> messages)
        {
            var result = new Dictionary<string, Int64>();
            messages.ForEach(x => result.Add(x.Node, x.SequenceId));
            return result;
        }
        public static Int64 GetOffset(this Dictionary<string,Int64> offsets, string node)
        {
            Int64 result = -1;
            if (!offsets.TryGetValue(node, out result))
                return -1;
            return result;
        }

        public static bool CompareMessages(this List<DMSNode> nodes)
        {
            for (int i = 0; i < nodes.Count; i++)
            {
                for (int j = i+1; j < nodes.Count; j++)
                {
                    if (!nodes[i].Persistence.GetAllMessages().EqualsTo(nodes[j].Persistence.GetAllMessages()))
                        return false;
                }
            }
            return true;

        }

        public static bool SequenceIsRespected(this List<DMSNode> nodes)
        {
            for (int i =0; i < nodes.Count; i++)
            {
                Int64 id = -1;
                var messages = nodes[i].Persistence.GetAllMessages();
                for (int j = 0; j < messages.Count; j++)
                {
                    if (messages[j].Node == nodes[i].Name)
                    {
                        if (messages[j].SequenceId != id + 1)
                            return false;
                        id = messages[j].SequenceId;
                    }
                }
            }
            return true;
        }


        public static bool EqualsTo(this List<Message> msgs, List<Message> otherMsgs)
        {
            for (int i = 0; i < msgs.Count; i++)
            {
                bool found = false;
                for (int j = 0; j < otherMsgs.Count; j++)
                {
                    if (msgs[i].IsEqualTo(otherMsgs[j]))
                    {
                        found = true;
                        break;
                    }
                }
                if (!found)
                    return false;
            }
            return true;
        }

        public static string FixAddress(this string address)
        {
            address = address.Replace("http://","");
            address = address.Replace("https://","");
            return address;
        }


    }
}
