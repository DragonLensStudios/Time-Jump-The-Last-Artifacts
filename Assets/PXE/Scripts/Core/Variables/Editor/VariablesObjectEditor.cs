using System;
using System.Linq;
using PXE.Core.Variables;
using UnityEditor;
using UnityEngine;

namespace PXE.Scripts.Core.Variables.Editor
{
    [CustomEditor(typeof(VariablesObject))]
    public class VariablesObjectEditor : UnityEditor.Editor
    {
        private VariablesObject _target;
        private void OnEnable()
        {
            _target = (VariablesObject)target;
        }

        private void DrawVariables<T>(VariableContainer<T> variableContainer, string addButtonLabel, ref string variableToAdd, T defaultValue) where T : IComparable<T>, IEquatable<T>
        {
            var variables = variableContainer.Variables;

            for (int i = 0; i < variables.Count; i++)
            {
                EditorGUILayout.BeginHorizontal();
                variables[i] = variables[i].EditorField(defaultValue);

                if (GUILayout.Button("-", GUILayout.Width(20)))
                {
                    variables.RemoveAt(i);
                    i--;
                }

                EditorGUILayout.EndHorizontal();
            }

            EditorGUILayout.BeginHorizontal();
            variableToAdd = EditorGUILayout.TextField(variableToAdd);
            var addVariableButtonLayoutOptions = GUILayout.Width(EditorGUIUtility.labelWidth - 4);
            if (GUILayout.Button(addButtonLabel, addVariableButtonLayoutOptions))
            {
                string inputVariableLabel = variableToAdd.Trim();
                if (string.IsNullOrEmpty(inputVariableLabel))
                {
                    EditorUtility.DisplayDialog("Invalid Variable Name", "Variable name cannot be empty.", "OK");
                }
                else if (DoesVariableExist(inputVariableLabel))
                {
                    EditorUtility.DisplayDialog("Variable Already Exists", $"Unable to add the variable '{inputVariableLabel}' since a variable with this name already exists.", "OK");
                }
                else
                {
                    variables.Add(new Variable<T>(inputVariableLabel, defaultValue));
                    variableToAdd = string.Empty;
                }
            }
            EditorGUILayout.EndHorizontal();
        }

        private bool DoesVariableExist(string variableName)
        {
            return _target.IntVariables.Variables.Any(v => v.Name == variableName) ||
                   _target.LongVariables.Variables.Any(v => v.Name == variableName) ||
                   _target.ShortVariables.Variables.Any(v => v.Name == variableName) ||
                   _target.DoubleVariables.Variables.Any(v => v.Name == variableName) ||
                   _target.DecimalVariables.Variables.Any(v => v.Name == variableName) ||
                   _target.FloatVariables.Variables.Any(v => v.Name == variableName) ||
                   _target.BoolVariables.Variables.Any(v => v.Name == variableName) ||
                   _target.StringVariables.Variables.Any(v => v.Name == variableName) ||
                   _target.Vector2Variables.Variables.Any(v => v.Name == variableName) ||
                   _target.Vector3Variables.Variables.Any(v => v.Name == variableName) ||
                   _target.DateTimeVariables.Variables.Any(v => v.Name == variableName);
        }

        private string _intVariableToAdd = string.Empty;
        private void DrawIntVariables()
        {
            DrawVariables(_target.IntVariables, "Add Int Variable", ref _intVariableToAdd, 0);
        }

        private string _longVariableToAdd = string.Empty;
        private void DrawLongVariables()
        {
            DrawVariables(_target.LongVariables, "Add Long Variable", ref _longVariableToAdd, 0L);
        }

        private string _shortVariableToAdd = string.Empty;
        private void DrawShortVariables()
        {
            DrawVariables(_target.ShortVariables, "Add Short Variable", ref _shortVariableToAdd, (short)0);
        }

        private string _doubleVariableToAdd = string.Empty;
        private void DrawDoubleVariables()
        {
            DrawVariables(_target.DoubleVariables, "Add Double Variable", ref _doubleVariableToAdd, 0.0);
        }

        private string _decimalVariableToAdd = string.Empty;
        private void DrawDecimalVariables()
        {
            DrawVariables(_target.DecimalVariables, "Add Decimal Variable", ref _decimalVariableToAdd, 0m);
        }

        private string _floatVariableToAdd = string.Empty;
        private void DrawFloatVariables()
        {
            DrawVariables(_target.FloatVariables, "Add Float Variable", ref _floatVariableToAdd, 0f);
        }

        private string _boolVariableToAdd = string.Empty;
        private void DrawBoolVariables()
        {
            DrawVariables(_target.BoolVariables, "Add Bool Variable", ref _boolVariableToAdd, false);
        }

        private string _stringVariableToAdd = string.Empty;
        private void DrawStringVariables()
        {
            DrawVariables(_target.StringVariables, "Add String Variable", ref _stringVariableToAdd, string.Empty);
        }
    
        private string _vector2VariableToAdd = string.Empty;
        private void DrawVector2Variables()
        {
            DrawVariables(_target.Vector2Variables, "Add Vector 2 Variable", ref _vector2VariableToAdd, new ComparableVector2(Vector2.zero));
        }

        private string _vector3VariableToAdd = string.Empty;
        private void DrawVector3Variables()
        {
            DrawVariables(_target.Vector3Variables, "Add Vector 3 Variable", ref _vector3VariableToAdd, new ComparableVector3(Vector3.zero));
        }

        private string _dateTimeVariableToAdd = string.Empty;
        private void DrawDateTimeVariables()
        {
            DrawVariables(_target.DateTimeVariables, "Add DateTime Variable", ref _dateTimeVariableToAdd, DateTime.Now);
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUI.BeginChangeCheck(); // Begin tracking changes

            DrawIntVariables();
            DrawLongVariables();
            DrawShortVariables();
            DrawDoubleVariables();
            DrawDecimalVariables();
            DrawFloatVariables();
            DrawBoolVariables();
            DrawStringVariables();
            DrawVector2Variables();
            DrawVector3Variables();
            DrawDateTimeVariables();

            if (EditorGUI.EndChangeCheck()) // Check if any changes were made
            {
                serializedObject.ApplyModifiedProperties(); // Apply the modified properties

                EditorUtility.SetDirty(target); // Save the ScriptableObject
            }

            serializedObject.ApplyModifiedProperties();
        }





    }
}