using System.Collections.Generic;
using System.Linq;
using PXE.Core.Dialogue;
using PXE.Core.Dialogue.Interaction;
using UnityEditor;
using UnityEngine;

namespace PXE.Scripts.Core.Dialogue.Editor
{
    [CustomEditor(typeof(DialogueManagerObject))]
    public class DialogueManagerObjectEditor : UnityEditor.Editor
    {
        protected DialogueManagerObject manager;
        
        protected virtual void OnEnable()
        {
            if (manager == null)
            {
                manager = (DialogueManagerObject)target;
            }
        }

        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
            
            if (manager == null)
            {
                EditorGUILayout.HelpBox("Unable to access DialogueManager properties.", MessageType.Error);
                return;
            }
            
            if (GUILayout.Button("Add Interactions"))
            {
                AddInteractionsFromSubfolders();
            }
        }

        private void AddInteractionsFromSubfolders()
        {
            // Dynamically locate all Resources/Dialogue folders
            string[] allPaths = AssetDatabase.GetAllAssetPaths();
            string[] searchPaths = allPaths.Where(p => p.EndsWith("/Resources/Dialogue")).ToArray();

            List<DialogueInteraction> interactions = new List<DialogueInteraction>();

            foreach (var searchPath in searchPaths)
            {
                string[] guids = AssetDatabase.FindAssets("t:DialogueInteraction", new[] { searchPath });

                foreach (string guid in guids)
                {
                    string assetPath = AssetDatabase.GUIDToAssetPath(guid);
                    DialogueInteraction interaction = AssetDatabase.LoadAssetAtPath<DialogueInteraction>(assetPath);
                    if (interaction != null)
                    {
                        interactions.Add(interaction);
                    }
                }
            }

            manager.Interactions = interactions;
        }
    }
}
