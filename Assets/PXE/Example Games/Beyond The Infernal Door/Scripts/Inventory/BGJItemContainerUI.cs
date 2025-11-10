using PXE.Core.Extensions.GameObjectExtensions;
using PXE.Core.Inventory.Items;
using PXE.Core.Objects;
using UnityEngine;
using UnityEngine.UI;

namespace PXE.Example_Games.Beyond_The_Infernal_Door.Scripts.Inventory
{
    public class BGJItemContainerUI : ObjectController
    {
        [field: SerializeField] public virtual ItemSlot Slot { get; set; }
        [field: SerializeField] public virtual Image ItemIconImage { get; set; }
    
        public override void OnActive()
        {
            base.OnActive();
            SetItemSlot(Slot);
        }
    
        public virtual void SetItemSlot(ItemSlot slot)
        {
            if (slot == null || slot.Item == null)
            {
                ItemIconImage.gameObject.SetObjectActive(false);
                return;
            }
        
            Slot = slot;

            if (slot.Item.ItemIcon == null)
            {
                ItemIconImage.gameObject.SetObjectActive(false);
            }
            if (slot.Item.ItemIcon != null)
            {
                ItemIconImage.gameObject.SetObjectActive(true);
                ItemIconImage.sprite = slot.Item.ItemIcon;
            }
            else
            {
                ItemIconImage.gameObject.SetObjectActive(false);
            }
        }
    
        public virtual void ClearItemSlot()
        {
            Slot = null;
            if(ItemIconImage == null) return;
            ItemIconImage.gameObject.SetActive(false);
        }
    
        public virtual void OnClick()
        {
            //TODO: Implement this to send a message for the item being used handling.
            if (Slot == null || Slot.Item == null) return;
            Debug.Log($"Using {Slot.Item.Name}");
            // Slot.Item.Use();
        }
    }
}