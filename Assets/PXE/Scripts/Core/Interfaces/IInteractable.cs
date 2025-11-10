using PXE.Core.SerializableTypes;
using UnityEngine;

namespace PXE.Core.Interfaces
{
    /// <summary>
    ///  This interface represents the interactable.
    /// </summary>
    public interface IInteractable : IGameObject
    {
        /// <summary>
        /// The target ID that the actor is interacting with.
        /// </summary>
        SerializableGuid TargetID { get; set; }
        
        GameObject TargetGameObject { get; set; }

        
        /// <summary>
        /// Indicates whether the actor is currently interacting with a target.
        /// </summary>
        bool IsInteracting { get; set; }
        
        /// <summary>
        /// Allows the actor to interact with the current target GameObject.
        /// </summary>
        void Interact();
    }
}