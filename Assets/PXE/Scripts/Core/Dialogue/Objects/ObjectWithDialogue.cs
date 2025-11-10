using System;
using PXE.Core.Dialogue.Interaction;
using PXE.Core.Dialogue.Messaging.Messages;
using PXE.Core.Enums;
using PXE.Core.Extensions.GameObjectExtensions;
using PXE.Core.Interfaces;
using PXE.Core.Inventory.Items;
using PXE.Core.Levels.Messaging.Messages;
using PXE.Core.Messaging;
using PXE.Core.Messaging.Messages;
using PXE.Core.Objects;
using PXE.Core.Player.Managers;
using PXE.Core.SerializableTypes;
using UnityEngine;

namespace PXE.Core.Dialogue.Objects
{
    /// <summary>
    ///  Represents the ObjectWithDialogue.
    /// </summary>
    public class ObjectWithDialogue : ObjectController, IActorData
    {
        [field: Tooltip("The reference state.")]
        [field: SerializeField] public virtual string ReferenceState { get; set; }
        
        [field: Tooltip("The current dialogue graph.")]
        [field: SerializeField] public virtual DialogueGraph CurrentDialogueGraph { get; set; }
        
        [field: Tooltip("The current dialogue interaction.")]
        [field: SerializeField] public virtual DialogueInteraction CurrentDialogueInteraction { get; set; }
        
        [field: Tooltip("The target ID.")]
        [field: SerializeField] public virtual SerializableGuid TargetID { get; set; }
        
        [field: Tooltip("The target game object.")]
        [field: SerializeField] public virtual GameObject TargetGameObject { get; set; }
        
        [field: Tooltip("Is the object interacting?")]
        [field: SerializeField] public virtual bool IsInteracting { get; set; }
        
        [field: Tooltip("The current portrait.")]
        [field: SerializeField] public virtual Sprite CurrentPortrait { get; set; }
        
        [field: Tooltip("Does the object open dialogue on start?")]
        [field: SerializeField] public virtual bool OpenDialogueOnStart { get; set; }
        
        [field: Tooltip("Does the object open dialogue on trigger?")]
        [field: SerializeField] public virtual bool OpenDialogueOnTrigger { get; set; }
        
        [field: Tooltip("The gizmo color.")]
        [field: SerializeField] public virtual Color GizmoColor { get; set; }
        
        [field: Tooltip("The collider.")]
        [field: SerializeField] public virtual Collider2D Col { get; set; }
        
        [field: Tooltip("Is movement disabled?")]
        [field: SerializeField] public virtual bool IsDisabled { get; set; }
        
        [field: Tooltip("The inventory.")]
        [field: SerializeField] public virtual InventoryObject Inventory { get; set; }
        
        [field: Tooltip("Is the object triggered?")]
        [field: SerializeField] public virtual bool IsTriggered { get; set; }

        /// <summary>
        ///  Sets the collider.
        /// </summary>
        public override void Awake()
        {
            base.Awake();
            if (Col == null)
            {
                Col = GetComponent<Collider2D>();
            }
        }

        /// <summary>
        ///  This method registers the ObjectWithDialogue for the DialogueMessage message and the LevelResetMessage message.
        /// </summary>
        public override void OnActive()
        {
            base.OnActive();
            MessageSystem.MessageManager.RegisterForChannel<DialogueMessage>(MessageChannels.Dialogue, DialogueMessageHandler);
            MessageSystem.MessageManager.RegisterForChannel<LevelResetMessage>(MessageChannels.Level, LevelResetMessageHandler);
        }

        /// <summary>
        ///  This method unregisters the ObjectWithDialogue for the DialogueMessage message and the LevelResetMessage message.
        /// </summary>
        public override void OnInactive()
        {
            base.OnInactive();
            MessageSystem.MessageManager.UnregisterForChannel<DialogueMessage>(MessageChannels.Dialogue, DialogueMessageHandler);
            MessageSystem.MessageManager.UnregisterForChannel<LevelResetMessage>(MessageChannels.Level, LevelResetMessageHandler);
        }

        /// <summary>
        ///  This method starts the dialogue if the object is set to open dialogue on start.
        /// </summary>
        public override void Start()
        {
            base.Start();
            if (!OpenDialogueOnStart) return;
            //TODO: Remove dependency on PlayerManager
            TargetID = PlayerManager.Instance.Player.ID;
            TargetGameObject = PlayerManager.Instance.Player.gameObject;
            MessageSystem.MessageManager.SendImmediate(MessageChannels.Gameplay, new GameObjectInteractionMessage(gameObject, TargetGameObject));
            Interact();
        }

        /// <summary>
        ///  This method handles the interaction with the object.
        /// </summary>
        public virtual void Interact()
        {
            if (IsTriggered) return;
            if(TargetID == null || (TargetID != null && TargetID.Guid == Guid.Empty)) return;
            IsInteracting = true;
            MessageSystem.MessageManager.SendImmediate(MessageChannels.Dialogue, new DialogueMessage(DialogueState.Start, ReferenceState, CurrentDialogueGraph, CurrentDialogueInteraction, ID, TargetID));
            IsTriggered = true;
        }

        /// <summary>
        ///  This method handling the ON trigger enter 2D event which sends a message to interact with the object.
        /// </summary>
        /// <param name="other"></param>
        public virtual void OnTriggerEnter2D(Collider2D other)
        {
            if(!OpenDialogueOnTrigger) return;
            if(!other.CompareTag("Player")) return;
            TargetGameObject = other.gameObject;
            MessageSystem.MessageManager.SendImmediate(MessageChannels.Gameplay, new GameObjectInteractionMessage(gameObject, TargetGameObject));
            TargetID = TargetGameObject.GetObjectID();
            if(TargetID.Guid.Equals(Guid.Empty)) return;
            Interact();
        }

        /// <summary>
        ///  This method handling the ON trigger exit 2D event which sets the target game object to null.
        /// </summary>
        /// <param name="other"></param>
        public virtual void OnTriggerExit2D(Collider2D other)
        {
            if(!OpenDialogueOnTrigger) return;
            TargetGameObject = null;
        }

        /// <summary>
        ///  This method handles the dialogue message and sets the reference state and the interaction state based on the dialogue state and controls the movement of the player.
        /// </summary>
        /// <param name="message"></param>
        public virtual void DialogueMessageHandler(MessageSystem.IMessageEnvelope message)
        {
            if(!message.Message<DialogueMessage>().HasValue) return;
            var data = message.Message<DialogueMessage>().GetValueOrDefault();
            if(!ID.Equals(data.SourceID) && !ID.Equals(data.TargetID)) return;
            switch (data.State)
            {
                case DialogueState.SetReferenceState:
                    ReferenceState = data.ReferenceState;
                    break;
                case DialogueState.Start:
                    ReferenceState = data.ReferenceState;
                    IsInteracting = true;
                    IsDisabled = true;
                    break;
                case DialogueState.End:
                    IsInteracting = false;
                    IsDisabled = false;
                    break;
            }
            
        }

        /// <summary>
        ///  This method handles the level reset message and sets the object to not triggered and sets the reference state to empty.
        /// </summary>
        /// <param name="message"></param>
        public virtual void LevelResetMessageHandler(MessageSystem.IMessageEnvelope message)
        {
            if(!message.Message<LevelResetMessage>().HasValue) return;
            IsTriggered = false;
            ReferenceState = string.Empty;
        }

        /// <summary>
        /// This method draws the gizmos for the object.
        /// </summary>
        public virtual void OnDrawGizmos()
        {
            if (Col == null) return;
            if(!OpenDialogueOnTrigger) return;

            Gizmos.color = GizmoColor;

            // Draw the cube (it might look like just an outline in the editor)
            var bounds = Col.bounds;
            Gizmos.DrawCube(bounds.center, bounds.size);

            // Additionally, you can draw a wireframe representation around it 
            Gizmos.DrawWireCube(bounds.center, bounds.size);
        }

    }
}
