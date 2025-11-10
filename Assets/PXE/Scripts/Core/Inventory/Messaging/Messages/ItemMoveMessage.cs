using PXE.Core.Inventory.UI;

namespace PXE.Core.Inventory.Messaging.Messages
{
    public struct ItemMoveMessage
    {
        public ItemContainerUI ItemContainerUi { get; }
        
        public ItemMoveMessage(ItemContainerUI itemContainerUi)
        {
            ItemContainerUi = itemContainerUi;
        }
    }
}