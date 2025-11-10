using PXE.Core.Inventory.Items;
using PXE.Core.SerializableTypes;

namespace PXE.Core.Inventory.Messaging.Messages
{
    public struct ItemPickupMessage
    {
        public SerializableGuid TargetID { get; }
        public ItemObject ItemObject { get; }
        
        public int Amount { get; }

        public ItemPickupMessage(SerializableGuid targetID, ItemObject itemObject, int amount)
        {
            TargetID = targetID;
            ItemObject = itemObject;
            Amount = amount;
        }
    }
}