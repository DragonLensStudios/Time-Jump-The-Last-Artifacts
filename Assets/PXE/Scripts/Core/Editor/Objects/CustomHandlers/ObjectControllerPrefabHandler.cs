using PXE.Core.Objects;
using UnityEditor;
using UnityEngine;

namespace PXE.Core.Editor.Objects
{
    [InitializeOnLoad]
    public static class ObjectControllerPrefabHandler
    {
        static ObjectControllerPrefabHandler()
        {
            UnityEditor.SceneManagement.PrefabStage.prefabSaving += OnPrefabSaving;
        }

        public static void OnPrefabSaving(GameObject instance)
        {
            // Handle the update for the root instance
            UpdateObjectController(instance);

            // Handle nested prefabs
            foreach (Transform child in instance.transform)
            {
                UpdateNestedObjectControllers(child);
            }
        }

        public static void UpdateNestedObjectControllers(Transform parent)
        {
            foreach (Transform child in parent)
            {
                // Check if the child is a nested prefab instance
                if (PrefabUtility.IsPartOfPrefabInstance(child))
                {
                    UpdateObjectController(child.gameObject);
                    UpdateNestedObjectControllers(child);
                }
            }
        }

        public static void UpdateObjectController(GameObject go)
        {
            // Ensure the GameObject has an ObjectController component
            if (go.TryGetComponent<ObjectController>(out var objectController))
            {
                // If this is a prefab, ensure it retains its ID and handles nested prefabs
                var prefabAsset = PrefabUtility.GetCorrespondingObjectFromOriginalSource(go);
                if (prefabAsset != null && prefabAsset.TryGetComponent<ObjectController>(out var prefabController))
                {
                    if (objectController.IsManualID)
                    {
                        // Retain the current ID and ParentID values if ManualID is checked
                        objectController.ID = prefabController.ID;
                    }

                    // Update the GameObject name before saving the prefab
                    UpdateGameObjectName(go, objectController.Name);
                }
                else
                {
                    // Ensure that new prefabs also have correct names
                    UpdateGameObjectName(go, objectController.Name);
                }
            }
        }

        public static void UpdateGameObjectName(GameObject go, string newName)
        {
            if (string.IsNullOrWhiteSpace(newName)) return;

            // Remove " (Inactive)" or " (Active)" from the name
            newName = newName.Replace(" (Inactive)", "").Replace(" (Active)", "");

            // Rename the GameObject
            go.name = newName;
        }
    }
}