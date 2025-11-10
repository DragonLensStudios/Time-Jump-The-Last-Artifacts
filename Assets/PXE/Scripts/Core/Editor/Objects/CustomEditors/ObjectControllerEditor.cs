using PXE.Core.Editor.Drawers;
using PXE.Core.Editor.Extensions.SerializedPropertyExtensions;
using PXE.Core.Enums;
using PXE.Core.Objects;
using PXE.Core.SerializableTypes;
using UnityEditor;
using UnityEngine;

namespace PXE.Core.Editor.Objects
{
    [CustomEditor(typeof(ObjectController), true)]
    [CanEditMultipleObjects]
    public class ObjectControllerEditor : UnityEditor.Editor
    {
        public ObjectController objController => target as ObjectController;
        protected bool ExpandIdentityFoldout { get; set; } = true;
        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            if (objController == null) return;
            DrawObjectControllerInspector();
            serializedObject.ApplyModifiedProperties();

            // Draw default inspector for subclasses only
            if (target.GetType() != typeof(ObjectController))
            {
                DrawDefaultInspector();
            }
            
            serializedObject.ApplyModifiedProperties();
        }

        public virtual void DrawObjectControllerInspector()
        {
            GUILayoutOption[] labelOptions = { GUILayout.Width(EditorGUIUtility.currentViewWidth - 120) };
            var primaryComp = objController.gameObject.GetComponent<ObjectController>();
            if (primaryComp == objController)
            {
                EditorGUILayout.BeginHorizontal();
                ExpandIdentityFoldout = EditorGUILayout.Foldout(ExpandIdentityFoldout, "Object Identity");
                EditorGUI.BeginDisabledGroup(true);
                EditorGUI.BeginDisabledGroup(true);
                EditorGUILayout.Toggle("Is Initialized", objController.IsInitialized,GUILayout.ExpandWidth(true));
                EditorGUI.EndDisabledGroup();
                EditorGUI.EndDisabledGroup();

                EditorGUILayout.EndHorizontal();
                if (ExpandIdentityFoldout)
                {
                    EditorGUILayout.BeginHorizontal();
                // Draw the Name field
                objController.Name = EditorGUILayout.TextField("Name", objController.Name, labelOptions);

                if (string.IsNullOrWhiteSpace(objController.Name))
                {
                    if (GUILayout.Button("Get Name" + (targets.Length != 1 ? "s" : ""), GUILayout.Width(90)))
                    {
                        foreach (var obj in targets)
                        {
                            if (obj is not ObjectController objectController) continue;
                            objectController.SetGameObjectName();
                            EditorUtility.SetDirty(objectController);
                        }
                        GUIUtility.keyboardControl = 0;
                    }
                }
                else
                {
                    if (GUILayout.Button("Set Name" + (targets.Length != 1 ? "s" : ""), GUILayout.Width(90)))
                    {
                        foreach (var obj in targets)
                        {
                            if (obj is not ObjectController objectController) continue;
                            objectController.SetGameObjectName();
                            EditorUtility.SetDirty(objectController);
                        }
                        GUIUtility.keyboardControl = 0;
                    }
                }
                
                EditorGUILayout.EndHorizontal();

                // Draw the ObjectController-specific inspector
                 // EditorGUILayout.PropertyField(serializedObject.FindBackingProperty(nameof(objController.IsManualID)));
                 objController.IsManualID = EditorGUILayout.Toggle("Manual ID", objController.IsManualID);
                if (objController.IsManualID)
                {
                    objController.ID = SerializableGuidDrawer.DrawSerializableGuid(objController.ID, new GUIContent("ID"));

                    // drawer.OnGUI(GUILayoutUtility.GetRect([position]), serializedObject.FindBackingProperty(nameof(objController.ID)), new GUIContent("ID"));
                    // objController.ID = EditorGUILayout.PropertyField(serializedObject.FindBackingProperty(nameof(objController.ID)));
                }
                else
                {
                    EditorGUILayout.BeginHorizontal();

                    // Adjust the width of the LabelField to fit the GUID
                    EditorGUILayout.LabelField("ID", objController.ID.ToString(), labelOptions);

                    if (!SerializableGuid.IsEmpty(objController.ID))
                    {
                        if (GUILayout.Button("Copy ID", GUILayout.Width(90)))
                        {
                            EditorGUIUtility.systemCopyBuffer = objController.ID.ToString();
                        }
                    }

                    EditorGUILayout.EndHorizontal();
                }


                EditorGUILayout.BeginHorizontal();

                if (GUILayout.Button("Generate New ID"))
                {
                    foreach (var obj in targets)
                    {
                        if (obj is ObjectController objectController)
                        {
                            objectController.ID = SerializableGuid.CreateNew;
                        }
                    }
                }
                
                EditorGUILayout.EndHorizontal();
                }
                EditorGUILayout.Space();
                // Assuming that all objects have the same state
                bool areAllObjectsActive = AreAllSelectedObjectsActive();

                EditorGUILayout.BeginHorizontal();

                // Toggle to show current state and get new state
                bool newIsActive = EditorGUILayout.Toggle("Is Active", areAllObjectsActive);
                EditorGUI.BeginChangeCheck();
                var gameObjectActiveType = (ActiveType)EditorGUILayout.EnumPopup(GUIContent.none, objController.ActiveType);
                if (EditorGUI.EndChangeCheck())
                {
                    foreach (var obj in targets)
                    {
                        if (obj is ObjectController objectController)
                        {
                            objectController.ActiveType = gameObjectActiveType;
                            EditorUtility.SetDirty(objectController);
                        }
                    }
                }            
                EditorGUILayout.EndHorizontal();
                EditorGUILayout.BeginHorizontal();

                objController.ComponentActiveType = (ChildrenActiveType)EditorGUILayout.EnumPopup("Component Active Type", objController.ComponentActiveType);

                EditorGUILayout.EndHorizontal();

                // If the state has changed, update all selected objects
                if (newIsActive != areAllObjectsActive)
                {
                    foreach (var obj in targets)
                    {
                        if (obj is ObjectController objectController)
                        {
                            objectController.SetObjectActive(newIsActive);
                            EditorUtility.SetDirty(objectController);
                        }
                    }
                }
            }
        }

        public virtual bool AreAllSelectedObjectsActive()
        {
            foreach (var obj in targets)
            {
                if (obj is ObjectController objectController && !objectController.IsActive)
                {
                    return false; // Found at least one inactive object
                }
            }
            return true; // All objects are active
        }
    }
}
