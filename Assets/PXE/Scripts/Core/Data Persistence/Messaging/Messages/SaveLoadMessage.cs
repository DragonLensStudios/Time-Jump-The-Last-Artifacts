using PXE.Core.Enums;
using PXE.Core.SerializableTypes;

namespace PXE.Core.Data_Persistence.Messaging.Messages
{
    public struct SaveLoadMessage
    {
        public SerializableGuid PlayerID { get; }
        
        public string PlayerName { get; }
        
        public SaveOperation SaveOperationType { get; }
        
        public SaveLoadMessage(SerializableGuid playerID, string playerName, SaveOperation saveOperationType)
        {
            PlayerID = playerID;
            PlayerName = playerName;
            SaveOperationType = saveOperationType;
        }
        
    }
}