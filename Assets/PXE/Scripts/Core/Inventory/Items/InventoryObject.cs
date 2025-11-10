using System;
using System.Collections.Generic;
using System.Linq;
using PXE.Core.Enums;
using PXE.Core.Inventory.Data;
using PXE.Core.Inventory.Interfaces;
using PXE.Core.Inventory.Messaging.Messages;
using PXE.Core.Messaging;
using PXE.Core.Player.Managers;
using PXE.Core.ScriptableObjects;
using PXE.Core.SerializableTypes;
using UnityEngine;

namespace PXE.Core.Inventory.Items
{
    /// <summary>
    ///  This class represents the inventory.
    /// </summary>
    [CreateAssetMenu(fileName ="Inventory", menuName ="PXE/Game/Items/Inventory", order = 0)]
    public class InventoryObject : ScriptableObjectController, IInventoryObject
    {
        [field: Tooltip("The items in the inventory.")]
        [field: SerializeReference] public virtual List<ItemSlot> Items { get; set; } = new ();
        
        [field: Tooltip("The currency in the inventory.")]
        [field: SerializeReference] public virtual StandardCurrency Currency { get; set; }
        
        [field: Tooltip("The maximum number of slots in the inventory.")]
        [field: SerializeField] public virtual int MaxSlots { get; set; } = 100;
        
        [field: Tooltip("Whether the inventory uses weight.")]
        [field: SerializeField] public virtual bool UseWeight { get; set; } = false;
        
        [field: Tooltip("The maximum weight of the inventory.")]
        [field: SerializeField] public virtual int MaxWeight { get; set; } = 500;
        
        [field: Tooltip("The current weight of the inventory.")]
        [field: SerializeField] public virtual int CurrentWeight { get; set; } = 0;
        
        [field: Tooltip("Whether the inventory uses starting slots.")]
        [field: SerializeField] public bool UseStartingSlots { get; set; } = true;
        
        [field: Tooltip("The starting number of slots in the inventory.")]
        [field: SerializeField] public int StartingSlots { get; set; } = 50;
        
        [field: Tooltip("Whether the inventory uses the maximum number of slots.")]
        [field: SerializeField] public bool UseMaxSlots { get; set; } = true;
        
        [field: Tooltip("Whether the inventory uses the maximum weight.")]
        [field: SerializeField] public bool UseMaxWeight { get; set; } = true;

        public virtual void Awake()
        {
            Initialize();
        }

        /// <summary>
        ///  This method registers the inventory for the InventoryChangedMessage and InventoryModifyMessage.
        /// </summary>
        public virtual void OnEnable()
        {
            MessageSystem.MessageManager.RegisterForChannel<InventoryChangedMessage>(MessageChannels.Items, InventoryChangedMessageHandler);
            MessageSystem.MessageManager.RegisterForChannel<InventoryModifyMessage>(MessageChannels.Items, InventoryModifyMessageHandler);
            MessageSystem.MessageManager.RegisterForChannel<InventoryMoveMessage>(MessageChannels.Items, InventoryMoveMessageHandler);
        }
        
        /// <summary>
        ///  This method unregisters the inventory for the InventoryChangedMessage and InventoryModifyMessage.
        /// </summary>
        public virtual void OnDisable()
        {
            MessageSystem.MessageManager.UnregisterForChannel<InventoryChangedMessage>(MessageChannels.Items, InventoryChangedMessageHandler);
            MessageSystem.MessageManager.UnregisterForChannel<InventoryModifyMessage>(MessageChannels.Items, InventoryModifyMessageHandler);
            MessageSystem.MessageManager.UnregisterForChannel<InventoryMoveMessage>(MessageChannels.Items, InventoryMoveMessageHandler);
        }

        public virtual void InventoryMoveMessageHandler(MessageSystem.IMessageEnvelope message)
        {
            if(!message.Message<InventoryMoveMessage>().HasValue) return;
            var data = message.Message<InventoryMoveMessage>().GetValueOrDefault();
            if(!data.InventoryID.Equals(ID)) return;
            if(data.ItemObject == null) return;
            if(data.Amount <= 0) return;
            if(data.FromIndex < 0) return;
            if(data.FromIndex >= Items.Count) return;
            if(data.ToIndex < 0) return;
            if(data.ToIndex >= Items.Count) return;
            MoveItem(data.FromIndex, data.ToIndex, data.Amount);
        }
        
        /// <summary>
        /// Moves an item from one slot to another, handling stacking and swapping.
        /// </summary>
        /// <param name="fromIndex">The index of the source slot.</param>
        /// <param name="toIndex">The index of the destination slot.</param>
        /// <param name="quantity">The quantity of the item to move.</param>
        /// <returns>True if the move was successful, false otherwise.</returns>
        public virtual bool MoveItem(int fromIndex, int toIndex, int quantity)
        {
            //TODO: Fix the logic for stacking items of the same type.
            // Validate indices
            if (fromIndex < 0 || fromIndex >= Items.Count || toIndex < 0 || toIndex >= Items.Count || fromIndex == toIndex)
                return false;
            
            var fromSlot = Items[fromIndex];
            var toSlot = Items[toIndex];
            
            // Validate source slot
            if (fromSlot == null || fromSlot.Item == null)
                return false;
            

            if (toSlot != null && toSlot.Item != null)
            {
                if (toSlot.Item.ID.Equals(fromSlot.Item.ID) && toSlot.Item.IsStackable)
                {
                    // Handle stacking if items are the same and stackable
                    int spaceInToSlot = toSlot.Item.MaxStack - toSlot.Quantity;
                    int transferQuantity = Math.Min(spaceInToSlot, Math.Min(quantity, fromSlot.Quantity));
                    toSlot.Quantity += transferQuantity;
                    fromSlot.Quantity -= transferQuantity;
                    if (fromSlot.Quantity == 0)
                        fromSlot.Item = null;
                }
                else
                {
                    // Handle swapping if items are different
                    (toSlot, fromSlot) = (fromSlot, toSlot);
                }
            }
            else
            {
                // If destination slot is empty, move the item there
                int transferQuantity = Math.Min(quantity, fromSlot.Item.MaxStack);
                toSlot = new ItemSlot { Item = fromSlot.Item, Quantity = transferQuantity };
                fromSlot.Quantity -= transferQuantity;
                if (fromSlot.Quantity == 0)
                    fromSlot.Item = null;
            }

            // Assign the modified slots back to the Items list
            Items[toIndex] = toSlot;
            Items[fromIndex] = fromSlot;

            // Update inventory and send messages if needed
            MessageSystem.MessageManager.SendImmediate(MessageChannels.Items, new InventoryChangedMessage(ID, this));
            return true;
        }




        public virtual InventoryData Save()
        {
            var inventoryData = new InventoryData
            {
                ID = ID,
                Name = Name,
                Currency = Currency,
                MaxSlots = MaxSlots,
                UseWeight = UseWeight,
                MaxWeight = MaxWeight,
                CurrentWeight = CurrentWeight,
                UseStartingSlots = UseStartingSlots,
                StartingSlots = StartingSlots,
                UseMaxSlots = UseMaxSlots,
                UseMaxWeight = UseMaxWeight
            };

            if(Items == null) return inventoryData;
            if (Items.Count <= 0) return inventoryData;
            for (int i = 0; i < Items.Count; i++)
            {
                var item = Items[i];
                if(item == null) continue;
                ItemSlotData slotData = null;
                if (item.Item == null)
                {
                    slotData = new ItemSlotData
                    {
                        Item = new ItemData(),
                        Quantity = 0
                    };
                }
                else
                {
                    slotData = new ItemSlotData(item);
                }
                inventoryData.Items.Add(slotData);
            }
            return inventoryData;
        }
        
        /// <summary>
        /// This method is used the load the inventory from the provided Item Database
        /// </summary>
        public virtual void Load(InventoryData inventoryData)
        {
            if (inventoryData == null) return;
            ID = inventoryData.ID;
            Name = inventoryData.Name;
            Currency = inventoryData.Currency;
            MaxSlots = inventoryData.MaxSlots;
            UseWeight = inventoryData.UseWeight;
            MaxWeight = inventoryData.MaxWeight;
            CurrentWeight = inventoryData.CurrentWeight;
            UseStartingSlots = inventoryData.UseStartingSlots;
            StartingSlots = inventoryData.StartingSlots;
            UseMaxSlots = inventoryData.UseMaxSlots;
            UseMaxWeight = inventoryData.UseMaxWeight;
            
            Items.Clear();
            for (int i = 0; i < inventoryData.Items.Count; i++)
            {
                var itemData = inventoryData.Items[i];
                if (itemData == null) continue;
                ItemObject item = null;
                if (itemData.Item.ID == null)
                {
                    item = CreateInstance<ItemObject>();
                }
                else
                {
                    item = itemData.Item.GetItemFromDatabase();
                }
                if(item == null) continue;
                AddItem(item, itemData.Quantity,i);
            }
            
        }
        
        public virtual void Initialize()
        {
            Items.Clear();
            if (UseStartingSlots)
            {
                for (int i = 0; i < StartingSlots; i++)
                {
                    Items.Add(new ItemSlot());
                }
            }
        }
        
        /// <summary>
        /// Indexer for accessing items by ID
        /// </summary>
        /// <param name="id"></param>
        public virtual ItemSlot this[SerializableGuid id]
        {
            get
            {
                return Items.FirstOrDefault(slot => slot.Item.ID.Equals(id));
            }
        }
        
        /// <summary>
        /// Indexer for accessing items by Name
        /// </summary>
        /// <param name="itemName"></param>
        public virtual ItemSlot this[string itemName]
        {
            get
            {
                return Items.FirstOrDefault(slot => slot.Item.Name.Equals(itemName));
            }
        }
        
        /// <summary>
        ///  This method adds the item to the inventory given the item and quantity.
        /// </summary>
        /// <param name="itemToAdd"></param>
        /// <param name="qty">defaults to 1</param>
        /// <param name="slotIndex">Defaults to -1 to find next available slot</param>
        /// <returns></returns>
        public virtual bool AddItem(ItemObject itemToAdd, int qty = 1, int slotIndex = -1)
        {
            if(Items.Count <= 0) Initialize();
            // Check for existing item stacks and an empty slot
            var foundItemStacks = Items.Where(x => x.Item != null && x.Item.ID.Equals(itemToAdd.ID) && x.Item.IsStackable)
                .OrderBy(x => x.Quantity);
            var nextEmptySlot = Items.FirstOrDefault(slot => slot.Item == null);
            if (slotIndex >= 0)
            {
                if (slotIndex >= Items.Count) return false;
                nextEmptySlot = Items[slotIndex];
            }
            var countNonEmptySlots = Items.Count(x => x.Item != null);

            // Check if adding the item would exceed the weight limit
            if (UseWeight && CurrentWeight + (itemToAdd.Weight * qty) > MaxWeight) return false;

            // Try to add to existing stacks first
            foreach (var stack in foundItemStacks)
            {
                int spaceInStack = stack.Item.MaxStack - stack.Quantity;
                if (qty <= spaceInStack)
                {
                    stack.Quantity += qty;
                    MessageSystem.MessageManager.SendImmediate(MessageChannels.Items, new InventoryChangedMessage(ID, this));
                    return true;
                }
                else
                {
                    stack.Quantity = stack.Item.MaxStack;
                    qty -= spaceInStack;
                }
            }

            // If there's remaining quantity, try to add it to an empty slot
            if (qty > 0)
            {
                if (countNonEmptySlots < MaxSlots && nextEmptySlot != null)
                {
                    nextEmptySlot.Item = itemToAdd;
                    nextEmptySlot.Quantity = Math.Min(qty, itemToAdd.MaxStack);
                    qty -= nextEmptySlot.Quantity;
                }
            }

            // If there's still remaining quantity and no space, drop the remaining items
            if (qty > 0)
            {
                MessageSystem.MessageManager.SendImmediate(MessageChannels.Items, new InventoryChangedMessage(ID, this));
                if(PlayerManager.Instance == null) return false;
                //TODO: Decouple this from the PlayerManager
                itemToAdd.Drop(qty, PlayerManager.Instance.Player.ID);
                return false;
            }

            MessageSystem.MessageManager.SendImmediate(MessageChannels.Items, new InventoryChangedMessage(ID, this));
            return true;
        }
        
        /// <summary>
        ///  This method adds the item to the inventory given the item slot.
        /// </summary>
        /// <param name="item"></param>
        public virtual bool AddItem(ItemSlot item)
        {
            return AddItem(item.Item, item.Quantity);
        }
        
        /// <summary>
        ///  This method adds the items to the inventory given the items and quantities.
        /// </summary>
        /// <param name="items"></param>
        /// <param name="quantities"></param>
        public virtual bool AddItems(List<ItemObject> items, List<int> quantities)
        {
            if (items.Count != quantities.Count) return false;
            return !items.Where((t, i) => !AddItem(t, quantities[i])).Any();
        }
        
        /// <summary>
        ///  This method adds the items to the inventory given the item slots provided.
        /// </summary>
        /// <param name="items"></param>
        public virtual bool AddItems(List<ItemSlot> items)
        {
            return items.All(item => AddItem(item.Item, item.Quantity));
        }

        /// <summary>
        ///  This method removes the item from the inventory given the item and quantity.
        /// </summary>
        /// <param name="itemToRemove"></param>
        /// <param name="qty"></param>
        /// <returns></returns>
        public virtual bool RemoveItem(ItemObject itemToRemove, int qty = 1)
        {
            // Getting all the item stacks matching the item to remove, ordered by quantity ascending.
            var itemStacks = Items.Where(slot => slot.Item != null && slot.Item.ID.Equals(itemToRemove.ID))
                .OrderBy(x => x.Quantity).ToList();

            // If there's no item stack with the desired item, then return false.
            if (!itemStacks.Any()) return false;

            int qtyToRemove = qty;

            foreach (var stack in itemStacks)
            {
                if (qtyToRemove <= 0) break;  // Stop when we've removed enough items.

                if (stack.Quantity <= qtyToRemove)
                {
                    // If the current stack has less or equal quantity than we need to remove, deduct the entire stack.
                    qtyToRemove -= stack.Quantity;
                    stack.Item = null;
                    stack.Quantity = 0;
                }
                else
                {
                    // If the current stack has more than we need to remove, deduct the necessary amount and break.
                    stack.Quantity -= qtyToRemove;
                    qtyToRemove = 0;
                }
            }

            // If after processing all stacks, there's still some qty to remove, then we couldn't satisfy the requirement.
            if (qtyToRemove > 0) return false;

            // Notify about the inventory change.
            MessageSystem.MessageManager.SendImmediate(MessageChannels.Items, new InventoryChangedMessage(ID, this));
            return true;
        }
        
        /// <summary>
        /// This method removes the item from the inventory given the item slot
       
        public virtual bool RemoveItem(ItemSlot item)
        {
            return RemoveItem(item.Item, item.Quantity);
        }
        
        
        /// <summary>
        ///  This method removes the item from the inventory given the item slots.
        /// </summary>
        /// <param name="items"></param>
        /// <returns></returns>
        public virtual bool RemoveItems(List<ItemSlot> items)
        {
            if (items.Any(item => !RemoveItem(item.Item, item.Quantity)))
            {
                return false;
            }
            MessageSystem.MessageManager.SendImmediate(MessageChannels.Items, new InventoryChangedMessage(ID, this));
            return true;
        }
        
        /// <summary>
        ///  This method removes the items from the inventory given the items and quantities.
        /// </summary>
        /// <param name="items"></param>
        /// <param name="quantities"></param>
        /// <returns></returns>
        public virtual bool RemoveItems(List<ItemObject> items, List<int> quantities)
        {
            if (items.Count != quantities.Count) return false;
            if (items.Where((t, i) => !RemoveItem(t, quantities[i])).Any())
            {
                return false;
            }
            MessageSystem.MessageManager.SendImmediate(MessageChannels.Items, new InventoryChangedMessage(ID, this));
            return true;
        }

        /// <summary>
        ///  This method edits the item in the inventory given the item and quantity.
        /// </summary>
        /// <param name="newItem"></param>
        /// <param name="newQty"></param>
        /// <returns></returns>
        public virtual bool EditItem(ItemObject newItem, int newQty)
        {
            var itemStacks = Items.Where(slot => slot.Item.ID.Equals(newItem.ID))
                .OrderByDescending(x => x.Quantity).ToList();

            if (!itemStacks.Any()) return false;
            
            // Positive quantity adjustments
            if (newQty > 0)
            {
                var primaryStack = itemStacks.First();
                primaryStack.Item = newItem;
        
                if (newQty <= primaryStack.Item.MaxStack)
                {
                    primaryStack.Quantity = newQty;
                }
                else
                {
                    primaryStack.Quantity = primaryStack.Item.MaxStack;
                    int remainingQty = newQty - primaryStack.Item.MaxStack;
            
                    // Here, you can add the remaining quantity to a new stack, or if there's logic for handling multiple stacks, distribute among them.
                    AddItem(newItem, remainingQty);
                }
            }
            // Zero or negative quantity adjustments
            else
            {
                int qtyToRemove = -newQty; // We want to remove the absolute value

                foreach (var stack in itemStacks)
                {
                    if (qtyToRemove <= 0) break; // We've removed enough items

                    if (stack.Quantity <= qtyToRemove)
                    {
                        qtyToRemove -= stack.Quantity;
                        stack.Item = null;
                        stack.Quantity = 0;
                    }
                    else
                    {
                        stack.Quantity -= qtyToRemove;
                        qtyToRemove = 0;
                    }
                }

                if (qtyToRemove > 0) return false; // Unable to remove the required amount
            }

            MessageSystem.MessageManager.SendImmediate(MessageChannels.Items, new InventoryChangedMessage(ID, this));
            return true;
        }
        
        /// <summary>
        ///  This method edits the item in the inventory given the item slot.
        /// </summary>
        /// <param name="newItem"></param>
        /// <returns></returns>
        public virtual bool EditItem(ItemSlot newItem)
        {
           return EditItem(newItem.Item, newItem.Quantity);
        }
        
        /// <summary>
        ///  This method edits the items in the inventory given the item slots.
        /// </summary>
        /// <param name="newItems"></param>
        /// <returns></returns>
        public virtual bool EditItems(List<ItemSlot> newItems)
        {
            if (newItems.Any(item => !EditItem(item)))
            {
                return false;
            }
            MessageSystem.MessageManager.SendImmediate(MessageChannels.Items, new InventoryChangedMessage(ID, this));
            return true;
        }
        
        /// <summary>
        ///  This method edits the items in the inventory given the items and quantities.
        /// </summary>
        /// <param name="newItems"></param>
        /// <param name="newQuantities"></param>
        /// <returns></returns>
        public virtual bool EditItems(List<ItemObject> newItems, List<int> newQuantities)
        {
            if (newItems.Count != newQuantities.Count) return false;
            
            if (newItems.Where((t, i) => !EditItem(t, newQuantities[i])).Any())
            {
                return false;
            }
            MessageSystem.MessageManager.SendImmediate(MessageChannels.Items, new InventoryChangedMessage(ID, this));
            return true;
        }

        /// <summary>
        ///  This method gets the total quantity of the item in the inventory given the item.
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public virtual int GetTotalQuantity(ItemObject item)
        {
            return Items.Where(slot => slot.Item.ID.Equals(item.ID)).Sum(slot => slot.Quantity);
        }
        
        /// <summary>
        ///  This method gets the total quantity of the item in the inventory given the item slot.
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public virtual int GetTotalQuantity(ItemSlot item)
        {
            return Items.Where(slot => slot.Item.ID.Equals(item.Item.ID)).Sum(slot => slot.Quantity);
        }
        
        /// <summary>
        ///  This method gets the total quantity of the items in the inventory given the items.
        /// </summary>
        /// <param name="items"></param>
        /// <returns></returns>
        public virtual int GetTotalQuantity(List<ItemObject> items)
        {
            return items.Sum(item => Items.Where(slot => slot.Item.ID.Equals(item.ID)).Sum(slot => slot.Quantity));
        }
        
        /// <summary>
        ///  This method gets the total quantity of the items in the inventory given the item slots.
        /// </summary>
        /// <param name="items"></param>
        /// <returns></returns>
        public virtual int GetTotalQuantity(List<ItemSlot> items)
        {
            return items.Sum(item => Items.Where(slot => slot.Item.ID.Equals(item.Item.ID)).Sum(slot => slot.Quantity));
        }
        
        /// <summary>
        ///  This method gets the total quantity of the items in the inventory given the items and quantities.
        /// </summary>
        /// <param name="items"></param>
        /// <param name="quantities"></param>
        /// <returns></returns>
        public virtual int GetTotalQuantity(List<ItemObject> items, List<int> quantities)
        {
            if (items.Count != quantities.Count) return 0;
            var total = 0;
            for (int i = 0; i < items.Count; i++)
            {
                total += Items.Where(slot => slot.Item.ID.Equals(items[i].ID)).Sum(slot => slot.Quantity);
            }
            return total;
        }

        /// <summary>
        ///  This method returns whether the inventory contains the item given the item.
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public virtual bool ContainsItem(ItemObject item)
        {
            if (item == null) return false;
            if(Items.All(x=> x.Item == null)) return false;
            return Items.Any(slot => slot.Item.ID.Equals(item.ID));
        }
        
        /// <summary>
        ///  This method returns whether the inventory contains the item given the item and quantity.
        /// </summary>
        /// <param name="item"></param>
        /// <param name="qty"></param>
        /// <returns></returns>
        public virtual bool ContainsItem(ItemObject item, int qty)
        {
            if (item == null) return false;
            if(Items.All(x=> x.Item == null)) return false;
            return Items.Any(slot => slot.Item.ID.Equals(item.ID) && slot.Quantity >= qty);
        }
        
        /// <summary>
        ///  This method returns whether the inventory contains the item given the item slot.
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public virtual bool ContainsItem(ItemSlot item)
        {
            if (item == null) return false;
            if(Items.All(x=> x.Item == null)) return false;
            return Items.Any(slot => slot.Item.ID.Equals(item.Item.ID) && slot.Quantity >= item.Quantity);
        }
        
        /// <summary>
        ///  This method returns whether the inventory contains the item given the item slot and quantity.
        /// </summary>
        /// <param name="item"></param>
        /// <param name="qty"></param>
        /// <returns></returns>
        public virtual bool ContainsItem(ItemSlot item, int qty)
        {
            if (item == null || item.Item == null) return false;
            if(Items.All(x=> x.Item == null)) return false;

            return Items.Any(slot => slot.Item.ID.Equals(item.Item.ID) && slot.Quantity >= qty);
        }
        
        /// <summary>
        ///  This method returns whether the inventory contains the items given the items.
        /// </summary>
        /// <param name="items"></param>
        /// <returns></returns>
        public virtual bool ContainsItem(List<ItemObject> items)
        {
            if(Items.All(x=> x.Item == null)) return false;
            return items.All(ContainsItem);
        }
        
        /// <summary>
        ///  This method returns whether the inventory contains the items given the items and quantities.
        /// </summary>
        /// <param name="items"></param>
        /// <param name="quantities"></param>
        /// <returns></returns>
        public virtual bool ContainsItem(List<ItemObject> items, List<int> quantities)
        {
            if (items.Count != quantities.Count) return false;
            if(Items.All(x=> x.Item == null)) return false;
            for (int i = 0; i < items.Count; i++)
            {
                if (!ContainsItem(items[i], quantities[i])) return false;
            }
            return true;
        }
        
        /// <summary>
        ///  This method returns whether the inventory contains the items given the item slots.
        /// </summary>
        /// <param name="items"></param>
        /// <returns></returns>
        public virtual bool ContainsItem(List<ItemSlot> items)
        {
            if(Items.All(x=> x.Item == null)) return false;
            return items.All(slot => ContainsItem(slot.Item, slot.Quantity));
        }
        
        /// <summary>
        ///  This method returns whether the inventory contains the items given the item objects.
        /// </summary>
        /// <param name="items"></param>
        /// <returns></returns>
        public virtual bool ContainsItems (List<ItemObject> items)
        {
            if(Items.All(x=> x.Item == null)) return false;
            return items.All(ContainsItem);
        }
        
        /// <summary>
        ///  This method returns whether the inventory contains the items given the item objects and quantities.
        /// </summary>
        /// <param name="items"></param>
        /// <param name="quantities"></param>
        /// <returns></returns>
        public virtual bool ContainsItems (List<ItemObject> items, List<int> quantities)
        {
            if (items.Count != quantities.Count) return false;
            if(Items.All(x=> x.Item == null)) return false;
            for (int i = 0; i < items.Count; i++)
            {
                if (!ContainsItem(items[i], quantities[i])) return false;
            }
            return true;
        }
        
        /// <summary>
        ///  This method returns whether the inventory contains the items given the item slots.
        /// </summary>
        /// <param name="items"></param>
        /// <returns></returns>
        public virtual bool ContainsItems (List<ItemSlot> items)
        {
            if(Items.All(x=> x.Item == null)) return false;
            return items.All(slot => ContainsItem(slot.Item, slot.Quantity));
        }
        
        /// <summary>
        ///  This method checks sets the Items to the provided items in the message.
        /// </summary>
        /// <param name="message"></param>
        public virtual void InventoryChangedMessageHandler(MessageSystem.IMessageEnvelope message)
        {
            if(!message.Message<InventoryChangedMessage>().HasValue) return;
            var data = message.Message<InventoryChangedMessage>().GetValueOrDefault();
            if(!data.ID.Equals(ID)) return;
            Items = data.Inventory.Items;
        }
        
        /// <summary>
        ///  This method checks the operation in the message and performs the appropriate action such as Add, Remove, or Edit.
        /// </summary>
        /// <param name="message"></param>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public virtual void InventoryModifyMessageHandler(MessageSystem.IMessageEnvelope message)
        {
            if(!message.Message<InventoryModifyMessage>().HasValue) return;
            var data = message.Message<InventoryModifyMessage>().GetValueOrDefault();
            if(!data.InventoryID.Equals(ID)) return;
            switch (data.Operation)
            {
                case InventoryModifyType.Add:
                    AddItem(data.Item, data.Quantity);
                    break;
                case InventoryModifyType.Remove:
                    RemoveItem(data.Item, data.Quantity);
                    break;
                case InventoryModifyType.Edit:
                    EditItem(data.Item, data.Quantity);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}