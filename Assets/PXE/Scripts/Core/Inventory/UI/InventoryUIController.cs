using System.Collections;
using System.Collections.Generic;
using System.Linq;
using PXE.Core.Enums;
using PXE.Core.Inventory.Items;
using PXE.Core.Inventory.Messaging.Messages;
using PXE.Core.Messaging;
using PXE.Core.Objects;
using PXE.Core.State_System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace PXE.Core.Inventory.UI
{
    public class InventoryUIController : ObjectController
    {
        [field: SerializeField] public virtual InventoryObject Inventory { get; set; }
        [field: SerializeField] public virtual bool UseStartingSlots { get; set; } = true;
        [field: SerializeField] public virtual bool UseMaxSlots { get; set; } = true;
        [field: SerializeField] public virtual bool UseMaxWeight { get; set; } = true;
        [field: SerializeField] public virtual int StartingSlots { get; set; } = 50;
        [field: SerializeField] public virtual List<ItemContainerUI> CurrentSlots { get; set; } = new ();
        [field: SerializeField] public virtual ItemContainerUI SelectedSlot { get; set; }
        [field: SerializeField] public virtual int MaxSlots { get; set; } = 100;
        [field: SerializeField] public virtual int MaxWeight { get; set; } = 500;
        [field: SerializeField] public virtual int CurrentWeight { get; set; } = 0;
        [field: SerializeField] public virtual ObjectController InventoryPanel { get; set; }
        [field: SerializeField] public virtual ObjectController ContentPanel { get; set; }
        [field: SerializeField] public virtual ItemActionPanelUI ActionsPanel { get; set; }
        [field: SerializeField] public virtual ObjectController InventorySlotPrefab { get; set; }
        [field: SerializeField] public virtual GridLayoutGroup Grid { get; set; }
        [field: SerializeField] public virtual InventoryState InventoryState { get; set; }
        [field: SerializeField] public virtual InputActionReference SubmitAction { get; set; }
        [field: SerializeField] public virtual InputActionReference ClickAction { get; set; }
        [field: SerializeField] public virtual InputActionReference CancelAction { get; set; }
        [field: SerializeField] public virtual bool IsItemSlotMoving { get; set; } = false;
        [field: SerializeField] public virtual ItemContainerUI MovingItemSlot { get; set; }
        [field: SerializeField] public virtual ItemContainerUI SplitItemSlot { get; set; }
        [field: SerializeField] public virtual bool HasItemJustMoved { get; set; } = false;
        [field: SerializeField] public virtual float HasItemJustMovedDelay { get; set; } = 0.1f;
        public override void Start()
        {
            base.Start();

            if (Grid == null)
            {
                Grid = ContentPanel.GetComponent<GridLayoutGroup>();
            }
        
        }
    
        public override void OnActive()
        {
            base.OnActive();
            SubmitAction.action.performed += SubmitActionOnperformed;
            ClickAction.action.performed += SubmitActionOnperformed;
            CancelAction.action.performed += CancelActionOnperformed;
            MessageSystem.MessageManager.RegisterForChannel<InventoryChangedMessage>(MessageChannels.Items, InventoryChangedMessageHandler);
            MessageSystem.MessageManager.RegisterForChannel<ItemSelectedMessage>(MessageChannels.Items, ItemSelectedMessageHandler);
            MessageSystem.MessageManager.RegisterForChannel<ItemInteractMessage>(MessageChannels.UI, ItemInteractMessageHandler);
            MessageSystem.MessageManager.RegisterForChannel<ItemMoveMessage>(MessageChannels.Items, ItemMoveMessageHandler);
            MessageSystem.MessageManager.RegisterForChannel<ItemSplitMessage>(MessageChannels.Items, ItemSplitMessageHandler);
            ClearAndUpdateInventory();
            EventSystem.current.SetSelectedGameObject(null);
            EventSystem.current.SetSelectedGameObject(CurrentSlots[0].gameObject);
        }

        public override void OnInactive()
        {
            base.OnInactive();
            SubmitAction.action.performed -= SubmitActionOnperformed;
            ClickAction.action.performed -= SubmitActionOnperformed;
            CancelAction.action.performed -= CancelActionOnperformed;
            MessageSystem.MessageManager.UnregisterForChannel<InventoryChangedMessage>(MessageChannels.Items, InventoryChangedMessageHandler);
            MessageSystem.MessageManager.UnregisterForChannel<ItemSelectedMessage>(MessageChannels.Items, ItemSelectedMessageHandler);
            MessageSystem.MessageManager.UnregisterForChannel<ItemInteractMessage>(MessageChannels.UI, ItemInteractMessageHandler);
            MessageSystem.MessageManager.UnregisterForChannel<ItemMoveMessage>(MessageChannels.Items, ItemMoveMessageHandler);
            MessageSystem.MessageManager.UnregisterForChannel<ItemSplitMessage>(MessageChannels.Items, ItemSplitMessageHandler);
        }
    
        public virtual void CancelActionOnperformed(InputAction.CallbackContext input)
        {
            if (IsItemSlotMoving)
            {
                if (SelectedSlot != null)
                {
                    // Reset the icon of the selected slot to its original item
                    SelectedSlot.SetItemSlot(SelectedSlot.Slot);
                }

                IsItemSlotMoving = false;
            }
        }
    
        public virtual void SubmitActionOnperformed(InputAction.CallbackContext input)
        {
            if (!IsItemSlotMoving || MovingItemSlot == null || SelectedSlot == null) return;

            var selectedSlotIndex = CurrentSlots.IndexOf(SelectedSlot);
            var movingSlotIndex = CurrentSlots.IndexOf(MovingItemSlot);
            // Swap the items between slots
            var tempItem = SelectedSlot.Slot;
            SelectedSlot.SetItemSlot(MovingItemSlot.Slot);
            MovingItemSlot.SetItemSlot(tempItem);

            // Reset the icon of the selected slot to its original item (if any)
            SelectedSlot.SetItemSlot(SelectedSlot.Slot); // This will reset the icon to the actual item
        
            MessageSystem.MessageManager.SendImmediate(MessageChannels.Items, new InventoryMoveMessage(Inventory.ID, MovingItemSlot.Slot.Item, MovingItemSlot.Slot.Quantity,movingSlotIndex, selectedSlotIndex));

            // Reset moving item state
            IsItemSlotMoving = false;
            MovingItemSlot = null;
            HasItemJustMoved = true;
            StartCoroutine(ResetItemMoved(HasItemJustMovedDelay));
            ClearAndUpdateInventory();
        }
    
        public virtual IEnumerator ResetItemMoved(float timeDelay)
        {
            yield return new WaitForSeconds(timeDelay);
            HasItemJustMoved = false;
        }
    
        public virtual void SetInventory(InventoryObject inventory)
        {
            Inventory = inventory;
            if (Inventory == null)
            {
                SetObjectActive(false);
            }
            else
            {
                SetObjectActive(true);
                ClearAndUpdateInventory();
            }
        }
    
        public virtual void ClearAndUpdateInventory()
        {
            if (Inventory == null) return;

            // Update current weight
            CurrentWeight = Inventory.Items.Where(x=> x.Item != null).Sum(item => item.Item.Weight * item.Quantity);

            InitializeInventorySlots();

            UpdateSlots();

            // Remove excess slots if necessary
            RemoveExcessSlots();
        }

        private void InitializeInventorySlots()
        {
            // Create enough slots for each item in the inventory
            for (int i = 0; i < Inventory.Items.Count; i++)
            {
                if (i >= CurrentSlots.Count)
                {
                    CreateEmptySlot();
                }
                if(Inventory.Items[i] == null) continue;
                if(Inventory.Items[i].Item == null) continue;
                CurrentSlots[i].SetItemSlot(Inventory.Items[i]);
            }

            // Create additional empty slots if needed
            for (int i = Inventory.Items.Count; i < StartingSlots; i++)
            {
                CreateEmptySlot();
            }
        }

        public virtual void UpdateSlots()
        {
            foreach (var slot in CurrentSlots)
            {
                if(slot == null) continue;
                var matchingSlot = CurrentSlots.FirstOrDefault(s=> s.Slot.ID == slot.Slot.ID)?.Slot;
                if (matchingSlot != null)
                {
                    slot.SetItemSlot(matchingSlot);
                }
                else
                {
                    slot.ClearItemSlot();
                }
            }
        }

        public virtual void RemoveExcessSlots()
        {
            if (!UseMaxSlots || CurrentSlots.Count <= MaxSlots) return;

            for (int i = CurrentSlots.Count - 1; i >= MaxSlots; i--)
            {
                Destroy(CurrentSlots[i].gameObject);
                CurrentSlots.RemoveAt(i);
            }
        }

        public virtual void CreateEmptySlot()
        {
            if (InventorySlotPrefab == null) return;
            var slotObject = Instantiate(InventorySlotPrefab, ContentPanel.transform);
            var slot = slotObject.GetComponent<ItemContainerUI>();
            slot.SetItemSlot(null);
            CurrentSlots.Add(slot);

        }

        public virtual void ItemInteractMessageHandler(MessageSystem.IMessageEnvelope message)
        {
            if(!message.Message<ItemInteractMessage>().HasValue) return;
            var data = message.Message<ItemInteractMessage>().GetValueOrDefault();
            if (data.ItemContainerUI == null) return;
            if(ActionsPanel == null) return;
            if (HasItemJustMoved) return; // If an item has just been moved, return early
            ActionsPanel.ItemContainerUI = data.ItemContainerUI;
            ActionsPanel.SetObjectActive(true);
        }

        public virtual void ItemSelectedMessageHandler(MessageSystem.IMessageEnvelope message)
        {
            if (!message.Message<ItemSelectedMessage>().HasValue) return;
            var data = message.Message<ItemSelectedMessage>().GetValueOrDefault();

            if (SelectedSlot != null && IsItemSlotMoving && MovingItemSlot != SelectedSlot)
            {
                SelectedSlot.ClearTemporaryIcon();
            }
        
            SelectedSlot = CurrentSlots.FirstOrDefault(slot => slot.Slot == data.ItemSlot);

            // Show the moving item's icon in the new selected slot
            if (SelectedSlot != null && IsItemSlotMoving && MovingItemSlot != null && MovingItemSlot.Slot != null)
            {
                SelectedSlot.DisplayTemporaryIcon(MovingItemSlot.Slot);
            }

        }
    
        public virtual void InventoryChangedMessageHandler(MessageSystem.IMessageEnvelope message)
        {
            if(!message.Message<InventoryChangedMessage>().HasValue) return;
            var data = message.Message<InventoryChangedMessage>().GetValueOrDefault();
            if(Inventory == null || data.Inventory != Inventory) return;
            if(data.ID != Inventory.ID) return;
            ClearAndUpdateInventory();
        }
    
        public virtual void ItemMoveMessageHandler(MessageSystem.IMessageEnvelope message)
        {
            if(!message.Message<ItemMoveMessage>().HasValue) return;
            var data = message.Message<ItemMoveMessage>().GetValueOrDefault();
            if (data.ItemContainerUi == null) return;
            if (data.ItemContainerUi.Slot == null) return;
            if (data.ItemContainerUi.Slot.Item == null) return;
            MovingItemSlot = data.ItemContainerUi;
            IsItemSlotMoving = true;
        }
    
        public virtual void ItemSplitMessageHandler(MessageSystem.IMessageEnvelope message)
        {
            if(!message.Message<ItemSplitMessage>().HasValue) return;
            var data = message.Message<ItemSplitMessage>().GetValueOrDefault();
            if (data.ItemContainerUi == null) return;
            if (data.ItemContainerUi.Slot == null) return;
            if (data.ItemContainerUi.Slot.Item == null) return;
            SplitItemSlot = data.ItemContainerUi;
            if (SplitItemSlot.Slot.Quantity <= 1) return;
            // SplitItem();
            // var splitItem = SplitItemSlot.Slot.Item;
            // var splitQuantity = SplitItemSlot.Slot.Quantity / 2;
            // var newSlot = new ItemSlot(splitItem, splitQuantity);
            // SplitItemSlot.Slot.Quantity -= splitQuantity;
            // var emptySlot = CurrentSlots.FirstOrDefault(s => s.Slot.Item == null);
            // if (emptySlot != null)
            // {
            //     emptySlot.SetItemSlot(newSlot);
            // }
            // else
            // {
            //     var slotObject = Instantiate(InventorySlotPrefab, ContentPanel.transform);
            //     var slot = slotObject.GetComponent<ItemContainerUI>();
            //     slot.SetItemSlot(newSlot);
            //     CurrentSlots.Add(slot);
            // }
        }
    
    
    }
}
