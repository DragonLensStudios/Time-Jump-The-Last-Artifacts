using PXE.Core.Enums;
using PXE.Core.Inventory.Items;
using PXE.Core.SerializableTypes;

namespace PXE.Core.Inventory.Messaging.Messages
{
    public struct ItemUnequipMessage
    {
        public SerializableGuid TargetID { get; }
        public EquipmentSlot Slot { get; }
        public ItemObject ItemObject { get; }
        public int Quantity { get; }

        public ItemUnequipMessage(SerializableGuid targetID, EquipmentSlot slot, ItemObject itemObject = null, int quantity = 1)
        {
            TargetID = targetID;
            Slot = slot;
            ItemObject = itemObject;
            Quantity = quantity;
        }
    }
}