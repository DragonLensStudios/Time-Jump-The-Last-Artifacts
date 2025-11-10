using PXE.Core.SerializableTypes;

namespace PXE.Core.Messaging.Messages
{
    public struct TargetDamageMessage
    {
        public SerializableGuid ID { get; }
        public int Damage { get; }

        public TargetDamageMessage(SerializableGuid id, int damage)
        {
            ID = id;
            Damage = damage;
        }
    }
}