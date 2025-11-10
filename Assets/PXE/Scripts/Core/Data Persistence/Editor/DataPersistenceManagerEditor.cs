// using System.Collections.Generic;
// using PXE.Data_Persistence;
// using PXE.Data_Persistence.Data;
// using PXE.Editor.Extensions.SerializedPropertyExtensions;
// using PXE.Managers;
// using UnityEditor;
// using UnityEngine;
//
// namespace PXE.Editor.Managers
// {
//     [CustomEditor(typeof(DataPersistenceManager))]
//     public class DataPersistenceManagerEditor : UnityEditor.Editor
//     {
//         protected DataPersistenceManager Manager => target as DataPersistenceManager;
//         
//         public override void OnInspectorGUI()
//         {
//             serializedObject.Update();
//
//             // Display object selector for valid handlers
//             var newHandler = EditorGUILayout.ObjectField("Data Handler", Manager.GameDataHandler as BasicGameDataHandlerObject, typeof(BasicGameDataHandlerObject), false) as BasicGameDataHandlerObject;
//             if (newHandler != null && newHandler != Manager.GameDataHandler)
//             {
//                 Undo.RecordObject(Manager, "Change Data Handler");
//                 Manager.GameDataHandler = newHandler;
//                 EditorUtility.SetDirty(Manager); // Mark the manager as dirty
//                 Debug.Log("Setting DataHandler to: " + newHandler);
//             }
//
//             base.OnInspectorGUI();
//
//             serializedObject.ApplyModifiedProperties();
//         }
//
//     }
// }