using PXE.Core.SerializableTypes;

namespace PXE.Example_Games.Oceans_Call.Messages
{
    public struct CollectibleMessage
    {
        public SerializableGuid ID { get; }
        public string Name { get; }
        
        public CollectibleMessage(SerializableGuid id, string name)
        {
            ID = id;
            Name = name;
        }
    }
}