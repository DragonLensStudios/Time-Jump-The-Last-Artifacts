using PXE.Core.Dialogue;
using PXE.Core.Dialogue.Interaction;
using PXE.Core.Dialogue.Messaging.Messages;
using PXE.Core.Enums;
using PXE.Core.Messaging;
using PXE.Core.Objects;
using UnityEngine;

namespace PXE.Example_Games.Beyond_The_Infernal_Door.Scripts.Dialogue
{
    public class BGJDialogueObject : ObjectController
    {
        public Sprite CurrentPortrait { get; set; }

        [field: Tooltip("The reference state.")]
        [field: SerializeField] public virtual string ReferenceState { get; set; }
        [field: Tooltip("The current dialogue graph.")]
        [field: SerializeField] public virtual DialogueGraph CurrentDialogueGraph { get; set; }
        
        [field: Tooltip("The current dialogue interaction.")]
        [field: SerializeField] public virtual DialogueInteraction CurrentDialogueInteraction { get; set; }
    
        [field: Tooltip("Does the object open dialogue on start?")]
        [field: SerializeField] public virtual bool OpenDialogueOnStart { get; set; }

        public override void Start()
        {
            base.Start();
            if (OpenDialogueOnStart)
            {
                MessageSystem.MessageManager.SendImmediate(MessageChannels.Dialogue, new DialogueMessage(DialogueState.Start, string.Empty, CurrentDialogueGraph, CurrentDialogueInteraction, ID));
            }
        }

        public override void OnActive()
        {
            base.OnActive();
            MessageSystem.MessageManager.RegisterForChannel<DialogueMessage>(MessageChannels.Dialogue, DialogueMessageHandler);
        }

        public override void OnInactive()
        {
            base.OnInactive();
            MessageSystem.MessageManager.UnregisterForChannel<DialogueMessage>(MessageChannels.Dialogue, DialogueMessageHandler);
        }

        private void DialogueMessageHandler(MessageSystem.IMessageEnvelope message)
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
                    break;
                case DialogueState.End:
                    break;
            }
        }
    }
}
