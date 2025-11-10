using System.Collections.Generic;
using PXE.Core.Objects;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace PXE.Core.Inventory.UI
{
    public class ItemActionPanelUI : ObjectController
    {
        [field: SerializeField] public virtual ItemContainerUI ItemContainerUI { get; set; }
        [field: SerializeField] public virtual Vector2 Position { get; set; }
        [field: SerializeField] public virtual ObjectController UseButton { get; set; }
        [field: SerializeField] public virtual ObjectController MoveButton { get; set; }
        [field: SerializeField] public virtual ObjectController SplitButton { get; set; }
        [field: SerializeField] public virtual ObjectController DropButton { get; set; }
        [field: SerializeField] public virtual ObjectController DestroyButton { get; set; }
        [field: SerializeField] public virtual ObjectController CancelButton { get; set; }
        [field: SerializeField] public virtual InputActionReference CancelAction { get; set; }
    
        public override void OnActive()
        {
            base.OnActive();
            if(ItemContainerUI == null) return;
            SetPosition(Position);
            if (CancelAction == null || CancelAction.action == null) return;
            CancelAction.action.Enable();
            CancelAction.action.performed += OnCancel;
            if(ItemContainerUI.Slot.Item == null) return;
        
            if (MoveButton == null) return;
            MoveButton.SetObjectActive(true);
        
            if(UseButton == null) return;
        
            if (ItemContainerUI.Slot.Item.Usable == false)
            {
                UseButton.SetObjectActive(false);
                EventSystem.current.SetSelectedGameObject(null);
                EventSystem.current.SetSelectedGameObject(MoveButton.gameObject);
            }
            else
            {
                UseButton.SetObjectActive(true);
                EventSystem.current.SetSelectedGameObject(null);
                EventSystem.current.SetSelectedGameObject(UseButton.gameObject);
            }
        
            if(SplitButton == null) return;

            if(ItemContainerUI.Slot.Item.IsStackable == false || ItemContainerUI.Slot.Quantity <= 1)
            {
                SplitButton.SetObjectActive(false);
            }
            else
            {
                SplitButton.SetObjectActive(true);
            }
            if(DropButton == null) return;

            if(ItemContainerUI.Slot.Item.Droppable == false)
            {
                DropButton.SetObjectActive(false);
            }
            else
            {
                DropButton.SetObjectActive(true);
            }
        
            if(DestroyButton == null) return;

            if(ItemContainerUI.Slot.Item.Destroyable == false)
            {
                DestroyButton.SetObjectActive(false);
            }
            else
            {
                DestroyButton.SetObjectActive(true);
            }
        
            if(CancelButton == null) return;
            CancelButton.SetObjectActive(true);
        
            UpdateNavigation(); // Update navigation based on active buttons

        
        }

        private void OnCancel(InputAction.CallbackContext input)
        {
            EventSystem.current.SetSelectedGameObject(null);
            EventSystem.current.SetSelectedGameObject(ItemContainerUI.gameObject);
            HidePanel();
        }
    
        public virtual void UpdateNavigation()
        {
            List<ObjectController> activeButtons = new List<ObjectController>
            {
                UseButton, MoveButton, SplitButton, DropButton, DestroyButton, CancelButton
            };

            // Filter out inactive buttons
            activeButtons = activeButtons.FindAll(button => button != null && button.IsActive);

            for (int i = 0; i < activeButtons.Count; i++)
            {
                Navigation nav = new Navigation
                {
                    mode = Navigation.Mode.Explicit,
                    selectOnUp = (i == 0) ? activeButtons[^1].GetComponent<Selectable>() : activeButtons[i - 1].GetComponent<Selectable>(),
                    selectOnDown = (i == activeButtons.Count - 1) ? activeButtons[0].GetComponent<Selectable>() : activeButtons[i + 1].GetComponent<Selectable>()
                };

                activeButtons[i].GetComponent<Selectable>().navigation = nav;
            }
        }

    
        public virtual void HidePanel()
        {
            if(UseButton != null)
            {
                UseButton.SetObjectActive(false);
            }
            if(MoveButton != null)
            {
                MoveButton.SetObjectActive(false);
            }
            if(SplitButton != null)
            {
                SplitButton.SetObjectActive(false);
            }
            if(DropButton != null)
            {
                DropButton.SetObjectActive(false);
            }
            if(DestroyButton != null)
            {
                DestroyButton.SetObjectActive(false);
            }
            if(CancelButton != null)
            {
                CancelButton.SetObjectActive(false);
            }
            SetObjectActive(false);
        
        }

        public override void OnInactive()
        {
            base.OnInactive();
            if(ItemContainerUI == null) return;
            if (CancelAction == null || CancelAction.action == null) return;
            CancelAction.action.Disable();
            CancelAction.action.performed -= OnCancel;
        }

        public virtual void SetPosition(Vector2 position)
        {
            // Check if ItemContainerUI is set and has a RectTransform.
            if (ItemContainerUI != null && ItemContainerUI.GetComponent<RectTransform>() != null)
            {
                RectTransform containerRect = ItemContainerUI.GetComponent<RectTransform>();
                RectTransform thisRect = GetComponent<RectTransform>();

                if (thisRect != null)
                {
                    // Convert local position offset based on the scale of the screen
                    Vector2 scaledPosition = new Vector2(position.x * containerRect.rect.width, position.y * containerRect.rect.height);
                
                    // Apply the scaledPosition as the anchored position with respect to the ItemContainerUI's anchored position
                    thisRect.anchoredPosition = containerRect.anchoredPosition + scaledPosition;
                }
                else
                {
                    Debug.LogError("ItemActionPanelUI does not have a RectTransform component!");
                }
            }
            else
            {
                Debug.LogWarning("ItemContainerUI is not set or does not have a RectTransform component for ItemActionPanelUI!");
            }
        }
    
        public virtual void UseItem()
        {
            // Do something with the item.
            if (ItemContainerUI == null) return;
            ItemContainerUI.UseItem();

            HidePanel();
        }

        public virtual void MoveItem()
        {
            if (ItemContainerUI == null) return;
            ItemContainerUI.MoveItem();
            HidePanel();
        }
    
        public virtual void SplitItem()
        {
            if (ItemContainerUI == null) return;
            ItemContainerUI.SplitItem();
            HidePanel();
        }
    
        public virtual void DropItem()
        {
            if (ItemContainerUI == null) return;
            ItemContainerUI.DropItem();
            HidePanel();
        }
    
        public virtual void DestroyItem()
        {
            if (ItemContainerUI == null) return;
            ItemContainerUI.DestroyItem();
            HidePanel();
        }
    
        public virtual void Cancel()
        {
            if (ItemContainerUI == null) return;
            EventSystem.current.SetSelectedGameObject(null);
            EventSystem.current.SetSelectedGameObject(ItemContainerUI.gameObject);
            HidePanel();
        }
    }
}