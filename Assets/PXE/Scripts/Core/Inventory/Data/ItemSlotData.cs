using PXE.Core.Inventory.Items;
using UnityEngine;

namespace PXE.Core.Inventory.Data
{
    [System.Serializable]
    public class ItemSlotData
    {
        [field: SerializeField] public virtual ItemData Item { get; set; }
        [field: SerializeField] public virtual int Quantity { get; set; }

        public ItemSlotData()
        {
            
        }

        public ItemSlotData(ItemSlot itemSlot)
        {
            Item = new ItemData(itemSlot.Item);
            Quantity = itemSlot.Quantity;
        }
    }
}