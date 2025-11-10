using PXE.Core.Enums;
using PXE.Core.Extensions.GameObjectExtensions;
using PXE.Core.Inventory.Items;
using PXE.Core.Inventory.Messaging.Messages;
using PXE.Core.Messaging;
using PXE.Core.Objects;
using PXE.Core.Player.Managers;
using PXE.Core.UI.Messaging.Messages;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace PXE.Core.Inventory.UI
{
    /// <summary>
    ///  This class is used to display an ItemSlot in a UI element.
    /// </summary>
    public class ItemContainerUI : ObjectController, ISelectHandler, IPointerEnterHandler, IPointerClickHandler
    {
        [field: SerializeField] public virtual ItemSlot Slot { get; set; }
        [field: SerializeField] public virtual Image ItemIconImage { get; set; }
        [field: SerializeField] public virtual TMP_Text ItemCountText { get; set; }
    
        public override void OnActive()
        {
            base.OnActive();
            SetItemSlot(Slot);
        }

        /// <summary>
        ///  Sets the ItemSlot for this ItemContainerUI and updates the UI elements to match the ItemSlot.
        /// </summary>
        /// <param name="slot"></param>
        public virtual void SetItemSlot(ItemSlot slot)
        {
            if (slot == null || slot.Item == null)
            {
                ItemIconImage.gameObject.SetObjectActive(false);
                ItemCountText.gameObject.SetObjectActive(false);    
                ItemCountText.text = string.Empty;
                return;
            }
        
            Slot = slot;

            if (slot.Item.ItemIcon == null)
            {
                ItemIconImage.gameObject.SetObjectActive(false);
                ItemCountText.gameObject.SetObjectActive(false);
                ItemCountText.text = string.Empty;
            }
            if (slot.Item.ItemIcon != null)
            {
                ItemIconImage.gameObject.SetObjectActive(true);
                ItemIconImage.sprite = slot.Item.ItemIcon;
                ItemCountText.gameObject.SetObjectActive(true);
                ItemCountText.text = $"{slot.Quantity}";
            }
            else
            {
                ItemIconImage.gameObject.SetObjectActive(false);
                ItemCountText.gameObject.SetObjectActive(false);
            }
        
            ItemCountText.gameObject.SetObjectActive(slot.Quantity > 1);
            ItemCountText.text = $"{slot.Quantity}";
        
        }
    
        public virtual void ClearItemSlot()
        {
            Slot = null;
            if(ItemIconImage == null || ItemCountText == null) return;
            ItemIconImage.gameObject.SetObjectActive(false);
            ItemCountText.gameObject.SetObjectActive(false);
            ItemCountText.text = string.Empty;
        }
    
        public virtual void InteractWithItem()
        {
            if (Slot == null) return;
            if (Slot.Item == null) return;
            MessageSystem.MessageManager.SendImmediate(MessageChannels.UI, new ItemInteractMessage(this));
        }
    
        public virtual void UseItem()
        {
            if (Slot == null) return;
            if (Slot.Item == null) return;
            //TODO: Remove dependacy for PlayerManager Here
            Slot.Item.Use(1, PlayerManager.Instance.Player.ID);
            MessageSystem.MessageManager.SendImmediate(MessageChannels.UI, new PopupMessage($"{Slot.Item.Name} used!", PopupType.Notification, PopupPosition.Bottom, 1f));
            EventSystem.current.SetSelectedGameObject(null);
            EventSystem.current.SetSelectedGameObject(gameObject);
        }
    
        public virtual void DropItem()
        {
            if (Slot == null) return;
            if (Slot.Item == null) return;
            //TODO: Remove dependacy for PlayerManager Here
            Slot.Item.Drop(1, PlayerManager.Instance.Player.ID);
            EventSystem.current.SetSelectedGameObject(null);
            EventSystem.current.SetSelectedGameObject(gameObject);
        }
    
        public virtual void DestroyItem()
        {
            if (Slot == null) return;
            if (Slot.Item == null) return;
            MessageSystem.MessageManager.SendImmediate(MessageChannels.UI, new PopupMessage("Are you sure you want to destroy this item?",
                PopupType.Confirm, PopupPosition.Middle, 0, null,
                () =>
                {
                    
                }, () =>
                {
                    Slot.Item.Destroy(1, PlayerManager.Instance.Player.ID);
                    if(Slot.Quantity <= 0)
                    {
                        ClearItemSlot();
                    }
                }));
        }

        public virtual void OnSelect(BaseEventData eventData)
        {
            MessageSystem.MessageManager.SendImmediate(MessageChannels.Items, new ItemSelectedMessage(Slot));
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            EventSystem.current.SetSelectedGameObject(null);
            EventSystem.current.SetSelectedGameObject(gameObject);
        }
    
        public void OnPointerClick(PointerEventData eventData)
        {
            EventSystem.current.SetSelectedGameObject(null);
            EventSystem.current.SetSelectedGameObject(gameObject);
            MessageSystem.MessageManager.SendImmediate(MessageChannels.Items, new ItemSelectedMessage(Slot));
        }

        public virtual void MoveItem()
        {
            if (Slot == null) return;
            if (Slot.Item == null) return;
            EventSystem.current.SetSelectedGameObject(null);
            EventSystem.current.SetSelectedGameObject(gameObject);
            MessageSystem.MessageManager.SendImmediate(MessageChannels.Items , new ItemMoveMessage(this));
        }

        public virtual void SplitItem()
        {
            if (Slot == null) return;
            if (Slot.Item == null) return;
            EventSystem.current.SetSelectedGameObject(null);
            EventSystem.current.SetSelectedGameObject(gameObject);
            MessageSystem.MessageManager.SendImmediate(MessageChannels.Items, new ItemSplitMessage(this));
        }
    
        public virtual void DisplayTemporaryIcon(ItemSlot slot)
        {
            if (slot == null) return;
            if (slot.Item == null) return;
            var icon = slot.Item.ItemIcon;
            if (icon != null)
            {
                ItemIconImage.sprite = icon;
                ItemIconImage.gameObject.SetObjectActive(true);
                ItemCountText.gameObject.SetObjectActive(true);
                ItemCountText.text = slot.Quantity > 1 ? $"{slot.Quantity}" : string.Empty;
            }
        }

        public virtual void ClearTemporaryIcon()
        {
            if (Slot != null && Slot.Item != null)
            {
                // Reset to the original icon
                ItemIconImage.sprite = Slot.Item.ItemIcon;
                ItemIconImage.gameObject.SetObjectActive(true);
                ItemCountText.gameObject.SetObjectActive(true);
                ItemCountText.text = Slot.Quantity > 1 ? $"{Slot.Quantity}" : string.Empty;
            }
            else
            {
                // Clear the icon if the slot is empty
                ItemIconImage.gameObject.SetObjectActive(false);
                ItemCountText.gameObject.SetObjectActive(false);
            }
        }

    }
}