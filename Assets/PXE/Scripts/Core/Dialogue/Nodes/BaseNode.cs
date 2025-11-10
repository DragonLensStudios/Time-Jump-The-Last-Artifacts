using PXE.Core.Dialogue.Interfaces;
using PXE.Core.Dialogue.xNode.Scripts;
using PXE.Core.Enums;
using PXE.Core.Interfaces;
using PXE.Core.Messaging;
using PXE.Core.Messaging.Messages;
using PXE.Core.SerializableTypes;
using PXE.Core.Variables;
using UnityEngine;
using UnityEngine.Scripting;

namespace PXE.Core.Dialogue.Nodes
{
    /// <summary>
    /// This abstract class is the base node for all other nodes in the dialogue graph.
    /// </summary>
    [Preserve]
    public abstract class BaseNode : Node
    {
        public VariablesObject Variables;
        public SerializableGuid SourceID;
        public SerializableGuid TargetID;
        public GameObject SourceGameobject;
        public GameObject TargetGameobject;
        public IDialogueActor SourceActor;
        public IDialogueActor TargetActor;


        /// <summary>
        /// Returns the string representation of the node.
        /// </summary>
        /// <returns>The string representation of the node.</returns>
        public virtual string GetString()
        {
            return null;
        }

        /// <summary>
        /// Returns the sprite associated with the node.
        /// </summary>
        /// <returns>The sprite associated with the node.</returns>
        public virtual Sprite GetPortrait()
        {
            return null;
        }

        /// <summary>
        /// Returns the type of the node as a string.
        /// </summary>
        /// <returns>The type of the node as a string.</returns>
        public virtual string GetNodeType()
        {
            return GetType().Name;
        }

        /// <summary>
        /// Assigns the method <see cref="DialogueMessageHandler"/> to the <see cref="ActorController.OnDialogueInteractAction"/> event.
        /// </summary>
        protected override void OnEnable()
        {
            base.OnEnable();
            MessageSystem.MessageManager.RegisterForChannel<GameObjectInteractionMessage>(MessageChannels.Gameplay, GameObjectInteractionMessageHandler);
        }

        /// <summary>
        /// Unassigns the method <see cref="DialogueMessageHandler"/> from the <see cref="ActorController.OnDialogueInteractAction"/> event.
        /// </summary>
        protected virtual void OnDisable()
        {
            MessageSystem.MessageManager.UnregisterForChannel<GameObjectInteractionMessage>(MessageChannels.Gameplay, GameObjectInteractionMessageHandler);
        }
        
        
        protected virtual void GameObjectInteractionMessageHandler(MessageSystem.IMessageEnvelope message)
        {
            if (!message.Message<GameObjectInteractionMessage>().HasValue) return;
            var data = message.Message<GameObjectInteractionMessage>().GetValueOrDefault();
            var sourceActor = data.SourceGameObject.GetComponent<IDialogueActor>();
            var targetActor = data.TargetGameObject.GetComponent<IDialogueActor>();

            if (sourceActor != null)
            {
                SourceActor = sourceActor;
                SourceID = sourceActor.ID;
            }

            if (targetActor != null)
            {
                TargetActor = targetActor;
                TargetID = targetActor.ID;
            }

            SourceGameobject = data.SourceGameObject;
            TargetGameobject = data.TargetGameObject;
        }
    }
}