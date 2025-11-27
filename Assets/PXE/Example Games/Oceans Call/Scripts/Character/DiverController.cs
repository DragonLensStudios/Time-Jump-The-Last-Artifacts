using System;
using System.Collections.Generic;
using PXE.Core.Actor;
using PXE.Core.Data_Persistence.Interfaces;
using PXE.Core.Dialogue.Messaging.Messages;
using PXE.Core.Enums;
using PXE.Core.Levels.Messaging.Messages;
using PXE.Core.Messaging;
using PXE.Core.Messaging.Messages;
using PXE.Core.Player;
using PXE.Core.Player.Managers;
using PXE.Core.UI.Messaging.Messages;
using PXE.Example_Games.Oceans_Call.Data_Persistence.Data;
using UnityEngine;
using UnityEngine.InputSystem;

namespace PXE.Example_Games.Oceans_Call.Character
{
    public class DiverController : ActorController, IDataPersistable
        {
            [field: Tooltip("The current speed of the diver.")]
            [SerializeField] protected float currentSpeed = 6f;
            
            [field: Tooltip("The minimum speed of the diver.")]
            [field: SerializeField] public virtual float MinSpeed { get; set; } = 6f;
            
            [field: Tooltip("The maximum speed of the diver.")]
            [field: SerializeField] public virtual float MaxSpeed { get; set; } = 15f;
            
            [field: Tooltip("The speed increase amount of the diver when they reach the player reaches the diver.")]
            [field: SerializeField] public virtual float IncreaseSpeedWhenReached { get; set; } = 3;
            
            [field: Tooltip("The action reference for the diver.")]
            [field: SerializeField] public virtual InputActionReference ActionReference { get; set; }
            
            [field: Tooltip("The depth in meters that the diver has reached.")]
            [field: SerializeField] public virtual float DepthInMeters { get; set; }
            
            [field: Tooltip("The dialogue trigger depths for the diver.")]
            [field: SerializeField] public virtual List<float> DialogueTriggerDepths { get; set; }
            
            [field: Tooltip("The last depth that the diver triggered a dialogue at.")]
            [field: SerializeField] public virtual float LastDepthTriggered { get; set; }
            
            protected PlayerController player;
            protected List<float> dialogueTriggerDepthsInitial;
            
            /// <summary>
            ///  Sets the player reference and the initial dialogue trigger depths.
            /// </summary>
            public override void Start()
            {
                base.Start();
                player = PlayerManager.Instance.Player;
                dialogueTriggerDepthsInitial = new List<float>(DialogueTriggerDepths);
            }
    
            /// <summary>
            ///  The current speed of the diver clamped between the MinSpeed and MaxSpeed.
            /// </summary>
            public float CurrentSpeed
            {
                get => currentSpeed;
                set => currentSpeed = value < MinSpeed ? MinSpeed : (value > MaxSpeed ? MaxSpeed : value);
            }
    
            /// <summary>
            ///  Registers for the Gameplay and Level channels and handles PatrolPointReachedMessages and LevelResetMessages.
            /// </summary>
            public override void OnActive()
            {
                base.OnActive();
                PatrolSpeed = CurrentSpeed;
                MessageSystem.MessageManager.RegisterForChannel<PatrolPointReachedMessage>(MessageChannels.Gameplay, PatrolPointReachedMessageHandler);
                MessageSystem.MessageManager.RegisterForChannel<LevelResetMessage>(MessageChannels.Level, LevelResetMessageHandler);
            }
    
            /// <summary>
            ///  Unregisters for the Gameplay and Level channels and stops handling PatrolPointReachedMessages and LevelResetMessages.
            /// </summary>
            public override void OnInactive()
            {
                base.OnInactive();
                MessageSystem.MessageManager.UnregisterForChannel<PatrolPointReachedMessage>(MessageChannels.Gameplay, PatrolPointReachedMessageHandler);
                MessageSystem.MessageManager.UnregisterForChannel<LevelResetMessage>(MessageChannels.Level, LevelResetMessageHandler);
            }
    
            /// <summary>
            ///  Handles the Update functionality and sets the DepthInMeters to the difference between the starting position and the current position.
            /// </summary>
            public override void Update()
            {
                DepthInMeters = StartingPosition.y - rb.position.y;
            }
    
            /// <summary>
            ///  Handles the PatrolPointReachedMessage functionality and sets the ReachedLastWaypoint to the ReachedFinalPosition.
            /// </summary>
            /// <param name="message"></param>
            public virtual void PatrolPointReachedMessageHandler(MessageSystem.IMessageEnvelope message)
            {
                if (!message.Message<PatrolPointReachedMessage>().HasValue) return;
                var data = message.Message<PatrolPointReachedMessage>().GetValueOrDefault();
                if(!data.ID.Equals(ID)) return;
                ReachedLastWaypoint = data.ReachedFinalPosition;
            }
    
            /// <summary>
            ///  When triggered by the player, the diver will increase their speed and send a HidePopupMessage and PopupMessage.
            /// </summary>
            /// <param name="col"></param>
            public override void OnTriggerEnter2D(Collider2D col)
            {
                base.OnTriggerEnter2D(col);
                if (!col.gameObject.CompareTag("Player")) return;
                if(player == null) return;
                if (ActionReference == null)
                {
                    Debug.LogError("Action reference or button text is not assigned.");
                    return;
                }
                if (ReachedLastWaypoint)
                {
                    MessageSystem.MessageManager.SendImmediate(MessageChannels.UI, new HidePopupMessage(PopupType.Notification));
                    //MessageSystem.MessageManager.SendImmediate(MessageChannels.Dialogue, new DialogueMessage(DialogueState.Start,ReferenceState, CurrentDialogueGraph, CurrentDialogueInteraction, ID, player.ID));
                }
                else
                {
                    CurrentSpeed += IncreaseSpeedWhenReached;
                    MessageSystem.MessageManager.SendImmediate(MessageChannels.UI, new HidePopupMessage(PopupType.Notification));
                    MessageSystem.MessageManager.SendImmediate(MessageChannels.UI, new PopupMessage("Get away from me!!", PopupType.Notification, PopupPosition.Middle, 1f));
                    PatrolSpeed = currentSpeed;
                    
                    // Check if there are any depths left to trigger dialogues for
                    if (DialogueTriggerDepths.Count <= 0) return;
                    // Check if the current DepthInMeters is greater than the first depth in DialogueTriggerDepths
                    if (!(DepthInMeters > DialogueTriggerDepths[0])) return;
                    //MessageSystem.MessageManager.SendImmediate(MessageChannels.Dialogue, new DialogueMessage(DialogueState.Start, ReferenceState, CurrentDialogueGraph, CurrentDialogueInteraction, ID, player.ID));
    
                    // Update LastDepthTriggered to the current depth threshold
                    LastDepthTriggered = DialogueTriggerDepths[0];
    
                    // Remove the first entry from DialogueTriggerDepths so we don't check it again
                    DialogueTriggerDepths.RemoveAt(0);
                }
                
            }
    
            /// <summary>
            ///  Handles the LevelResetMessage functionality and resets the CurrentSpeed, PatrolSpeed, DialogueTriggerDepths, and ReferenceState.
            /// </summary>
            /// <param name="message"></param>
            public override void LevelResetMessageHandler(MessageSystem.IMessageEnvelope message)
            {
                if(!message.Message<LevelResetMessage>().HasValue) return;
                base.LevelResetMessageHandler(message);
                CurrentSpeed = MinSpeed;
                PatrolSpeed = CurrentSpeed;
                DialogueTriggerDepths = new List<float>(dialogueTriggerDepthsInitial);
                ReferenceState = string.Empty;
            }
    
            /// <summary>
            ///  When the player leaves the trigger, the diver will send a HidePopupMessage.
            /// </summary>
            /// <param name="col"></param>
            public override void OnTriggerExit2D(Collider2D col)
            {
                base.OnTriggerExit2D(col);
                if (!col.gameObject.CompareTag("Player")) return;
                MessageSystem.MessageManager.SendImmediate(MessageChannels.UI, new HidePopupMessage(PopupType.Notification));
            }
            
            public virtual void LoadData<T>(T loadedGameData) where T : class, IGameDataContent, new()
            {
                if (loadedGameData is OceansCallGameData oceansCallGameData)
                {
                    if(!oceansCallGameData.ID.Equals(ID)) return;
                    transform.position = oceansCallGameData.Position;
                    ReachedPoints = oceansCallGameData.ReachedPositions;
                    ReferenceState = oceansCallGameData.ReferenceState;
                    currentSpeed = oceansCallGameData.MoveSpeed;
                    LastDepthTriggered = oceansCallGameData.LastDepthTriggered;
                    DialogueTriggerDepths = oceansCallGameData.DialogueTriggerDepths;
                    ReachedLastWaypoint = oceansCallGameData.ReachedLastWaypoint;
                    SetClosestPatrolIndex();
                    TimeSinceLastWaypoint = 0f;
                }
            }

            public virtual void SaveData<T>(T savedGameData) where T : class, IGameDataContent, new()
            {
                if (savedGameData is OceansCallGameData oceansCallGameData)
                {
                    oceansCallGameData.ID =ID;
                    oceansCallGameData.Name = Name;
                    oceansCallGameData.Position = transform.position;
                    oceansCallGameData.MoveSpeed = currentSpeed;
                    oceansCallGameData.ReachedPositions = ReachedPoints;
                    oceansCallGameData.ReferenceState = ReferenceState;
                    oceansCallGameData.LastDepthTriggered = LastDepthTriggered;
                    oceansCallGameData.DialogueTriggerDepths = DialogueTriggerDepths;
                    oceansCallGameData.ReachedLastWaypoint = ReachedLastWaypoint;
                    oceansCallGameData.LastUpdated = DateTime.Now;
                }
            }

            public override void OnDestroy()
            {
                base.OnDestroy();
                OnInactive();
            }
        }
}
