using PXE.Core.Dialogue.Interaction;
using PXE.Core.Enums;
using PXE.Core.SerializableTypes;

namespace PXE.Core.Dialogue.Messaging.Messages
{
    public struct EndLevelMessage
    {
        public bool IsWin { get; }
        
        public EndLevelMessage(bool isWin)
        {
            IsWin = isWin;
        }
    }
}