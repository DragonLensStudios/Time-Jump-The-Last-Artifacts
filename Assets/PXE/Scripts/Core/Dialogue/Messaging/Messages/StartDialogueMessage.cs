using PXE.Core.Dialogue.Interaction;
using PXE.Core.Enums;
using PXE.Core.SerializableTypes;

namespace PXE.Core.Dialogue.Messaging.Messages
{
    public struct StartDialogueMessage
    {
        public string DialogueID { get; }
        public DialogueGraph Graph { get; }

        public StartDialogueMessage(string dialogueID, DialogueGraph graph)
        {
            DialogueID = dialogueID;
            Graph = graph;
        }
        
    }
}