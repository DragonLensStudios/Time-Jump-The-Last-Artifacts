using System;
using PXE.Core.Dialogue.Editor.xNode.Scripts;
using PXE.Core.Dialogue.Nodes.CustomNodes;
using PXE.Core.Enums;
using PXE.Core.Variables;
using UnityEditor;
using UnityEngine;

namespace PXE.Scripts.Core.Dialogue.Editor.Nodes
{
    /// <summary>
    /// Custom editor for the VariableNode node.
    /// </summary>
    [NodeEditor.CustomNodeEditor(typeof(VariableNode))]
    public class VariableNodeEditor : NodeEditor
    {
        private const string InvalidValueLabel = "Invalid value";

        /// <summary>
        /// Override of the OnBodyGUI method to draw the custom GUI for the VariableNode node.
        /// </summary>
        public override void OnBodyGUI()
        {
            serializedObject.Update();

            var segment = serializedObject.targetObject as VariableNode;

            if (segment != null)
            {
                NodeEditorGUILayout.PortField(segment.GetPort(nameof(segment.Input)));
                NodeEditorGUILayout.PortField(segment.GetPort(nameof(segment.ExitTrue)));
                NodeEditorGUILayout.PortField(segment.GetPort(nameof(segment.ExitFalse)));
                GUILayout.Label("Variables");
                NodeEditorGUILayout.PropertyField(serializedObject.FindProperty(nameof(segment.Variables)), GUIContent.none);
                GUILayout.Label("Variable Name");
                NodeEditorGUILayout.PropertyField(serializedObject.FindProperty(nameof(segment.VariableName)), GUIContent.none);
                GUILayout.Label("Variable Type");
                NodeEditorGUILayout.PropertyField(serializedObject.FindProperty(nameof(segment.VariableType)), GUIContent.none);
                GUILayout.Label("Value");
                EditorGUI.BeginChangeCheck();

                // Depending on the variable type, draw the appropriate field
                switch (segment.VariableType)
                {
                    case VariableType.Int:
                        int intValue = EditorGUILayout.IntField("", (int)segment.VariableValue);
                        segment.VariableValue = intValue;
                        break;
                    case VariableType.Long:
                        long longValue = EditorGUILayout.LongField("", (long)segment.VariableValue);
                        segment.VariableValue = longValue;
                        break;
                    case VariableType.Short:
                        short shortValue = (short)EditorGUILayout.IntField("", (short)segment.VariableValue);
                        segment.VariableValue = shortValue;
                        break;
                    case VariableType.Double:
                        double doubleValue = EditorGUILayout.DoubleField("", (double)segment.VariableValue);
                        segment.VariableValue = doubleValue;
                        break;
                    case VariableType.Decimal:
                        decimal decimalValue = (decimal)EditorGUILayout.DoubleField("", (double)(decimal)segment.VariableValue);
                        segment.VariableValue = decimalValue;
                        break;
                    case VariableType.Float:
                        float floatValue = EditorGUILayout.FloatField("", (float)segment.VariableValue);
                        segment.VariableValue = floatValue;
                        break;
                    case VariableType.Bool:
                        bool boolValue = EditorGUILayout.Toggle("", (bool)segment.VariableValue);
                        segment.VariableValue = boolValue;
                        break;
                    case VariableType.String:
                        // Define a GUIStyle that forces text to wrap
                        GUIStyle textAreaStyle = new GUIStyle(EditorStyles.textArea);
                        textAreaStyle.wordWrap = true;
                        segment.VariableValue = EditorGUILayout.TextArea((string)segment.VariableValue, textAreaStyle); // Removed the GUILayout.Height directive to let the TextArea auto-size
                        break;
                    case VariableType.Vector2:
                        ComparableVector2 vector2Value = segment.VariableValue as ComparableVector2;
                        if (vector2Value == null)
                        {
                            vector2Value = new ComparableVector2(Vector2.zero);
                            segment.VariableValue = vector2Value;
                        }
                        vector2Value.Value = EditorGUILayout.Vector2Field("", vector2Value.Value);
                        segment.VariableValue = vector2Value;
                        break;
                    case VariableType.Vector3:
                        ComparableVector3 vector3Value = segment.VariableValue as ComparableVector3;
                        if (vector3Value == null)
                        {
                            vector3Value = new ComparableVector3(Vector3.zero);
                            segment.VariableValue = vector3Value;
                        }
                        vector3Value.Value = EditorGUILayout.Vector3Field("", vector3Value.Value);
                        segment.VariableValue = vector3Value;
                        break;
                    case VariableType.DateTime:
                        string dateTimeString = segment.VariableValue != null ? segment.VariableValue.ToString() : "";
                        dateTimeString = EditorGUILayout.TextField("", dateTimeString);
                        if (DateTime.TryParse(dateTimeString, out var dateTimeValue))
                        {
                            segment.VariableValue = dateTimeValue;
                        }
                        else
                        {
                            EditorGUILayout.LabelField(InvalidValueLabel);
                        }
                        break;
                }
            }

            if (EditorGUI.EndChangeCheck())
            {
                serializedObject.ApplyModifiedProperties();
            }

            GUILayout.Label("Operator");
            NodeEditorGUILayout.PropertyField(serializedObject.FindProperty(nameof(segment.OperatorType)), GUIContent.none);
            serializedObject.ApplyModifiedProperties();
        }
    }
}
