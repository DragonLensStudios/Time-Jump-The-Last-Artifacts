using PXE.Core.Objects;
using PXE.Core.SerializableTypes;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using Object = UnityEngine.Object;

namespace PXE.Core.Editor.Objects
{
    public static class ObjectControllerTool
    {
        [MenuItem("GameObject/Create Object Controller", false, -1)] // Above Create Empty
        public static void CreateObjectController()
        {
            GameObject newObject = new GameObject("New Object Controller");
            
            var controller = newObject.AddComponent<ObjectController>();
            controller.Name = newObject.name;
            controller.ID = SerializableGuid.CreateNew;

            controller.SetObjectActive(true);
            
            GameObject selectedObject = Selection.activeGameObject;  // Get the currently selected GameObject

            if (selectedObject != null)
            {
                newObject.transform.SetParent(selectedObject.transform, false);  // Set the newObject as child
            }

            var currentPrefabStage = PrefabStageUtility.GetCurrentPrefabStage();
            if (currentPrefabStage != null) // In prefab mode
            {
                if (selectedObject == null)
                {
                    // Move newObject under the prefab root
                    newObject.transform.SetParent(currentPrefabStage.prefabContentsRoot.transform, false);
                }

                // Save changes
                AssetDatabase.SaveAssets();
            }

            // Set the created GameObject as the selected object
            Selection.activeObject = newObject;
        }

        [MenuItem("GameObject/UI/Create UI Object Controller", false, -1)] // Top of the UI list
        public static void CreateUIObjectController()
        {
            GameObject newUIObject = new GameObject("New UI Object Controller", typeof(RectTransform));
            
            var controller = newUIObject.AddComponent<ObjectController>();
            controller.Name = newUIObject.name;
            controller.ID = SerializableGuid.CreateNew;
            controller.SetObjectActive(true);
            
            GameObject selectedObject = Selection.activeGameObject;  // Get the currently selected GameObject

            if (selectedObject != null)
            {
                newUIObject.transform.SetParent(selectedObject.transform, false);  // Set the newUIObject as child
            }

            var currentPrefabStage = PrefabStageUtility.GetCurrentPrefabStage();
            if (currentPrefabStage != null) // In prefab mode
            {
                if (selectedObject == null)
                {
                    newUIObject.transform.SetParent(currentPrefabStage.prefabContentsRoot.transform, false);
                }

                // Save changes
                AssetDatabase.SaveAssets();
            }

            // Set the created UI GameObject as the selected object
            Selection.activeObject = newUIObject;
        }


        [MenuItem("GameObject/Add Object Controller", false, 0)] // For GameObjects in the scene.
        [MenuItem("Assets/Add Object Controller", false, 2)] // For Prefab Assets in the project view.
        public static void AddObjectControllerToSelection()
        {
            foreach (Object obj in Selection.objects)
            {
                ProcessObject(obj);
            }
        }

        [MenuItem("GameObject/Add Object Controller", true)] // Validate for GameObjects in the scene.
        [MenuItem("Assets/Add Object Controller", true)] // Validate for Prefab Assets in the project view.
        public static bool CanAddObjectController()
        {
            foreach (Object obj in Selection.objects)
            {
                if (obj is GameObject || PrefabUtility.GetPrefabAssetType(obj) != PrefabAssetType.NotAPrefab)
                    return true;
            }
            return false;
        }

        public static void ProcessObject(Object obj)
        {
            GameObject gameObject = null;

            // Check if the selected object is an in-scene GameObject
            if (obj is GameObject)
            {
                gameObject = obj as GameObject;
            }
            // Check if the selected object is a prefab asset
            else if (PrefabUtility.GetPrefabAssetType(obj) != PrefabAssetType.NotAPrefab)
            {
                gameObject = PrefabUtility.InstantiatePrefab(obj as GameObject) as GameObject;
            }

            if (gameObject == null || gameObject.GetComponent<ObjectController>() != null) return;

            ObjectController controller = gameObject.AddComponent<ObjectController>();
            controller.Name = gameObject.name;
            controller.ID = SerializableGuid.CreateNew;
            controller.SetObjectActive(true);
            
            // If it's a prefab, apply changes
            if (PrefabUtility.GetPrefabAssetType(obj) == PrefabAssetType.NotAPrefab) return;
            PrefabUtility.SaveAsPrefabAsset(gameObject, AssetDatabase.GetAssetPath(obj));
            Object.DestroyImmediate(gameObject);
        }
    }
}
