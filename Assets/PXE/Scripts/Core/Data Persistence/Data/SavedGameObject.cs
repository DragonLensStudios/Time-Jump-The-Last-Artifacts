using System;
using PXE.Core.SerializableTypes;
using UnityEngine;

namespace PXE.Core.Data_Persistence.Data
{
    /// <summary>
    /// Represents the SavedGameObject.
    /// The SavedGameObject class provides functionality related to savedgameobject management.
    /// This class contains methods and properties that assist in managing and processing savedgameobject related tasks.
    /// </summary>
    [Serializable]
    public class SavedGameObject : IEquatable<SavedGameObject>
    {
        [field: SerializeField] public string CurrentLevel { get; set; }
        [field: SerializeField] public SerializableGuid CurrentLevelID { get; set; }
        [field: SerializeField] public SerializableGuid PrefabID { get; set; }
        [field: SerializeField] public SerializableGuid InstanceID { get; set; }
        [field: SerializeField] public string ObjectName { get; set; }
        [field: SerializeField] public TransformData TransformData { get; set; }
        [field: SerializeField] public bool IsActive { get; set; }
        

/// <summary>
/// Executes the SavedGameObject method.
/// Handles the SavedGameObject functionality.
/// </summary>
        public SavedGameObject()
        {
            CurrentLevel = string.Empty;
            PrefabID = new SerializableGuid(Guid.Empty);
            CurrentLevelID = new SerializableGuid(Guid.Empty);
            InstanceID = new SerializableGuid(Guid.NewGuid());
            ObjectName = string.Empty;
            TransformData = new TransformData(Vector3.zero, Quaternion.identity, Vector3.one, new SerializableGuid(Guid.Empty));
            IsActive = true;
        }

/// <summary>
/// Executes the Equals method.
/// Handles the Equals functionality.
/// </summary>
        public bool Equals(SavedGameObject other)
        {
            if (other == null) return false;
            return CurrentLevel == other.CurrentLevel &&
                   CurrentLevelID == other.CurrentLevelID && 
                   PrefabID.Equals(other.PrefabID) &&
                   ObjectName == other.ObjectName &&
                   InstanceID == other.InstanceID &&
                   TransformData.Equals(other.TransformData); // Assumes TransformData has an appropriate Equals method
        }

/// <summary>
/// Executes the Equals method.
/// Handles the Equals functionality.
/// </summary>
        public override bool Equals(object obj)
        {
            if (obj is SavedGameObject otherGameObject)
            {
                return Equals(otherGameObject);
            }
            return false;
        }

/// <summary>
/// Executes the GetHashCode method.
/// Handles the GetHashCode functionality.
/// </summary>
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}