using System.Collections.Generic;
using PXE.Core.SerializableTypes;
using UnityEngine;

namespace PXE.Core.Messaging.Messages
{
    public struct PatrolPointReachedMessage
    {
        public SerializableGuid ID { get; }
        public Vector3 ReachedPosition { get; }
        
        public List<Vector3> ReachedPositions { get; }
        public bool ReachedFinalPosition { get; }
        public bool IsLooping { get; }

        public PatrolPointReachedMessage(SerializableGuid id, Vector3 reachedPosition, List<Vector3> reachedPositions, bool reachedFinalPosition, bool isLooping)
        {
            ID = id;
            ReachedPosition = reachedPosition;
            ReachedPositions = reachedPositions;
            ReachedFinalPosition = reachedFinalPosition;
            IsLooping = isLooping;
        }
    }
}