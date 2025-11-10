using PXE.Core.SerializableTypes;

namespace PXE.Example_Games.Don_t_Follow_Me.Scripts.Messages
{
    public struct ICTSJ_PhaseMessage
    {
        public SerializableGuid SourceId { get; }
        public bool IsPhasing { get; }
        
        public ICTSJ_PhaseMessage(SerializableGuid sourceId, bool isPhasing)
        {
            SourceId = sourceId;
            IsPhasing = isPhasing;
        }
        
    }
}