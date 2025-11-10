using PXE.Core.Dialogue.Editor.xNode.Scripts;
using PXE.Core.Dialogue.Nodes.CustomNodes;
using PXE.Core.Enums;
using UnityEditor;
using UnityEngine;

namespace PXE.Scripts.Core.Dialogue.Editor.Nodes
{
    [NodeEditor.CustomNodeEditor(typeof(MessageSenderNode))]
    public class MessageSenderNodeEditor : NodeEditor
    {
        public override void OnBodyGUI()
        {
            serializedObject.Update();

            var segment = serializedObject.targetObject as MessageSenderNode;
            if (segment != null)
            {
                NodeEditorGUILayout.PortField(segment.GetPort(nameof(segment.Input)));
                NodeEditorGUILayout.PortField(segment.GetPort(nameof(segment.Exit)));

                GUILayout.Label("Message Channel");
                NodeEditorGUILayout.PropertyField(serializedObject.FindProperty(nameof(segment.Channel)), GUIContent.none);
                
                GUILayout.Label("Message Config");
                NodeEditorGUILayout.PropertyField(serializedObject.FindProperty(nameof(segment.MessageConfig)), GUIContent.none);
                
                GUILayout.Label("Send Action");
                segment.SendAction = (MessageSendAction)EditorGUILayout.EnumPopup("", segment.SendAction);
            }
            serializedObject.ApplyModifiedProperties();
        }
    }
}