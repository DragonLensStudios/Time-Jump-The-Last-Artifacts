using PXE.Core.Inventory.Items;
using PXE.Core.SerializableTypes;

namespace PXE.Core.Inventory.Messaging.Messages
{
    public struct ItemDestroyMessage
    {
        public SerializableGuid TargetID { get; }
        public ItemObject ItemObject { get; }
        public int Quantity { get; }

        public ItemDestroyMessage(SerializableGuid targetID, ItemObject itemObject, int quantity = 1)
        {
            TargetID = targetID;
            ItemObject = itemObject;
            Quantity = quantity;
        }
    }
}