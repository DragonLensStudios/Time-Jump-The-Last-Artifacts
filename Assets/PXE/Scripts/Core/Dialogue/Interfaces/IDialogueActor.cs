using PXE.Core.Dialogue.Interaction;
using PXE.Core.Interfaces;
using UnityEngine;

namespace PXE.Core.Dialogue.Interfaces
{
    /// <summary>
    ///  This interface represents the dialogue.
    /// </summary>
    public interface IDialogueActor : IActorData
    {
        /// <summary>
        /// The portrait sprite of the actor.
        /// </summary>
        Sprite CurrentPortrait { get; set; }
        
        /// <summary>
        ///  The reference state of the dialogue.
        /// </summary>
        string ReferenceState { get; set; }
        
        /// <summary>
        ///  The current dialogue graph.
        /// </summary>
        DialogueGraph CurrentDialogueGraph { get; set; }
        
        /// <summary>
        ///  The current dialogue interaction.
        /// </summary>
        DialogueInteraction CurrentDialogueInteraction { get; set; }
    }
}