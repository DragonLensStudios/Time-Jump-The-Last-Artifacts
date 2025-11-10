using PXE.Core.Enums;
using PXE.Core.Interfaces;
using PXE.Core.Inventory.Items;
using PXE.Core.SerializableTypes;
using UnityEngine;

namespace PXE.Core.Inventory.Interfaces
{
    public interface IItemObject : IID
    {
        string Description { get; set; }
        ItemType Type { get; set; }
        StandardCurrency Cost { get; set; }
        StandardCurrency SellValue { get; set; }
        StandardCurrency BuyValue { get; set; }
        int LevelRequirement { get; set; }
        bool Sellable { get; set; }
        bool Buyable { get; set; }
        bool Usable { get; set; }
        bool Consumable { get; set; }
        bool Equipable { get; set; }
        bool Droppable { get; set; }
        bool Destroyable { get; set; }
        bool PickupAble { get; set; }
        bool QuestItem { get; set; }
        bool KeyItem { get; set; }
        bool Unique { get; set; }
        int Weight { get; set; }
        bool HasDurability { get; set; }
        int Durability { get; set; }
        int MaxDurability { get; set; }
        bool IsStackable { get; set; }
        int MaxStack { get; set; }
        RarityType Rarity { get; set; }
        Sprite ItemIcon { get; set; }
        string ItemIconPath { get; set; }
        string ItemSpritesheetPath { get; set; }
        void Use(int qty = 1, params SerializableGuid[] targetIds);
        void Use(int qty = 1, params IActorData[] actors);
        void Drop(int qty = 1, params SerializableGuid[] targetIds);
        void Pickup(int qty = 1, params SerializableGuid[] targetIds);
        void Sell(int qty = 1, params SerializableGuid[] targetIds);
        void Buy(int qty = 1, params SerializableGuid[] targetIds);
        void Equip(int qty = 1, params SerializableGuid[] targetIds);
        void Unequip(EquipmentSlot slot, int qty = 1, params SerializableGuid[] targetIds);
        void Destroy(int qty = 1, params SerializableGuid[] targetIds);
        void Consume(int qty = 1, params SerializableGuid[] targetIds);
        Sprite GetItemSprite();
    }
}