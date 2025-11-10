using PXE.Core.Inventory.UI;

namespace PXE.Core.Inventory.Messaging.Messages
{
    public struct ItemSplitMessage
    {
        public ItemContainerUI ItemContainerUi { get; }
        public ItemSplitMessage(ItemContainerUI itemContainerUI)
        {
            ItemContainerUi = itemContainerUI;
        }
    }
}