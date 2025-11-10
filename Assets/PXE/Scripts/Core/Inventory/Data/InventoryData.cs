using System.Collections.Generic;
using PXE.Core.Interfaces;
using PXE.Core.Inventory.Items;
using PXE.Core.SerializableTypes;
using UnityEngine;

namespace PXE.Core.Inventory.Data
{
    [System.Serializable]
    public class InventoryData : IID
    {
        [field: SerializeField] public virtual SerializableGuid ID { get; set; }
        [field: SerializeField] public bool IsManualID { get; set; }
        [field: SerializeField] public virtual string Name { get; set; }
        [field: SerializeField] public virtual List<ItemSlotData> Items { get; set; } = new();
        [field: SerializeField] public virtual StandardCurrency Currency { get; set; }
        [field: SerializeField] public virtual int MaxSlots { get; set; }
        [field: SerializeField] public virtual bool UseWeight { get; set; }
        [field: SerializeField] public virtual int MaxWeight { get; set; }
        [field: SerializeField] public virtual int CurrentWeight { get; set; }
        [field: SerializeField] public virtual bool UseStartingSlots { get; set; }
        [field: SerializeField] public virtual int StartingSlots { get; set; }
        [field: SerializeField] public virtual bool UseMaxSlots { get; set; }
        [field: SerializeField] public virtual bool UseMaxWeight { get; set; }
        
        public InventoryData()
        {
            
        }
        
        public InventoryData(InventoryObject inventory)
        {
            ID = inventory.ID;
            Name = inventory.Name;
            Currency = inventory.Currency;
            MaxSlots = inventory.MaxSlots;
            UseWeight = inventory.UseWeight;
            MaxWeight = inventory.MaxWeight;
            CurrentWeight = inventory.CurrentWeight;
            UseStartingSlots = inventory.UseStartingSlots;
            StartingSlots = inventory.StartingSlots;
            UseMaxSlots = inventory.UseMaxSlots;
            UseMaxWeight = inventory.UseMaxWeight;
            foreach (var itemSlot in inventory.Items)
            {
                Items.Add(new ItemSlotData(itemSlot));
            }
        }
    }
}