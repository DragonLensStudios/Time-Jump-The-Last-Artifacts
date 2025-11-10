using PXE.Core.Data_Persistence.Asset_Management.Asset_References;
using PXE.Core.ScriptableObjects;
using PXE.Core.SerializableTypes;
using UnityEngine;

namespace PXE.Core.Data_Persistence.Asset_Management
{
    /// <summary>
    /// Represents the PrefabReferences.
    /// The PrefabReferences class provides functionality related to prefabreferences management.
    /// This class contains methods and properties that assist in managing and processing prefabreferences related tasks.
    /// </summary>
    [CreateAssetMenu(fileName = "Prefab Asset Management", menuName = "PXE/Asset Management/Prefab References")]
    public class PrefabReferences : ScriptableObjectController
    {
        [field: Tooltip("The list of prefab asset references.")]
        [field: SerializeField] public SerializableDictionary<string, AssetReferenceGameObject> PrefabAssetReferences { get; set; }
    }
}