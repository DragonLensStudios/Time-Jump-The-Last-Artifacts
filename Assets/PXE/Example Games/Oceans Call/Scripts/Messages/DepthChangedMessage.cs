#nullable enable
namespace PXE.Example_Games.Oceans_Call.Messages
{
    public struct DepthChangedMessage
    {
        public float Depth { get; }

        public DepthChangedMessage(float depth)
        {
            Depth = depth;
        }
    }
}