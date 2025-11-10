using System.Collections.Generic;
using PXE.Core.Interfaces;
using PXE.Core.SerializableTypes;
using UnityEngine;

namespace PXE.Core.Objects
{
    /// <summary>
    /// Represents the ObjectList.
    /// The ObjectList class provides functionality related to objectlist management.
    /// This class contains methods and properties that assist in managing and processing objectlist related tasks.
    /// </summary>
    [System.Serializable]
    public class ObjectList : IGameObjectIdentity
    {
        [field: Tooltip("The id of the objectlist.")]
        [field: SerializeField] public SerializableGuid ID { get; set; }

        [field: Tooltip("When Enabled, The ID will be able to be manually set.")]
        [field: SerializeField] public bool IsManualID { get; set; }

        [field: Tooltip("The name of the objectlist.")]
        [field: SerializeField] public string Name { get; set; }
        
        [field: Tooltip("The level id of the objectlist.")]
        [field: SerializeField] public SerializableGuid LevelID { get; set; }
        
        [field: Tooltip("The game objects of the objectlist.")]
        [field: SerializeField] public List<ObjectController> ObjectControllers { get; set; }
        
        public IEnumerable<T> GetExistingIDs<T>() where T : IID
        {
            return ObjectControllers as IEnumerable<T>;
        }
    }
}