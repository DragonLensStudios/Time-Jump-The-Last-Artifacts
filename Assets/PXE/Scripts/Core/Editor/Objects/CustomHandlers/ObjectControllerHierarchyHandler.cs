using PXE.Core.Objects;
using UnityEditor;
using UnityEngine;

namespace PXE.Core.Editor.Objects
{
    [InitializeOnLoad]
    public static class ObjectControllerHierarchyHandler
    {
        static ObjectControllerHierarchyHandler()
        {
            EditorApplication.hierarchyChanged += OnHierarchyChanged;
        }

        private static void OnHierarchyChanged()
        {
            foreach (GameObject go in GameObject.FindObjectsOfType<GameObject>())
            {
                if (go.TryGetComponent<ObjectController>(out var objectController))
                {
                    // Ensure the base GameObject name does not contain the suffix
                    string baseName = go.name.Replace(" (Inactive)", "").Replace(" (Active)", "");
                
                    if (objectController.Name != baseName)
                    {
                        objectController.Name = baseName;
                        objectController.SetGameObjectName();
                        EditorUtility.SetDirty(objectController);
                    }
                }
            }
        }
    }
}