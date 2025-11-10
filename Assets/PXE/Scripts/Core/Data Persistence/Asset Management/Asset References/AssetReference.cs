using UnityEngine;
using UnityEngine.AddressableAssets;

namespace PXE.Core.Data_Persistence.Asset_Management.Asset_References
{
    /// <summary>
    /// Represents the AssetReference.
    /// The AssetReference class provides functionality related to assetreference management.
    /// This class contains methods and properties that assist in managing and processing assetreference related tasks.
    /// </summary>
    [System.Serializable]
    public class AssetReference : AssetReferenceT<Object>
    {
        /// <summary>
        /// Executes the AssetReference method.
        /// Handles the AssetReference functionality.
        /// </summary>
        public AssetReference(string guid) : base(guid) { }
    }
}