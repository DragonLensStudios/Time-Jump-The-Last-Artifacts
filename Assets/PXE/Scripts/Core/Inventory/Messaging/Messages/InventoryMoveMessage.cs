using PXE.Core.Inventory.Items;
using PXE.Core.SerializableTypes;

namespace PXE.Core.Inventory.Messaging.Messages
{
    public struct InventoryMoveMessage
    {
        public SerializableGuid InventoryID { get; }
        public ItemObject ItemObject { get; }
        public int Amount { get; }
        public int FromIndex { get; }
        public int ToIndex { get; }

        public InventoryMoveMessage(SerializableGuid inventoryID, ItemObject itemObject, int amount, int fromIndex,int toIndex)
        {
            InventoryID = inventoryID;
            ItemObject = itemObject;
            Amount = amount;
            FromIndex = fromIndex;
            ToIndex = toIndex;
        }
    }
}