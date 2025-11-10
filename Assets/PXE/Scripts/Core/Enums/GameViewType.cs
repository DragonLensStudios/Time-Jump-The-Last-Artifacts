namespace PXE.Core.Enums
{
    public enum GameViewType
    {
        FirstPerson,  // Player sees the game from the viewpoint of the character.
        ThirdPerson,  // Player sees the game from behind or slightly above the character.
        TopDown,      // Player views the game from directly above the action.
        SideView,     // Classic side-scrolling perspective.
        Isometric,    // Uses a form of parallel projection to create a pseudo-3D effect.
        BirdEye,      // Similar to TopDown but usually at a slight angle, providing more depth.
        VR            // Virtual Reality, providing a 360-degree view around the player.
    }
}