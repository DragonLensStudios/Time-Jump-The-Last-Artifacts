using PXE.Core.SerializableTypes;
using UnityEngine;

namespace PXE.Core.Messaging.Messages
{
    public struct TransformPositionMessage
    {
        public SerializableGuid ID { get; }
        public Vector3 Position { get;}
        public Quaternion Rotation { get; }

        public TransformPositionMessage(SerializableGuid id, Vector3 position, Quaternion rotation)
        {
            ID = id;
            Position = position;
            Rotation = rotation;
        }
    }
}