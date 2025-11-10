using PXE.Core.Enums;
using PXE.Core.Messaging.Message_Config_Objects;

namespace PXE.Core.Dialogue.Nodes.CustomNodes
{
    public class MessageSenderNode : BaseNode
    {
        [Input] public Connection Input;
        [Output] public Connection Exit;
        public MessageChannels Channel;
        public MessageConfigBaseObject MessageConfig;
        public MessageSendAction SendAction;


    }
}