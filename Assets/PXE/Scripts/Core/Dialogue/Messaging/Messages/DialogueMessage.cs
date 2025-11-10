using PXE.Core.Dialogue.Interaction;
using PXE.Core.Enums;
using PXE.Core.SerializableTypes;

namespace PXE.Core.Dialogue.Messaging.Messages
{
    public struct DialogueMessage
    {
        public DialogueState State { get; }
        public SerializableGuid SourceID { get; }
        public SerializableGuid TargetID { get; }
        public string ReferenceState { get; }
        public DialogueGraph Graph { get; }
        public DialogueInteraction Interaction { get; }

        public DialogueMessage(DialogueState state, string referenceState = "", DialogueGraph graph = null, DialogueInteraction interaction = null, SerializableGuid sourceID = null, SerializableGuid targetID = null)
        {
            State = state;
            SourceID = sourceID;
            TargetID = targetID;
            ReferenceState = referenceState;
            Graph = graph;
            Interaction = interaction;
        }
    }
}