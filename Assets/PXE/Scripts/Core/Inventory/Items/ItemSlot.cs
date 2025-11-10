using PXE.Core.Inventory.Interfaces;
using PXE.Core.SerializableTypes;
using UnityEngine;

namespace PXE.Core.Inventory.Items
{
    /// <summary>
    ///  This class represents the item slot.
    /// </summary>
    [System.Serializable]
    public class ItemSlot : IItemSlot
    {
        [field: SerializeField] public string Name { get; set; }
        [field: SerializeField] public virtual SerializableGuid ID { get; set; }
        [field: SerializeField] public bool IsManualID { get; set; }

        [field: Tooltip("The item in the slot.")]
        [field: SerializeField] public virtual ItemObject Item { get; set; }
        
        [field: Tooltip("The quantity of the item in the slot.")]
        [field: SerializeField] public virtual int Quantity  { get; set; }

        /// <summary>
        ///  Initializes a new instance of the <see cref="ItemSlot"/> class.
        /// </summary>
        public ItemSlot()
        {
            ID = SerializableGuid.CreateNew;
        }

        /// <summary>
        ///  Initializes a new instance of the <see cref="ItemSlot"/> class with the specified item and quantity.
        /// </summary>
        /// <param name="item"></param>
        /// <param name="quantity"></param>
        public ItemSlot(ItemObject item, int quantity)
        {
            ID = SerializableGuid.CreateNew;
            Item = item;
            Quantity = quantity;
        }
    }
}