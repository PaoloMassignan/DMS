using DMS.Kernel.Interfaces;

namespace DMS.Kernel
{
    public class UpdateRequest:IMessage
    {
        public UpdateRequest()
        {
            Offset = new MessageOffset();
        }
        public MessageOffset Offset { get; set; }
        public string From { get; set; }
        public string To { get; set; }

        public void OnReceive(DMSNode node)
        {
            node.OnUpdateRequest(this);
        }
    }
}
