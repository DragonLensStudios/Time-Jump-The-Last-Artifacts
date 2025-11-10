using PXE.Core.Dialogue.Editor.xNode.Scripts;
using PXE.Core.Dialogue.Nodes.CustomNodes;
using UnityEngine;

namespace PXE.Scripts.Core.Dialogue.Editor.Nodes
{
    [NodeEditor.CustomNodeEditor(typeof(AudioNode))]
    public class AudioNodeEditor : NodeEditor
    {
        public override void OnBodyGUI()
        {
            serializedObject.Update();

            var segment = serializedObject.targetObject as AudioNode;
            if (segment != null)
            {
                NodeEditorGUILayout.PortField(segment.GetPort(nameof(segment.Input)));
                NodeEditorGUILayout.PortField(segment.GetPort(nameof(segment.ExitTrue)));
                NodeEditorGUILayout.PortField(segment.GetPort(nameof(segment.ExitFalse)));

                GUILayout.Label("Audio");
                NodeEditorGUILayout.PropertyField(serializedObject.FindProperty(nameof(segment.Audio)), GUIContent.none);
                
                GUILayout.Label("Audio Channel");
                NodeEditorGUILayout.PropertyField(serializedObject.FindProperty(nameof(segment.AudioChannel)), GUIContent.none);
                
                GUILayout.Label("Audio Operation");
                NodeEditorGUILayout.PropertyField(serializedObject.FindProperty(nameof(segment.AudioOperation)), GUIContent.none);
                
                GUILayout.Label("Use Random Volume");
                NodeEditorGUILayout.PropertyField(serializedObject.FindProperty(nameof(segment.UseRandomVolume)), GUIContent.none);

                GUILayout.Label("Use Random Pitch");
                NodeEditorGUILayout.PropertyField(serializedObject.FindProperty(nameof(segment.UseRandomPitch)), GUIContent.none);

                GUILayout.Label(segment.UseRandomVolume ? "Random Audio Volume Range" : "Audio Volume");
                NodeEditorGUILayout.PropertyField(serializedObject.FindProperty(nameof(segment.AudioVolume)), GUIContent.none);

                GUILayout.Label(segment.UseRandomPitch ? "Random Audio Pitch Range" : "Audio Pitch");
                NodeEditorGUILayout.PropertyField(serializedObject.FindProperty(nameof(segment.AudioPitch)), GUIContent.none);
                
            }
            serializedObject.ApplyModifiedProperties();
        }

    }
}