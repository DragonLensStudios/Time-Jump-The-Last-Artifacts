using PXE.Core.Enums;
using PXE.Core.Inventory.Items;
using PXE.Core.SerializableTypes;

namespace PXE.Core.Inventory.Messaging.Messages
{
    public struct InventoryModifyMessage
    {
        public SerializableGuid InventoryID { get; }
        public ItemObject Item { get; }
        public int Quantity { get; }
        public InventoryModifyType Operation { get; }

        public InventoryModifyMessage(SerializableGuid inventoryID, ItemObject item, int quantity, InventoryModifyType operation)
        {
            InventoryID = inventoryID;
            Item = item;
            Quantity = quantity;
            Operation = operation;
        }
    }
}