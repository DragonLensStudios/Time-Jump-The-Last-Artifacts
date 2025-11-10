using PXE.Core.Dialogue.Editor.xNode.Scripts;
using PXE.Core.Dialogue.Nodes.CustomNodes;
using UnityEngine;

namespace PXE.Scripts.Core.Dialogue.Editor.Nodes
{
    /// <summary>
    /// Custom editor for the ReferenceStateNode node.
    /// </summary>
    [NodeEditor.CustomNodeEditor(typeof(ReferenceStateNode))]
    public class ReferenceStateNodeEditor : NodeEditor
    {
        /// <summary>
        /// Override of the OnBodyGUI method to draw the custom GUI for the ReferenceStateNode node.
        /// </summary>
        public override void OnBodyGUI()
        {
            serializedObject.Update();

            var segment = serializedObject.targetObject as ReferenceStateNode;
            if (segment != null)
            {
                NodeEditorGUILayout.PortField(segment.GetPort(nameof(segment.Input)));
                NodeEditorGUILayout.PortField(segment.GetPort(nameof(segment.ExitTrue)));
                GUILayout.Label("Reference State");
                NodeEditorGUILayout.PropertyField(serializedObject.FindProperty(nameof(segment.ReferenceState)), GUIContent.none);
                GUILayout.Label("Override Source ID");
                NodeEditorGUILayout.PropertyField(serializedObject.FindProperty(nameof(segment.OverrideSourceID)), GUIContent.none);
                GUILayout.Label("Override Target ID");
                NodeEditorGUILayout.PropertyField(serializedObject.FindProperty(nameof(segment.OverrideTargetID)), GUIContent.none);
                if (segment.OverrideSourceID)
                {
                    GUILayout.Label("Source ID");
                    NodeEditorGUILayout.PropertyField(serializedObject.FindProperty(nameof(segment.OverrideSourceIDValue)), GUIContent.none);
                }

                if (segment.OverrideTargetID)
                {
                    GUILayout.Label("Target ID");
                    NodeEditorGUILayout.PropertyField(serializedObject.FindProperty(nameof(segment.OverrideTargetIDValue)), GUIContent.none);
                }
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
}
