using PXE.Core.Inventory.Items;
using PXE.Core.SerializableTypes;

namespace PXE.Core.Inventory.Messaging.Messages
{
    public struct InventoryChangedMessage
    {
        public SerializableGuid ID { get; }
        public InventoryObject Inventory { get; }

        public InventoryChangedMessage(SerializableGuid id, InventoryObject inventory)
        {
            ID = id;
            Inventory = inventory;
        }
    }
}