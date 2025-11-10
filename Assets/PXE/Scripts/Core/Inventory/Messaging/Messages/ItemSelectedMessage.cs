using PXE.Core.Inventory.Items;

namespace PXE.Core.Inventory.Messaging.Messages
{
    public struct ItemSelectedMessage
    {
        public ItemSlot ItemSlot { get; }
        public ItemSelectedMessage(ItemSlot itemSlot)
        {
            ItemSlot = itemSlot;
        }
    }
}