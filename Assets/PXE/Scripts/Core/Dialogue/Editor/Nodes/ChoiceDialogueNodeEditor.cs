using PXE.Core.Dialogue.Editor.xNode.Scripts;
using PXE.Core.Dialogue.Nodes.CustomNodes;
using PXE.Core.Dialogue.xNode.Scripts;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace PXE.Scripts.Core.Dialogue.Editor.Nodes
{
    /// <summary>
    /// Custom editor for the ChoiceDialogueNode.
    /// </summary>
    [NodeEditor.CustomNodeEditor(typeof(ChoiceDialogueNode))]
    public class ChoiceDialogueNodeEditor : NodeEditor
    {
        /// <summary>
        /// Override of the OnBodyGUI method to draw the custom GUI for the ChoiceDialogueNode.
        /// </summary>
        public override void OnBodyGUI()
        {
            serializedObject.Update();

            var segment = serializedObject.targetObject as ChoiceDialogueNode;
            if (segment != null)
            {
                NodeEditorGUILayout.PortField(segment.GetPort(nameof(segment.Input)));

                GUILayout.Label("Variables");
                NodeEditorGUILayout.PropertyField(serializedObject.FindProperty(nameof(segment.Variables)), GUIContent.none);
                
                // Radio buttons for actor choice
                bool newUseSourceActor = GUILayout.Toggle(segment.UseSourceActor, "Use Source Actor");
                if (newUseSourceActor != segment.UseSourceActor)
                {
                    segment.UseSourceActor = newUseSourceActor;
                    if (segment.UseSourceActor) // If the button was just selected
                    {
                        segment.UseTargetActor = false;
                        segment.Manual = false;
                    }
                }

                bool newUseTargetActor = GUILayout.Toggle(segment.UseTargetActor, "Use Target Actor");
                if (newUseTargetActor != segment.UseTargetActor)
                {
                    segment.UseTargetActor = newUseTargetActor;
                    if (segment.UseTargetActor) // If the button was just selected
                    {
                        segment.UseSourceActor = false;
                        segment.Manual = false;
                    }
                }

                bool newManual = GUILayout.Toggle(segment.Manual, "Manual");
                if (newManual != segment.Manual)
                {
                    segment.Manual = newManual;
                    if (segment.Manual) // If the button was just selected
                    {
                        segment.UseSourceActor = false;
                        segment.UseTargetActor = false;
                    }
                }

                if (segment.Manual)
                {
                    GUILayout.Label("Display Actor Name");
                    NodeEditorGUILayout.PropertyField(serializedObject.FindProperty(nameof(segment.ActorName)), GUIContent.none);

                    GUILayout.Label("Actor Portrait");
                    NodeEditorGUILayout.PropertyField(serializedObject.FindProperty(nameof(segment.Portrait)), GUIContent.none);
                }
                
                if (!segment.Manual)
                {
                    bool newOverridePortrait = GUILayout.Toggle(segment.OverridePortrait, "Override Portrait");
                    segment.OverridePortrait = newOverridePortrait;
                }

                if (segment.OverridePortrait && !segment.Manual)
                {
                    GUILayout.Label("Actor Portrait");
                    NodeEditorGUILayout.PropertyField(serializedObject.FindProperty(nameof(segment.Portrait)), GUIContent.none);
                }
                
                GUILayout.Label("Dialogue Text");
                // Use EditorGUILayout.TextArea to create an expandable text area
                var dialogueTextProp = serializedObject.FindProperty(nameof(segment.DialogueText));

                // Define a GUIStyle that forces text to wrap
                GUIStyle textAreaStyle = new GUIStyle(EditorStyles.textArea);
                textAreaStyle.wordWrap = true;

                dialogueTextProp.stringValue = EditorGUILayout.TextArea(dialogueTextProp.stringValue, textAreaStyle); // Removed the GUILayout.Height directive to let the TextArea auto-size
                
                NodeEditorGUILayout.DynamicPortList(
                    nameof(segment.Answers), // field name
                    typeof(string), // field type
                    serializedObject, // serializable object
                    NodePort.IO.Input, // new port i/o
                    Node.ConnectionType.Override, // new port connection type
                    Node.TypeConstraint.None,
                    OnCreateReorderableList); // onCreate override. This is where the magic happens

                foreach (NodePort dynamicPort in target.DynamicPorts)
                {
                    if (NodeEditorGUILayout.IsDynamicPortListPort(dynamicPort)) continue;
                    NodeEditorGUILayout.PortField(dynamicPort);
                }
            }
            serializedObject.ApplyModifiedProperties();
        }

        void OnCreateReorderableList(ReorderableList list)
        {
            list.elementHeightCallback = (int index) => 
            {
                var segment = serializedObject.targetObject as ChoiceDialogueNode;
                var content = new GUIContent(segment.Answers[index]);
                var style = new GUIStyle(EditorStyles.textArea) { wordWrap = true };
                var width = EditorGUIUtility.currentViewWidth - 50; // approximate width of the list
                var height = style.CalcHeight(content, width);
                return Mathf.Max(height, 20); // Ensure a minimum height of 60
            };

            // Override drawHeaderCallback to display node's name instead
            list.drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
            {
                var segment = serializedObject.targetObject as ChoiceDialogueNode;

                NodePort port = segment.GetPort($"{nameof(segment.Answers)} " + index);

                // Define a GUIStyle that forces text to wrap
                GUIStyle textAreaStyle = new GUIStyle(EditorStyles.textArea);
                textAreaStyle.wordWrap = true;

                segment.Answers[index] = EditorGUI.TextArea(rect, segment.Answers[index], textAreaStyle);

                if (port != null)
                {
                    Vector2 pos = rect.position + (port.IsOutput ? new Vector2(rect.width + 6, 0) : new Vector2(-36, 0));
                    NodeEditorGUILayout.PortField(pos, port);
                }
            };
        }
    }
}
