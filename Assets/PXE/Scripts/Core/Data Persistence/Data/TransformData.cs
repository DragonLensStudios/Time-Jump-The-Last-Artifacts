using System;
using PXE.Core.SerializableTypes;
using PXE.Core.Utilities.GameObject;
using UnityEngine;

namespace PXE.Core.Data_Persistence.Data
{
    /// <summary>
    /// Represents the TransformData.
    /// The TransformData class provides functionality related to transformdata management.
    /// This class contains methods and properties that assist in managing and processing transformdata related tasks.
    /// </summary>
    [Serializable]
    public class TransformData
    {
        [field: SerializeField] public virtual Vector3 LocalPosition { get; set; }
        [field: SerializeField] public virtual Quaternion LocalRotation { get; set; }
        [field: SerializeField] public virtual Vector3 LocalScale { get; set; }
        [field: SerializeField] public virtual SerializableGuid ParentID { get; set; }

        /// <summary>
        /// Executes the TransformData method.
        /// Handles the TransformData functionality.
        /// </summary>
        public TransformData(Vector3 localPosition, Quaternion localRotation, Vector3 localScale, SerializableGuid parentID)
        {
            LocalPosition = localPosition;
            LocalRotation = localRotation;
            LocalScale = localScale;
            ParentID = parentID;
        }

        public Transform SetTransform(Transform transform)
        {
            transform.localPosition = LocalPosition;
            transform.localRotation = LocalRotation;
            transform.localScale = LocalScale;
            if (ParentID == null) return transform;
            if (ParentID.Guid.Equals(Guid.Empty)) return transform;
            var parent = GameObjectUtilities.GetGameObjectByID(ParentID);
            if (ParentID != null)
            {
                transform.parent = parent.transform;
            }
            return transform;
        }
    }
}