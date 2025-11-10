using Newtonsoft.Json;
using PXE.Core.Enums;
using PXE.Core.Interfaces;
using PXE.Core.Inventory.Interfaces;
using PXE.Core.Inventory.Messaging.Messages;
using PXE.Core.Messaging;
using PXE.Core.ScriptableObjects;
using PXE.Core.SerializableTypes;
using UnityEngine;

namespace PXE.Core.Inventory.Items
{
    /// <summary>
    ///  This class represents the basic item.
    /// </summary>
    [CreateAssetMenu(fileName ="Basic Item", menuName ="PXE/Game/Items/Basic Item", order = 1)]
    public class ItemObject : ScriptableObjectController, IItemObject
    {
        
        [field: Tooltip("The description of the item.")]
        [field: SerializeField] public virtual string Description { get; set; }
        
        [field: Tooltip("The type of the item.")]
        [field: SerializeField] public virtual ItemType Type { get; set; }
        
        [Tooltip("The cost of the item.")]
        [field: SerializeReference] public virtual StandardCurrency Cost { get; set; }
        
        [Tooltip("The sell value of the item.")]
        [field: SerializeReference] public virtual StandardCurrency SellValue { get; set; }
        
        [Tooltip("The buy value of the item.")]
        [field: SerializeReference] public virtual StandardCurrency BuyValue { get; set; }
        
        [field: Tooltip("The level requirement for the item.")]
        [field: SerializeField] public virtual int LevelRequirement { get; set; } = 0;
        
        [field: Tooltip("Is the item sellable?")]
        [field: SerializeField] public virtual bool Sellable { get; set; } = true;
        
        [field: Tooltip("Is the item buyable?")]
        [field: SerializeField] public virtual bool Buyable { get; set; } = true;
        
        [field: Tooltip("Is the item usable?")]
        [field: SerializeField] public virtual bool Usable { get; set; } = true;
        
        [field: Tooltip("Is the item consumable?")]
        [field: SerializeField] public virtual bool Consumable { get; set; } = false;
        
        [field: Tooltip("Is the item equipable?")]
        [field: SerializeField] public virtual bool Equipable { get; set; } = false;
        
        [field: Tooltip("Is the item droppable?")]
        [field: SerializeField] public virtual bool Droppable { get; set; } = true;
        
        [field: Tooltip("Is the item destroyable?")]
        [field: SerializeField] public virtual bool Destroyable { get; set; } = true;
        
        [field: Tooltip("Is the item pickupable?")]
        [field: SerializeField] public virtual bool PickupAble { get; set; } = true;
        
        [field: Tooltip("Is the item a quest item?")]
        [field: SerializeField] public virtual bool QuestItem { get; set; } = false;
        
        [field: Tooltip("Is the item a key item?")]
        [field: SerializeField] public virtual bool KeyItem { get; set; } = false;
        
        [field: Tooltip("Is the item unique?")]
        [field: SerializeField] public virtual bool Unique { get; set; } = false;
        
        [field: Tooltip("The weight of the item.")]
        [field: SerializeField] public virtual int Weight { get; set; } = 1;
        
        [field: Tooltip("does the item have durability?")]
        [field: SerializeField] public virtual bool HasDurability { get; set; } = false;
        
        [field: Tooltip("The durability of the item.")]
        [field: SerializeField] public virtual int Durability { get; set; } = 100;
        
        [field: Tooltip("The maximum durability of the item.")]
        [field: SerializeField] public virtual int MaxDurability { get; set; } = 100;
        
        [field: Tooltip("Is the item stackable?")]
        [field: SerializeField] public virtual bool IsStackable { get; set; } = true;
        
        [field: Tooltip("The maximum stack size for the item.")]
        [field: SerializeField] public virtual int MaxStack { get; set; } = 999;
        
        [field: Tooltip("The Rarity of the item.")]
        [field: SerializeField] public virtual RarityType Rarity { get; set; } = RarityType.None;
        
        [field: Tooltip("The icon of the item.")]
        [field: SerializeField, JsonIgnore] public virtual Sprite ItemIcon { get; set; }
        
        [field: Tooltip("The path of the item icon.")]
        [field: SerializeField] public virtual string ItemIconPath { get; set; }
        
        [field: Tooltip("The spritesheet path of the item.")]
        [field: SerializeField] public virtual string ItemSpritesheetPath { get; set; }


        /// <summary>
        ///  This method handles the using of an item provided the target ids and quantity.
        /// </summary>
        /// <param name="qty"></param>
        /// <param name="targetIds"></param>
        public virtual void Use(int qty = 1, params SerializableGuid[] targetIds)
        {
            if(!Usable) return;
            foreach (var targetId in targetIds)
            {
                Debug.Log($"{Name} used on {targetId}");
                MessageSystem.MessageManager.SendImmediate(MessageChannels.Items, new ItemUseMessage(targetId, this, qty));
            }
        }
        
        /// <summary>
        ///  This method handles the using of an item provided the quantity and actors.
        /// </summary>
        /// <param name="qty"></param>
        /// <param name="actors"></param>
        public virtual void Use(int qty = 1, params IActorData[] actors)
        {
            if(!Usable) return;
            foreach (var actor in actors)
            {
                Debug.Log($"{Name} used on {actor.Name}");
                MessageSystem.MessageManager.SendImmediate(MessageChannels.Items, new ItemUseMessage(actor.ID, this, qty));
            }
        }
        
        /// <summary>
        ///  This method handles the dropping of an item provided the quantity and target ids.
        /// </summary>
        /// <param name="qty"></param>
        /// <param name="targetIds"></param>
        public virtual void Drop(int qty = 1, params SerializableGuid[] targetIds)
        {
            if(!Droppable) return;
            foreach (var targetId in targetIds)
            {
                MessageSystem.MessageManager.SendImmediate(MessageChannels.Items, new ItemDropMessage(targetId, this, qty));
            }
        }
        
        /// <summary>
        ///  This method handles the picking up of an item provided the quantity and target ids.
        /// </summary>
        /// <param name="qty"></param>
        /// <param name="targetIds"></param>
        public virtual void Pickup(int qty = 1, params SerializableGuid[] targetIds)
        {
            if(!PickupAble) return;
            foreach (var targetId in targetIds)
            {
                MessageSystem.MessageManager.SendImmediate(MessageChannels.Items, new ItemPickupMessage(targetId, this, qty));
            }
        }
        
        /// <summary>
        ///  This method handles the selling of an item provided the quantity and target ids.
        /// </summary>
        /// <param name="qty"></param>
        /// <param name="targetIds"></param>
        public virtual void Sell(int qty = 1, params SerializableGuid[] targetIds)
        {
            if(!Sellable) return;
            foreach (var targetId in targetIds)
            {
                MessageSystem.MessageManager.SendImmediate(MessageChannels.Items, new ItemSellMessage(targetId, this, qty));
            }
        }
        
        /// <summary>
        ///  This method handles the buying of an item provided the quantity and target ids.
        /// </summary>
        /// <param name="qty"></param>
        /// <param name="targetIds"></param>
        public virtual void Buy(int qty = 1, params SerializableGuid[] targetIds)
        {
            if(!Buyable) return;
            foreach (var targetId in targetIds)
            {
                MessageSystem.MessageManager.SendImmediate(MessageChannels.Items, new ItemBuyMessage(targetId, this, qty));
            }
        }
        
        /// <summary>
        ///  This method handles the equipping of an item provided the quantity and target ids.
        /// </summary>
        /// <param name="qty"></param>
        /// <param name="targetIds"></param>
        public virtual void Equip(int qty = 1, params SerializableGuid[] targetIds)
        {
            if(!Equipable) return;
            foreach (var targetId in targetIds)
            {
                MessageSystem.MessageManager.SendImmediate(MessageChannels.Items, new ItemEquipMessage(targetId, this, qty));
            }
        }
        
        /// <summary>
        ///  This method handles the unequipping of an item provided the slot, quantity and target ids.
        /// </summary>
        /// <param name="slot"></param>
        /// <param name="qty"></param>
        /// <param name="targetIds"></param>
        public virtual void Unequip(EquipmentSlot slot, int qty = 1, params SerializableGuid[] targetIds)
        {
            if(!Equipable) return;
            foreach (var targetId in targetIds)
            {
                MessageSystem.MessageManager.SendImmediate(MessageChannels.Items, new ItemUnequipMessage(targetId, slot,this, qty));
            }
        }
        
        /// <summary>
        ///  This method handles the destroying of an item provided the quantity and target ids.
        /// </summary>
        /// <param name="qty"></param>
        /// <param name="targetIds"></param>
        public virtual void Destroy(int qty = 1, params SerializableGuid[] targetIds)
        {
            if(!Destroyable) return;
            foreach (var targetId in targetIds)
            {
                MessageSystem.MessageManager.SendImmediate(MessageChannels.Items, new ItemDestroyMessage(targetId, this, qty));
            }
        }
        
        /// <summary>
        ///  This method handles the consuming of an item provided the quantity and target ids.
        /// </summary>
        /// <param name="qty"></param>
        /// <param name="targetIds"></param>
        public virtual void Consume(int qty = 1, params SerializableGuid[] targetIds)
        {
            if(!Consumable) return;
            foreach (var targetId in targetIds)
            {
                MessageSystem.MessageManager.SendImmediate(MessageChannels.Items, new ItemConsumeMessage(targetId, this, qty));
            }
        }
        
        /// <summary>
        ///  This method handles getting the item sprite based ont he spritesheet path and icon path.
        /// </summary>
        /// <returns></returns>
        public virtual Sprite GetItemSprite()
        {
            if (string.IsNullOrEmpty(ItemSpritesheetPath)) return null;
            
            var sprites = Resources.LoadAll<Sprite>(ItemSpritesheetPath);
            
            foreach (var sprite in sprites)
            {
                if (ItemIconPath.Contains(sprite.name))
                {
                    return sprite;
                }
            }
            return null;
        }
    }


}
