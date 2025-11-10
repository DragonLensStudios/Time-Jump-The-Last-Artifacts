using PXE.Core.Data_Persistence.Data;
using PXE.Core.SerializableTypes;

namespace PXE.Core.Spawning.Messaging.Messages
{
    public struct SpawnPrefabMessage
    {
        public string PrefabKey { get; }
        public string Name { get; }
        public TransformData Transform { get; }
        public SerializableGuid ObjectID { get;}
        
/// <summary>
/// Executes the SpawnPrefabMessage method.
/// Handles the SpawnPrefabMessage functionality.
/// </summary>
        public SpawnPrefabMessage(string prefabKey, string name, TransformData transform, SerializableGuid objectID = null)
        {
            PrefabKey = prefabKey;
            Name = name;
            Transform = transform;
            ObjectID = objectID;
        }
    }
}