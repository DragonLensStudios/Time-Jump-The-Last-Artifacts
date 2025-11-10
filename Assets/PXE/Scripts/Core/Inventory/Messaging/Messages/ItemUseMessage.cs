using PXE.Core.Inventory.Items;
using PXE.Core.SerializableTypes;

namespace PXE.Core.Inventory.Messaging.Messages
{
    public struct ItemUseMessage
    {
        public SerializableGuid TargetID { get; }
        public ItemObject Item { get; }
        public int Quantity { get; }
        public ItemUseMessage(SerializableGuid targetID, ItemObject item, int quantity = 1)
        {
            TargetID = targetID;
            Item = item;
            Quantity = quantity;
        }
    }
}