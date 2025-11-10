using PXE.Core.Inventory.UI;

namespace PXE.Core.Inventory.Messaging.Messages
{
    public struct ItemInteractMessage
    {
        public ItemContainerUI ItemContainerUI { get; }
        public ItemInteractMessage(ItemContainerUI itemContainerUI)
        {
            ItemContainerUI = itemContainerUI;
        }
    }
}