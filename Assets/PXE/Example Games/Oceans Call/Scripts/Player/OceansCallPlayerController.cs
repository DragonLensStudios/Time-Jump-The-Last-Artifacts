using PXE.Core.Audio.Messaging.Messages;
using PXE.Core.Enums;
using PXE.Core.Game.Managers;
using PXE.Core.Levels.Messaging.Messages;
using PXE.Core.Messaging;
using PXE.Core.Messaging.Messages;
using PXE.Core.Player;
using PXE.Core.State_System;
using PXE.Core.State_System.Messaging.Messages;
using PXE.Example_Games.Oceans_Call.Data_Persistence.Data;
using PXE.Example_Games.Oceans_Call.Messages;
using UnityEngine;

namespace PXE.Example_Games.Oceans_Call.Player
{
    /// <summary>
    ///  Represents the OceansCallPlayerController.
    /// </summary>
    [RequireComponent(typeof(Rigidbody2D), typeof(Animator))]
    public class OceansCallPlayerController : PlayerController
    {
        [field: Tooltip("The starting number of lives for the player.")]
        [SerializeField] protected int startingLives = 3;
        
        [field: Tooltip("The current number of lives for the player.")]
        [SerializeField] protected int currentLives = 3;
        
        [field: Tooltip("The maximum number of lives for the player.")]
        [SerializeField] protected int maxLives = 5;

        /// <summary>
        ///  The starting number of lives for the player.
        /// </summary>
        public virtual int StartingLives => startingLives;
        
        /// <summary>
        ///  The current number of lives for the player.
        /// </summary>
        public virtual int CurrentLives
        {
            get => currentLives;
            set => currentLives = Mathf.Clamp(value, 0, maxLives);
        }

        /// <summary>
        ///  The maximum number of lives for the player.
        /// </summary>
        public virtual int MaxLives
        {
            get => maxLives;
            set
            {
                maxLives = value;
                CurrentLives = Mathf.Clamp(CurrentLives, 0, maxLives); // Ensure current lives doesn't exceed the new max value.
            }
        }
        
        [field: Tooltip("The depth in meters that the player has reached.")]
        [field: SerializeField] public virtual float DepthInMeters { get; set; }
        
        [field: Tooltip("The last checkpoint position for the player.")]
        [field: SerializeField] public virtual Vector2 LastCheckpointPosition { get; set; }
        
        [field: Tooltip("The bounds for the player.")]
        [field: SerializeField] public virtual Vector2 Bounds { get; set; }

        /// <summary>
        ///  Sets the LastCheckpointPosition to the current position.
        /// </summary>
        public override void Start()
        {
            base.Start();
            LastCheckpointPosition = transform.position;
        }

        /// <summary>
        ///  Registers for the Player channel and handles TargetDamageMessages and PlayerLifePowerUpMessages and registers for the Level channel and handles LevelResetMessages.
        /// </summary>
        public override void OnActive()
        {
            base.OnActive();
            MessageSystem.MessageManager.RegisterForChannel<TargetDamageMessage>(MessageChannels.Player, TargetDamageMessageHandler);
            MessageSystem.MessageManager.RegisterForChannel<PlayerLifePowerUpMessage>(MessageChannels.Player, PlayerLifePowerUpMessageHandler);
        }



        /// <summary>
        ///  Unregisters for the Player channel and stops handling TargetDamageMessages and PlayerLifePowerUpMessages and unregisters for the Level channel and stops handling LevelResetMessages.
        /// </summary>
        public override void OnInactive()
        {
            base.OnInactive();
            MessageSystem.MessageManager.UnregisterForChannel<TargetDamageMessage>(MessageChannels.Player, TargetDamageMessageHandler);
            MessageSystem.MessageManager.UnregisterForChannel<PlayerLifePowerUpMessage>(MessageChannels.Player, PlayerLifePowerUpMessageHandler);

        }

        /// <summary>
        ///  Handles the Update functionality and sends DepthChangedMessages and PlayerInfoMessages.
        /// </summary>
        public override void Update()
        {
            base.Update();
            DepthInMeters = StartingPosition.y - rb.position.y;
            MessageSystem.MessageManager.SendImmediate(MessageChannels.Lighting, new DepthChangedMessage(DepthInMeters));
            MessageSystem.MessageManager.SendImmediate(MessageChannels.UI, new PlayerInfoMessage(currentLives, DepthInMeters));
        }

        /// <summary>
        ///  Handles the FixedUpdate functionality and applies a downward force if the player is above the bounds.
        /// </summary>
        public override void FixedUpdate()
        {
            base.FixedUpdate();
            if(rb.position.y > Bounds.y)
            {
                rb.AddForce(new Vector2(0, -MoveSpeed)); // Apply a downward force
            }
            else
            {
                base.FixedUpdate();
                rb.AddForce(movement * MoveSpeed);
            }

        }

        public override void Interact()
        {
            // Do nothing when interacting since the player should not be able to interact with anything.
        }

        /// <summary>
        ///  Handles the TargetDamageMessage functionality and applies damage to the player.
        /// </summary>
        /// <param name="message"></param>
        public virtual void TargetDamageMessageHandler(MessageSystem.IMessageEnvelope message)
        {
            if(!message.Message<TargetDamageMessage>().HasValue) return;
            if(GodMode) return;
            var data = message.Message<TargetDamageMessage>().GetValueOrDefault();
            if(data.ID != ID) return;
            CurrentLives -= data.Damage;
            if (CurrentLives > 0)
            {
                transform.position = LastCheckpointPosition;
                MessageSystem.MessageManager.SendImmediate(MessageChannels.Audio, new AudioMessage(RespawnSfx, AudioOperation.Play, AudioChannel.SoundEffects));
            }
            else
            {
                // LastCheckpointPosition = StartingPosition;
                MessageSystem.MessageManager.SendImmediate(MessageChannels.GameFlow, new PauseMessage(this, true));
                MessageSystem.MessageManager.SendImmediate(MessageChannels.GameFlow, new GameStateMessage(GameManager.Instance.GetStateByType<GameOverState>()));
            }

        }
        
        /// <summary>
        ///  Handles the PlayerLifePowerUpMessage functionality and applies lives to the player.
        /// </summary>
        /// <param name="message"></param>
        public virtual void PlayerLifePowerUpMessageHandler(MessageSystem.IMessageEnvelope message)
        {
            if(!message.Message<PlayerLifePowerUpMessage>().HasValue) return;
            var data = message.Message<PlayerLifePowerUpMessage>().GetValueOrDefault();
            if(data.ID != ID) return;
            CurrentLives += data.Lives;
        }

        /// <summary>
        ///  Handles the LevelResetMessage functionality and resets the player.
        /// </summary>
        /// <param name="message"></param>
        public override void LevelResetMessageHandler(MessageSystem.IMessageEnvelope message)
        {
            if (!message.Message<LevelResetMessage>().HasValue) return;
            if(gameObject == null) return;
            base.LevelResetMessageHandler(message);
            transform.position = StartingPosition;
            CurrentLives = startingLives;
            SetObjectActive(true);
        }

        public override void LoadData<T>(T loadedGameData)
        {
            base.LoadData(loadedGameData);
            if (loadedGameData is OceansCallGameData oceansCallGameData)
            {
                currentLives = oceansCallGameData.CurrentLives;
            }
            
        }

        public override void SaveData<T>(T savedGameData)
        {
            base.SaveData(savedGameData);
            if (savedGameData is OceansCallGameData oceansCallGameData)
            {
                oceansCallGameData.CurrentLives = CurrentLives;
            }
        }

        public override void OnDestroy()
        {
            base.OnDestroy();
            OnInactive();
        }
    }
}