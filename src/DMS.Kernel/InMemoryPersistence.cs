using DMS.Kernel.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DMS.Kernel
{
    public class InMemoryPersistence:IPersistenceLayer
    {
        public string NodeName { get; set; }
        public MessageOffset CurrentOffset { get; set; }
        private List<Message> Messages { get; set; }

        public InMemoryPersistence(string nodeName)
        {
            Messages = new List<Message>();
            CurrentOffset = new MessageOffset();
            NodeName = nodeName;
        }

        public void AddMessage(Message msg)
        {
            Messages.Add(msg);
            CurrentOffset.Set(msg.Node, msg.SequenceId);
        }

        public void AddMessageRange(List<Message> messages)
        {
            messages.ForEach(x => AddMessage(x));
        }

        public List<Message> GetMessagesFrom(MessageOffset offset)
        {
            // Find oldest offset
            List<Message> result = new List<Message>();
            int index = FindOldestOffset(offset);
            if (index < 0)
                index = 0;
            for (int i = index; i < Messages.Count; i++)
            {
                if (Messages[i].SequenceId > offset.GetOffset(Messages[i].Node))
                    result.Add(Messages[i]);
            }
            return result;
        }

        private int FindOldestOffset(MessageOffset offset)
        {
            bool allMessages = CurrentOffset.Keys.FirstOrDefault(x=> !offset.ContainsKey(x))!= null;
            if (allMessages)
                return -1;
            return Messages.FindIndex(x => x.SequenceId < offset.GetOffset(x.Node));
        }
        public void NewMessage(string topic, string data)
        {
            Int64 newSequenceId = 0;
            if (CurrentOffset.TryGetValue(NodeName, out newSequenceId))
                newSequenceId++;

            AddMessage(new Message() { SequenceId = newSequenceId, Topic = topic, Node = NodeName, Data = data });
        }

        public MessageOffset GetCurrentPersistedOffset()
        {
            return CurrentOffset;
        }

        public List<Message> GetAllMessages()
        {
            return Messages;
        }

        public void Reset()
        {
            throw new NotImplementedException();
        }
    }
}
