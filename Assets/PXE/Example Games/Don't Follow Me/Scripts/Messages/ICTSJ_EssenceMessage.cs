using PXE.Core.Enums;
using PXE.Core.SerializableTypes;

namespace PXE.Example_Games.Don_t_Follow_Me.Scripts.Messages
{
    public struct ICTSJ_EssenceMessage
    {
        public SerializableGuid SourceID { get; }
        public int EssenceValue { get; }
        public int MaxEssencevalue { get; }
        public Operator Operator { get; }

        public ICTSJ_EssenceMessage(SerializableGuid sourceID, int essenceValue, int maxEssencevalue, Operator @operator)
        {
            SourceID = sourceID;
            EssenceValue = essenceValue;
            MaxEssencevalue = maxEssencevalue;
            Operator = @operator;
        }
    }
}