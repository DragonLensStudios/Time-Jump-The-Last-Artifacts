using UnityEngine;
using UnityEngine.AddressableAssets;

namespace PXE.Core.Data_Persistence.Asset_Management.Asset_References
{
    /// <summary>
    /// Represents the AssetReferenceGameObject.
    /// The AssetReferenceGameObject class provides functionality related to assetreferencegameobject management.
    /// This class contains methods and properties that assist in managing and processing assetreferencegameobject related tasks.
    /// </summary>
    [System.Serializable]
    public class AssetReferenceGameObject : AssetReferenceT<GameObject>
    {
        /// <summary>
        /// Executes the AssetReferenceGameObject method.
        /// Handles the AssetReferenceGameObject functionality.
        /// </summary>
        public AssetReferenceGameObject(string guid) : base(guid) { }
    }
}