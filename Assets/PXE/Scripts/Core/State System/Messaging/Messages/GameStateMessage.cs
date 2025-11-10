namespace PXE.Core.State_System.Messaging.Messages
{
    public struct GameStateMessage
    {
        public GameState State { get; }

/// <summary>
/// Executes the GameStateMessage method.
/// Handles the GameStateMessage functionality.
/// </summary>
        public GameStateMessage(GameState state)
        {
            State = state;
        }
    }
}