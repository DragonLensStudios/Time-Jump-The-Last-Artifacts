using System;
using System.Collections.Generic;
using System.Linq;
using PXE.Core.Achievements.Data;
using PXE.Core.Achievements.ScriptableObjects;
using PXE.Core.Actor;
using PXE.Core.Audio;
using PXE.Core.Crafting;
using PXE.Core.Data_Persistence.Data;
using PXE.Core.Data_Persistence.Interfaces;
using PXE.Core.Enums;
using PXE.Core.Inventory.Items;
using PXE.Core.Inventory.Messaging.Messages;
using PXE.Core.Levels.Messaging.Messages;
using PXE.Core.Messaging;
using PXE.Core.Messaging.Messages;
using PXE.Core.Objects;
using PXE.Core.SerializableTypes;
using PXE.Core.State_System.Messaging.Messages;
using UnityEngine;
using UnityEngine.InputSystem;

namespace PXE.Core.Player
{
    /// <summary>
    ///  Represents the PlayerController.
    /// </summary>
    [RequireComponent(typeof(Rigidbody2D), typeof(Animator))]
    public class PlayerController: ActorController, IDataPersistable
    {
        [field: Tooltip("The current steps taken.")]
        [field: SerializeField] public virtual long Steps { get; set; }
        
        [field: Tooltip("The units per step.")]
        [field: SerializeField] public virtual float StepSize { get; set; } = 1f;
        
        [field: Tooltip("The distance traveled.")]
        [field: SerializeField] public virtual float DistanceTraveled { get; set; } = 0f;
        
        [field: Tooltip("The current level name.")]
        [field: SerializeField] public virtual string CurrentLevelName { get; set; }
        
        [field: Tooltip("The current level ID.")]
        [field: SerializeField] public virtual SerializableGuid CurrentLevelID { get; set; }
        
        [field: Tooltip("The achievement manager.")]
        [field: SerializeField] public virtual AchievementManagerSettings AchievementManager { get; set; }
        
        [field: Tooltip("The achievement progress list.")]
        [field: SerializeField] public virtual List<PlayerAchievementProgress> AchievementProgressList { get; set; }
      
        [field: Tooltip("The respawn sfx.")]
        [field: SerializeField] public virtual AudioObject RespawnSfx { get; set; }
        
        [field: Tooltip("The item pickup prefab.")]
        [field: SerializeField] public virtual ObjectController ItemPickupPrefab { get; set; }
        
        [field: Tooltip("The Item Pickup prefab spawn position.")]
        [field: SerializeField] public virtual Vector3 ItemPickupPrefabSpawnOffset { get; set; }
        
        [field: Tooltip("The crafting controller.")]
        [field: SerializeField] public virtual CraftingController CraftingController { get; set; }
        
        [field: Tooltip("Is god mode enabled?")]
        [field: SerializeField] public virtual bool GodMode { get; set; }
        
        protected Vector3 movement;
        protected PlayerInputActions playerInput;
        
        /// <summary>
        ///  Sets the original gravity scale and starting position.
        /// </summary>
        public override void Start()
        {
            base.Start();
            anim = GetComponent<Animator>();
            playerInput = new PlayerInputActions();
            OriginalGravityScale = rb.gravityScale;
            StartingPosition = transform.position;
        }
        
        /// <summary>
        ///  Sets the achievement progress list.
        /// </summary>
        public virtual void SetAchievementProgress()
        {
            AchievementProgressList.Clear();
            // Create a dictionary from AchievementProgressList for easy lookup
            var progressDictionary = AchievementProgressList.ToDictionary(x => x.AchievementKey, x => x);

            // Iterate through AchievementManager.AchievementList and only add new Achievements
            foreach (var achievement in AchievementManager.AchievementList)
            {
                if (!progressDictionary.ContainsKey(achievement.Key))
                {
                    AchievementProgressList.Add(new PlayerAchievementProgress(achievement.Key));
                }
            }

            // You may want to order the list as per some property
            AchievementProgressList = AchievementProgressList.OrderBy(x => x.AchievementKey).ToList();
        }

        /// <summary>
        ///  This method registers the PlayerController for the PauseMessage message and the LevelMessage message and the GodModeMessage message and sets the achievement progress list and enables the PlayerInputActions.
        /// </summary>
        public override void OnActive()
        {
            base.OnActive();
            if (anim == null)
            {
                anim = GetComponent<Animator>();
            }

            if (rb == null)
            {
                rb = GetComponent<Rigidbody2D>();
            }

            playerInput ??= new PlayerInputActions();
            SetAchievementProgress();
            playerInput.Enable();
            playerInput.Player.Move.performed += MoveOnperformed;
            playerInput.Player.Move.canceled += MoveOncanceled;
            playerInput.Player.Interact.performed += InteractOnperformed;
            MessageSystem.MessageManager.RegisterForChannel<LevelMessage>(MessageChannels.Level, LevelMessageHandler);
            MessageSystem.MessageManager.RegisterForChannel<GodModeMessage>(MessageChannels.Player, GodModeMessageHandler);
            MessageSystem.MessageManager.RegisterForChannel<ItemDestroyMessage>(MessageChannels.Items, ItemDestroyMessageHandler);
            MessageSystem.MessageManager.RegisterForChannel<ItemDropMessage>(MessageChannels.Items, ItemDropMessageHandler);
        }

        public virtual void ItemDestroyMessageHandler(MessageSystem.IMessageEnvelope message)
        {
            if (!message.Message<ItemDestroyMessage>().HasValue) return;
            var data = message.Message<ItemDestroyMessage>().GetValueOrDefault();
            if (data.TargetID != ID) return;
            if (data.ItemObject == null) return;
            if (Inventory == null) return;
            MessageSystem.MessageManager.SendImmediate(MessageChannels.Items, new InventoryModifyMessage(Inventory.ID, data.ItemObject, data.Quantity,InventoryModifyType.Remove));
        }
        
        public virtual void ItemDropMessageHandler(MessageSystem.IMessageEnvelope message)
        {
            if (!message.Message<ItemDropMessage>().HasValue) return;
            var data = message.Message<ItemDropMessage>().GetValueOrDefault();
            if (data.TargetID != ID) return;
            if (data.ItemObject == null) return;
            if (Inventory == null) return;
            MessageSystem.MessageManager.SendImmediate(MessageChannels.Items, new InventoryModifyMessage(Inventory.ID, data.ItemObject, data.Quantity,InventoryModifyType.Remove));
            
            var itemPickupGo = Instantiate(ItemPickupPrefab, transform.position + ItemPickupPrefabSpawnOffset, Quaternion.identity);
            var itemPickup = itemPickupGo.GetComponent<ItemPickup>();
            itemPickup.Name = $"{data.ItemObject.Name} X {data.Quantity}";
            UpdateIdentity(gameObject);
            itemPickup.ItemSlots.Add(new ItemSlot(data.ItemObject, data.Quantity));
            itemPickup.SetSprite();
        }

        /// <summary>
        ///  This method unregisters the PlayerController for the PauseMessage message and the LevelMessage message and the GodModeMessage message and disables the PlayerInputActions.
        /// </summary>
        public override void OnInactive()
        {
            base.OnInactive();
            playerInput ??= new PlayerInputActions();
            playerInput.Disable();
            playerInput.Player.Move.performed -= MoveOnperformed;
            playerInput.Player.Move.canceled -= MoveOncanceled;
            playerInput.Player.Interact.performed -= InteractOnperformed;
            MessageSystem.MessageManager.UnregisterForChannel<LevelMessage>(MessageChannels.Level, LevelMessageHandler);
            MessageSystem.MessageManager.UnregisterForChannel<GodModeMessage>(MessageChannels.Player, GodModeMessageHandler);
            MessageSystem.MessageManager.UnregisterForChannel<ItemDestroyMessage>(MessageChannels.Items, ItemDestroyMessageHandler);
            MessageSystem.MessageManager.UnregisterForChannel<ItemDropMessage>(MessageChannels.Items, ItemDropMessageHandler);
        }

        /// <summary>
        ///  Sets the movement to zero and sets the isMoving parameter to false and sets the move_x and move_y parameters to the last movement position.
        /// </summary>
        /// <param name="input"></param>
        public virtual void MoveOncanceled(InputAction.CallbackContext input)
        {
            if (IsDisabled) return;
            MovementDirection = movement;
            movement = Vector2.zero;
            anim.SetBool("isMoving", false);
            anim.SetFloat("move_x", MovementDirection.x);
            anim.SetFloat("move_y", MovementDirection.y);
        }

        /// <summary>
        ///  Sets the movement to the input value and sets the isMoving parameter to true and sets the move_x and move_y parameters to the movement x and y values.
        /// </summary>
        /// <param name="input"></param>
        public virtual void MoveOnperformed(InputAction.CallbackContext input)
        {
            if (IsDisabled) return;
            movement = input.ReadValue<Vector2>();
            anim.SetBool("isMoving", true);
            anim.SetFloat("move_x", movement.x);
            anim.SetFloat("move_y", movement.y);
        }
        
        /// <summary>
        ///  Calls the Interact method.
        /// </summary>
        /// <param name="input"></param>
        public virtual void InteractOnperformed(InputAction.CallbackContext input)
        {
            if (IsDisabled) return;
            Interact();
        }

        /// <summary>
        ///  Sets the movement to the normalized movement times the move speed and sets the rigidbody2D velocity to the target velocity and adds the movement magnitude times the move speed times the fixed delta time to the distance traveled and if the distance traveled is greater than or equal to the step size increments the steps and subtracts the step size from the distance traveled.
        /// </summary>
        public override void FixedUpdate()
        {
            if (IsDisabled) return;
            Vector2 targetVelocity = movement.normalized * MoveSpeed;
            rb.linearVelocity = targetVelocity;
        }  
        
        /// <summary>
        ///  This method handles the pause message and disables the player input and sets the movement disabled to true and sets the rigidbody2D gravity scale to zero and if the game is not paused enables the player input and sets the movement disabled to false and sets the rigidbody2D gravity scale to the original gravity scale.
        /// </summary>
        /// <param name="message"></param>
        public override void PauseMessageHandler(MessageSystem.IMessageEnvelope message)
        {
            if (!message.Message<PauseMessage>().HasValue) return;
            var data = message.Message<PauseMessage>().GetValueOrDefault();
            base.PauseMessageHandler(message);
            if (data.IsPaused)
            {
                playerInput.Disable();
                IsDisabled = true;
                if (rb != null && !rb.isKinematic)
                {
                    rb.gravityScale = 0;
                }
            }
            else
            {
                playerInput.Enable();
                IsDisabled = false;
                if (rb != null && !rb.isKinematic)
                {
                    rb.gravityScale = OriginalGravityScale;
                }
            }
        }
        
        /// <summary>
        ///  This method handles the level message and sets the current level name and the current level ID and sets the position to the position from the message.
        /// </summary>
        /// <param name="message"></param>
        public virtual void LevelMessageHandler(MessageSystem.IMessageEnvelope message)
        {
            if (transform == null) return;
            if (!message.Message<LevelMessage>().HasValue) return;
            var data = message.Message<LevelMessage>().GetValueOrDefault();
            if (data.LevelState != LevelState.Loading) return;
            CurrentLevelName = data.LevelName;
            CurrentLevelID = data.LevelID;
            transform.position = data.Position;
            StartingPosition = data.Position;
        }
        
        // /// <summary>
        // ///  This method handles the interaction with the object.
        // /// </summary>
        // public virtual void Interact()
        // {
            
        // }
        
        /// <summary>
        ///  This method handles the god mode message and sets the god mode to the data god mode.
        /// </summary>
        /// <param name="message"></param>
        public virtual void GodModeMessageHandler(MessageSystem.IMessageEnvelope message)
        {
            if (!message.Message<GodModeMessage>().HasValue) return;
            var data = message.Message<GodModeMessage>().GetValueOrDefault();
            GodMode = data.GodMode;
            var godModeText = GodMode ? "ENABLED" : "DISABLED";
            Debug.Log($"GOD MODE: {godModeText}");
        }

        public virtual void LoadData<T>(T loadedGameData) where T : class, IGameDataContent, new()
        {
            if (loadedGameData is BaseGameData gameData)
            {
                if (!gameData.ID.Equals(ID)) return;
                transform.position = gameData.Position;
                CurrentLevelID = gameData.CurrentLevelID;
                CurrentLevelName = gameData.CurrentLevelName;
                AchievementProgressList = gameData.Achievements;
                MoveSpeed = gameData.MoveSpeed;
                if (Inventory != null && gameData.Inventory != null)
                {
                    Inventory.Load(gameData.Inventory);
                }
                ReferenceState = gameData.ReferenceState;
                MovementDirection = gameData.MovementDirection;
            }
        }

        public virtual void SaveData<T>(T savedGameData) where T : class, IGameDataContent, new()
        {
            if (savedGameData is BaseGameData gameData)
            {
                gameData.ID = ID;
                gameData.Name = Name;
                gameData.Position = transform.position;
                gameData.CurrentLevelID = CurrentLevelID;
                gameData.CurrentLevelName = CurrentLevelName;
                gameData.Achievements = AchievementProgressList;
                gameData.MoveSpeed = MoveSpeed;
                if (Inventory != null)
                {
                    gameData.Inventory = Inventory.Save();
                }
                gameData.ReferenceState = ReferenceState;
                gameData.MovementDirection = MovementDirection;
                gameData.LastUpdated = System.DateTime.Now;
            }
            
        }

        public override void LateUpdate() // Cleanup code, Overrides should run base.LateUpdate()
        {
            if (IsDisabled) return;
            base.LateUpdate();

            DistanceTraveled += Mathf.Min(rb.linearVelocity.magnitude * UnityEngine.Time.deltaTime, Vector2.Distance(transform.position, LastPosition));
            if (!(DistanceTraveled >= StepSize)) return;
            Steps++;
            DistanceTraveled -= StepSize;

            LastPosition = transform.position;
        }

        public override void OnDestroy()
        {
            base.OnDestroy();
            OnInactive();
        }
    }
}