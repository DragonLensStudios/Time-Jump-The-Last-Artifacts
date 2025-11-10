using PXE.Core.Enums;
using PXE.Core.Interfaces;
using PXE.Core.Inventory.Items;
using PXE.Core.Inventory.Managers;
using PXE.Core.SerializableTypes;
using UnityEngine;

namespace PXE.Core.Inventory.Data
{
    [System.Serializable]
    public class ItemData : IID
    {
        [field: Tooltip("The id of the item.")]
        [field: SerializeField] public virtual SerializableGuid ID { get; set; }

        [field: SerializeField] public bool IsManualID { get; set; }

        [field: Tooltip("The name of the item.")]
        [field: SerializeField] public virtual string Name { get; set; }
        
        [field: Tooltip("The description of the item.")]
        [field: SerializeField] public virtual string Description { get; set; }
        
        [field: Tooltip("The type of the item.")]
        [field: SerializeField] public virtual ItemType Type { get; set; }
        
        [field: Tooltip("The cost of the item.")]
        [field: SerializeField] public virtual StandardCurrency Cost { get; set; }
        
        [field: Tooltip("The sell value of the item.")]
        [field: SerializeField] public virtual StandardCurrency SellValue { get; set; }
        
        [field: Tooltip("The buy value of the item.")]
        [field: SerializeField] public virtual StandardCurrency BuyValue { get; set; }
        
        [field: Tooltip("The level requirement for the item.")]
        [field: SerializeField] public virtual int LevelRequirement { get; set; }
        
        [field: Tooltip("Is the item sellable?")]
        [field: SerializeField] public virtual bool Sellable { get; set; }
        
        [field: Tooltip("Is the item buyable?")]
        [field: SerializeField] public virtual bool Buyable { get; set; }
        
        [field: Tooltip("Is the item usable?")]
        [field: SerializeField] public virtual bool Usable { get; set; }
        
        [field: Tooltip("Is the item consumable?")]
        [field: SerializeField] public virtual bool Consumable { get; set; }
        
        [field: Tooltip("Is the item equipable?")]
        [field: SerializeField] public virtual bool Equipable { get; set; }
        
        [field: Tooltip("Is the item droppable?")]
        [field: SerializeField] public virtual bool Droppable { get; set; }
        
        [field: Tooltip("Is the item destroyable?")]
        [field: SerializeField] public virtual bool Destroyable { get; set; }
        
        [field: Tooltip("Is the item pickupable?")]
        [field: SerializeField] public virtual bool PickupAble { get; set; }
        
        [field: Tooltip("Is the item a quest item?")]
        [field: SerializeField] public virtual bool QuestItem { get; set; }
        
        [field: Tooltip("Is the item a key item?")]
        [field: SerializeField] public virtual bool KeyItem { get; set; } 
        
        [field: Tooltip("Is the item unique?")]
        [field: SerializeField] public virtual bool Unique { get; set; }
        
        [field: Tooltip("The weight of the item.")]
        [field: SerializeField] public virtual int Weight { get; set; }
        
        [field: Tooltip("does the item have durability?")]
        [field: SerializeField] public virtual bool HasDurability { get; set; }
        
        [field: Tooltip("The durability of the item.")]
        [field: SerializeField] public virtual int Durability { get; set; }
        
        [field: Tooltip("The maximum durability of the item.")]
        [field: SerializeField] public virtual int MaxDurability { get; set; }
        
        [field: Tooltip("Is the item stackable?")]
        [field: SerializeField] public virtual bool IsStackable { get; set; }
        
        [field: Tooltip("The maximum stack size for the item.")]
        [field: SerializeField] public virtual int MaxStack { get; set; }
        
        [field: Tooltip("The Rarity of the item.")]
        [field: SerializeField] public virtual RarityType Rarity { get; set; }
        
        [field: Tooltip("The path of the item icon.")]
        [field: SerializeField] public virtual string ItemIconPath { get; set; }
        
        [field: Tooltip("The spritesheet path of the item.")]
        [field: SerializeField] public virtual string ItemSpritesheetPath { get; set; }

        public ItemData()
        {
            
        }
        
        /// <summary>
        /// This constructor saves an <see cref="ItemObject"/>.
        /// </summary>
        /// <param name="item"></param>
        public ItemData(ItemObject item)
        {
            ID = item.ID;
            Name = item.Name;
            Description = item.Description;
            Type = item.Type;
            Cost = item.Cost;
            SellValue = item.SellValue;
            BuyValue = item.BuyValue;
            LevelRequirement = item.LevelRequirement;
            Sellable = item.Sellable;
            Buyable = item.Buyable;
            Usable = item.Usable;
            Consumable = item.Consumable;
            Equipable = item.Equipable;
            Droppable = item.Droppable;
            Destroyable = item.Destroyable;
            PickupAble = item.PickupAble;
            QuestItem = item.QuestItem;
            KeyItem = item.KeyItem;
            Unique = item.Unique;
            Weight = item.Weight;
            HasDurability = item.HasDurability;
            Durability = item.Durability;
            MaxDurability = item.MaxDurability;
            IsStackable = item.IsStackable;
            MaxStack = item.MaxStack;
            Rarity = item.Rarity;
            ItemIconPath = item.ItemIconPath;
            ItemSpritesheetPath = item.ItemSpritesheetPath;
        }
        
        /// <summary>
        /// This method returns an <see cref="ItemObject"/> on load.
        /// </summary>
        /// <returns><see cref="ItemObject"/></returns>
        public virtual ItemObject GetItemFromDatabase()
        {
            //TODO: Remove dependency on ItemDatabaseManager
            ItemDatabaseManager.Instance.ItemDatabase.Items.TryGetValue(ID, out var item);
            if (item == null) return null;
            var sprite = item.GetItemSprite();
            item.ItemIcon = sprite != null ? sprite : null;
            return item;
        }
    }
}