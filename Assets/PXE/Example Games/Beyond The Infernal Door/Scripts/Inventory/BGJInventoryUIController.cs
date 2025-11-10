using System.Collections.Generic;
using PXE.Core.Enums;
using PXE.Core.Inventory.Items;
using PXE.Core.Inventory.Messaging.Messages;
using PXE.Core.Messaging;
using PXE.Core.Objects;
using UnityEngine;

namespace PXE.Example_Games.Beyond_The_Infernal_Door.Scripts.Inventory
{
    public class BGJInventoryUIController : ObjectController
    {
        [field: SerializeField] public virtual InventoryObject Inventory { get; set; }
        [field: SerializeField] public virtual List<BGJItemContainerUI> ItemContainers { get; set; } = new();
        [field: SerializeField] public virtual ObjectController InventorySlotPrefab { get; set; }

        public override void Start()
        {
            base.Start();
            if (Inventory == null) return;
            if (InventorySlotPrefab == null) return;

            if (ItemContainers.Count == 0)
            {
                for (int i = 0; i < Inventory.MaxSlots; i++)
                {
                    var slot = Instantiate(InventorySlotPrefab, transform);
                    ItemContainers.Add(slot.GetComponent<BGJItemContainerUI>());
                }
            }
        
            UpdateUI();
        }

        public override void OnActive()
        {
            base.OnActive();
            MessageSystem.MessageManager.RegisterForChannel<InventoryChangedMessage>(MessageChannels.Items, InventoryChangedMessageHandler);
        }

        public override void OnInactive()
        {
            base.OnInactive();
            MessageSystem.MessageManager.UnregisterForChannel<InventoryChangedMessage>(MessageChannels.Items, InventoryChangedMessageHandler);
        }

        public virtual void InventoryChangedMessageHandler(MessageSystem.IMessageEnvelope message)
        {
            if(!message.Message<InventoryChangedMessage>().HasValue) return;
            var data = message.Message<InventoryChangedMessage>().GetValueOrDefault();
            if(Inventory == null || data.Inventory != Inventory) return;
            if(data.ID != Inventory.ID) return;
            UpdateUI();
        }

        public virtual void UpdateUI()
        {
            if (Inventory == null) return;
            for (int i = 0; i < Inventory.MaxSlots; i++)
            {
                if (ItemContainers[i] == null) continue;
                if (i < Inventory.Items.Count)
                {
                    ItemContainers[i].SetItemSlot(Inventory.Items[i]);
                }
                else
                {
                    ItemContainers[i].ClearItemSlot();
                }
            }
        }
    
    }
}
