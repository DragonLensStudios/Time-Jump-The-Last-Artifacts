using PXE.Core.Objects;
using UnityEditor;
using UnityEngine;

namespace PXE.Core.Editor.Objects
{
    [InitializeOnLoad]
    public class ObjectControllerPrefabPostProcessor : AssetPostprocessor
    {
        public static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
        {
            foreach (string assetPath in importedAssets)
            {
                if (assetPath.EndsWith(".prefab"))
                {
                    GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(assetPath);
                    if (prefab.TryGetComponent<ObjectController>(out var objectController))
                    {
                        UpdatePrefabAssetName(prefab, objectController.Name);
                    }
                }
            }
        }

        public static void UpdatePrefabAssetName(GameObject prefab, string newName)
        {
            if (string.IsNullOrWhiteSpace(newName)) return;

            var path = AssetDatabase.GetAssetPath(prefab);
            if (string.IsNullOrEmpty(path)) return;

            // Remove " (Inactive)" or " (Active)" from the name
            newName = newName.Replace(" (Inactive)", "").Replace(" (Active)", "");

            // Rename the asset
            AssetDatabase.RenameAsset(path, newName);
            AssetDatabase.SaveAssets();
        }
    }
}