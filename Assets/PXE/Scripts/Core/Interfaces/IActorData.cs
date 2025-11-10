namespace PXE.Core.Interfaces
{
    /// <summary>
    /// This interface represents the actor data.
    /// </summary>
    public interface IActorData : IInteractable
    {
        /// <summary>
        /// Indicates whether movement is disabled for the actor.
        /// </summary>
        bool IsDisabled { get; set; }
    }
}