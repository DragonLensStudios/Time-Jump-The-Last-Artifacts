using System.Collections.Generic;
using PXE.Core.Interfaces;
using PXE.Core.Inventory.Data;
using PXE.Core.Inventory.Items;
using PXE.Core.Messaging;
using PXE.Core.SerializableTypes;

namespace PXE.Core.Inventory.Interfaces
{
    public interface IInventoryObject : IObject
    {
        List<ItemSlot> Items { get; set; }
        StandardCurrency Currency { get; set; }
        int MaxSlots { get; set; }
        bool UseWeight { get; set; }
        int MaxWeight { get; set; }
        int CurrentWeight { get; set; }
        bool UseStartingSlots { get; set; }
        int StartingSlots { get; set; }
        bool UseMaxSlots { get; set; }
        bool UseMaxWeight { get; set; }
        void InventoryMoveMessageHandler(MessageSystem.IMessageEnvelope message);
        bool MoveItem(int fromIndex, int toIndex, int quantity);
        InventoryData Save();
        void Load(InventoryData inventoryData);
        void Initialize();
        ItemSlot this[SerializableGuid id] { get; }
        ItemSlot this[string itemName] { get; }
        bool AddItem(ItemObject itemToAdd, int qty = 1, int slotIndex = -1);
        bool AddItem(ItemSlot item);
        bool AddItems(List<ItemObject> items, List<int> quantities);
        bool AddItems(List<ItemSlot> items);
        bool RemoveItem(ItemObject itemToRemove, int qty = 1);
        bool RemoveItem(ItemSlot item);
        bool RemoveItems(List<ItemSlot> items);
        bool RemoveItems(List<ItemObject> items, List<int> quantities);
        bool EditItem(ItemObject newItem, int newQty);
        bool EditItem(ItemSlot newItem);
        bool EditItems(List<ItemSlot> newItems);
        bool EditItems(List<ItemObject> newItems, List<int> newQuantities);
        int GetTotalQuantity(ItemObject item);
        int GetTotalQuantity(ItemSlot item);
        int GetTotalQuantity(List<ItemObject> items);
        int GetTotalQuantity(List<ItemSlot> items);
        int GetTotalQuantity(List<ItemObject> items, List<int> quantities);
        bool ContainsItem(ItemObject item);
        bool ContainsItem(ItemObject item, int qty);
        bool ContainsItem(ItemSlot item);
        bool ContainsItem(ItemSlot item, int qty);
        bool ContainsItem(List<ItemObject> items);
        bool ContainsItem(List<ItemObject> items, List<int> quantities);
        bool ContainsItem(List<ItemSlot> items);
        bool ContainsItems(List<ItemObject> items);
        bool ContainsItems(List<ItemObject> items, List<int> quantities);
        bool ContainsItems(List<ItemSlot> items);
        void InventoryChangedMessageHandler(MessageSystem.IMessageEnvelope message);
        void InventoryModifyMessageHandler(MessageSystem.IMessageEnvelope message);
    }
}