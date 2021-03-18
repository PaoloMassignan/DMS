using System;
using System.Collections.Generic;
using System.Text;

namespace DMS.Kernel.Interfaces
{
    public interface IPersistenceLayer
    {
        string NodeName { get; }
        MessageOffset GetCurrentPersistedOffset();
        void AddMessageRange(List<Message> messages);
        List<Message> GetMessagesFrom(MessageOffset offset);
        void NewMessage(string topic, string data);
        List<Message> GetAllMessages();
    }
}
