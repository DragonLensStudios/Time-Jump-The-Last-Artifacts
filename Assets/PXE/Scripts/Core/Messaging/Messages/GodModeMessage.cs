namespace PXE.Core.Messaging.Messages
{
    public struct GodModeMessage
    {
        public bool GodMode { get; }

        public GodModeMessage(bool godMode)
        {
            GodMode = godMode;
        }
    }
}