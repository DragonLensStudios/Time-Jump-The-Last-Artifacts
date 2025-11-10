using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using PXE.Core.Enums;
using PXE.Core.Inventory.Messaging.Messages;
using PXE.Core.Messaging;
using PXE.Core.Objects;
using PXE.Core.Player;
using PXE.Core.UI.Messaging.Messages;
using UnityEngine;

namespace PXE.Core.Inventory.Items
{
    /// <summary>
    ///  This class represents the item pickup.
    /// </summary>
    [Serializable, RequireComponent(typeof(SpriteRenderer), typeof(BoxCollider2D), typeof(Rigidbody2D))]
    public class ItemPickup : ObjectController
    {
        [field: Tooltip("The item slots for the item pickup.")]
        [field: SerializeField] public virtual List<ItemSlot> ItemSlots { get; set; }
        
        [field: Tooltip("The message delay for the item pickup.")]
        [field: SerializeField] public virtual float MessageDelay { get; set; } = 2f;
        
        [field: Tooltip("The sprite for item pickup.")]
        [field: SerializeField] public virtual Sprite Sprite { get; set; }
        
        [field: Tooltip("The sprite renderer for the item pickup.")]
        [field: SerializeField] public virtual SpriteRenderer SpriteRenderer { get; set; }
        
        [field: Tooltip("The box collider for the item pickup.")]
        [field: SerializeField] public virtual BoxCollider2D BoxCollider2D { get; set; }
        
        [field: Tooltip("The rigidbody 2D for the item pickup.")]
        [field: SerializeField] public virtual Rigidbody2D Rigidbody2D { get; set; }
        
        [field: Tooltip("The creation time for the item pickup.")]
        [field: SerializeField] public virtual float CreationTime { get; set; }
        
        public override void Start()
        {
            base.Start();
            if (SpriteRenderer == null)
            {
                SpriteRenderer = GetComponent<SpriteRenderer>();
            }
            
            if (BoxCollider2D == null)
            {
                BoxCollider2D = GetComponent<BoxCollider2D>();
            }
            
            if (Rigidbody2D == null)
            {
                Rigidbody2D = GetComponent<Rigidbody2D>();
            }
            
            SetSprite();
            CreationTime = UnityEngine.Time.time;
        }

        public virtual void OnValidate()
        {
            if (SpriteRenderer == null)
            {
                SpriteRenderer = GetComponent<SpriteRenderer>();
            }
            SetSprite();
        }

        public virtual void SetSprite()
        {
            if (SpriteRenderer == null) return;
            if(Sprite != null)
            {
                SpriteRenderer.sprite = Sprite;
                return;
            }
            if (Sprite == null)
            {
                if (ItemSlots.Any())
                {
                    Sprite = ItemSlots.FirstOrDefault()?.Item.ItemIcon;
                }
            }

            SpriteRenderer.sprite = Sprite;
        }

        /// <summary>
        ///  On trigger enter 2D event when the player enters the trigger it calls the inventory modify message amd sets the object to inactive and shows the item message.
        /// </summary>
        /// <param name="other"></param>
        public virtual void OnTriggerEnter2D(Collider2D other)
        {
            // Check if the collider is the player
            if (other.CompareTag("Player"))
            {
                var player = other.GetComponent<PlayerController>();
                if (player == null) return;

                // Process each item slot for the player
                foreach (var slot in ItemSlots)
                {
                    MessageSystem.MessageManager.SendImmediate(MessageChannels.Items, new InventoryModifyMessage(player.Inventory.ID, slot.Item, slot.Quantity, InventoryModifyType.Add));
                }

                ActiveType = ActiveType.Inactive;
                SetObjectActive(false);
                StartCoroutine(ShowItemMessage(ItemSlots, MessageDelay));
                Destroy(gameObject);
                return; // Return after handling player collision to prevent further processing
            }

            // Logic for handling collision with another item pickup
            var pickup = other.GetComponent<ItemPickup>();
            if (pickup != null && pickup != this)
            {
                // Process only if this pickup is older than the other pickup
                if (CreationTime < pickup.CreationTime)
                {
                    foreach (var pickupSlot in pickup.ItemSlots.ToList()) // Use ToList() to create a copy for safe iteration
                    {
                        var inventorySlot = ItemSlots.FirstOrDefault(itemSlot => itemSlot.Item.ID.Equals(pickupSlot.Item.ID));

                        if (inventorySlot != null)
                        {
                            int combinedQuantity = inventorySlot.Quantity + pickupSlot.Quantity;

                            if (combinedQuantity <= inventorySlot.Item.MaxStack)
                            {
                                inventorySlot.Quantity = combinedQuantity;
                                Name = $"{inventorySlot.Item.Name} X {inventorySlot.Quantity}";
                                UpdateIdentity(gameObject);
                                pickup.ItemSlots.Remove(pickupSlot);
                            }
                            else
                            {
                                int excessQuantity = combinedQuantity - inventorySlot.Item.MaxStack;
                                inventorySlot.Quantity = inventorySlot.Item.MaxStack;
                                pickupSlot.Quantity = excessQuantity;
                            }
                        }
                        else
                        {
                            ItemSlots.Add(pickupSlot);
                            pickup.ItemSlots.Remove(pickupSlot);
                        }
                    }

                    if (pickup.ItemSlots.Count == 0)
                    {
                        Destroy(pickup.gameObject);
                    }
                }
            }
        }


        /// <summary>
        ///  Show item message coroutine that shows the item message with provided item slots and delay.
        /// </summary>
        /// <param name="itemSlots"></param>
        /// <param name="delay"></param>
        /// <returns></returns>
        public virtual IEnumerator ShowItemMessage(List<ItemSlot> itemSlots, float delay)
        {
            foreach (var itemSlot in itemSlots)
            {
                MessageSystem.MessageManager.SendImmediate(MessageChannels.UI, new PopupMessage($"Picked up {itemSlot.Quantity} {itemSlot.Item.Name}", PopupType.Notification, PopupPosition.Top, delay));
                yield return new WaitForSeconds(delay);
            }
        }
    }
}