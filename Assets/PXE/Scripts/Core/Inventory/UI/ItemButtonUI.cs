using PXE.Core.Enums;
using PXE.Core.Inventory.Items;
using PXE.Core.Inventory.Messaging.Messages;
using PXE.Core.Messaging;
using PXE.Core.Objects;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace PXE.Core.Inventory.UI
{
    public class ItemButtonUI : ObjectController, ISelectHandler
    {
        [field: SerializeField] public virtual ItemSlot ItemSlot { get; set; }
        [field: SerializeField] public virtual TMP_Text ItemNameText { get; set; }
        [field: SerializeField] public virtual TMP_Text ItemCountText { get; set; }
        [field: SerializeField] public virtual TMP_Text ItemDescriptionText { get; set; }
        [field: SerializeField] public virtual Image ItemIcon { get; set; }

        public override void OnActive()
        {
            base.OnActive();
            MessageSystem.MessageManager.RegisterForChannel<ItemPickupMessage>(MessageChannels.Items, ItemPickupMessageHandler);
        }
    
        public override void OnInactive()
        {
            base.OnInactive();
            MessageSystem.MessageManager.UnregisterForChannel<ItemPickupMessage>(MessageChannels.Items, ItemPickupMessageHandler);
        }

        public virtual void ItemPickupMessageHandler(MessageSystem.IMessageEnvelope message)
        {
            if(!message.Message<ItemPickupMessage>().HasValue) return;
            var itemPickupMessage = message.Message<ItemPickupMessage>().Value;
            if (itemPickupMessage.ItemObject == ItemSlot.Item)
            {
                ItemSlot.Quantity += itemPickupMessage.Amount;
                SetItemSlot(ItemSlot);
            }
        }

        public virtual void SetItemSlot(ItemSlot itemSlot)
        {
            ItemSlot = itemSlot;
            if (ItemSlot == null || ItemSlot.Item == null)
            {
                ItemNameText.text = "";
                ItemCountText.text = "";
                ItemDescriptionText.text = "";
                ItemIcon.sprite = null;
                ItemIcon.enabled = false;
            }
            else
            {
                ItemNameText.text = ItemSlot.Item.Name;
                ItemCountText.text = ItemSlot.Quantity.ToString();
                ItemDescriptionText.text = ItemSlot.Item.Description;
                if (ItemSlot.Item.ItemIcon != null)
                {
                    ItemIcon.sprite = ItemSlot.Item.ItemIcon;
                    ItemIcon.enabled = true;
                }
                else
                {
                    ItemIcon.sprite = null;
                    ItemIcon.enabled = false;                
                }
            }
        }
    
        public virtual void SetItemSlot(ItemObject item, int quantity)
        {
            ItemSlot = new ItemSlot(item, quantity);
            if (ItemSlot == null || ItemSlot.Item == null)
            {
                ItemNameText.text = "";
                ItemCountText.text = "";
                ItemDescriptionText.text = "";
                ItemIcon.sprite = null;
                ItemIcon.enabled = false;
            }
            else
            {
                ItemNameText.text = ItemSlot.Item.Name;
                ItemCountText.text = ItemSlot.Quantity.ToString();
                ItemDescriptionText.text = ItemSlot.Item.Description;
                if (ItemSlot.Item.ItemIcon != null)
                {
                    ItemIcon.sprite = ItemSlot.Item.ItemIcon;
                    ItemIcon.enabled = true;
                }
                else
                {
                    ItemIcon.sprite = null;
                    ItemIcon.enabled = false;                
                }
            }
        }

        public virtual void OnSelect(BaseEventData eventData)
        {
            // MessageSystem.MessageManager.SendImmediate(MessageChannels.UI, new ToolTipMessage(true, ItemSlot.Item.Description));
        }
    }
}
