using System;
using PXE.Core.SerializableTypes;
using UnityEditor;
using UnityEngine;

namespace PXE.Core.Editor.Drawers
{
    /// <summary>
    /// Represents the SerializableGuidDrawer.
    /// The SerializableGuidDrawer class provides functionality related to serializableguiddrawer management.
    /// This class contains methods and properties that assist in managing and processing serializableguiddrawer related tasks.
    /// </summary>
    [CustomPropertyDrawer(typeof(SerializableGuid), true)]
    public class SerializableGuidDrawer : PropertyDrawer
    {
        /// <summary>
        /// Executes the OnGUI method.
        /// Handles the OnGUI functionality.
        /// </summary>
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            // Draw label
            position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);

            // Calculate rect for the guid string
            var guidRect = new Rect(position.x, position.y, position.width - 100, position.height);

            // Calculate rect for the button
            var buttonRect = new Rect(position.x + position.width - 95, position.y, 90, position.height);

            // Get the guidString property
            var guidStringProp = property.FindPropertyRelative("guidString");

            // Draw the guid string field
            EditorGUI.PropertyField(guidRect, guidStringProp, GUIContent.none);

            // Draw the button
            if (GUI.Button(buttonRect, "New ID"))
            {
                guidStringProp.stringValue = Guid.NewGuid().ToString();
                GUIUtility.keyboardControl = 0;
            }

            EditorGUI.EndProperty();
        }

        /// <summary>
        /// Manually draws a SerializableGuid.
        /// </summary>
        public static SerializableGuid DrawSerializableGuid(SerializableGuid guid, GUIContent label)
        {
            Rect position = GUILayoutUtility.GetRect(0, EditorGUIUtility.singleLineHeight);
            // Draw label
            position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);

            // Calculate rect for the guid string
            var guidRect = new Rect(position.x, position.y, position.width - 100, position.height);

            // Calculate rect for the button
            var buttonRect = new Rect(position.x + position.width - 95, position.y, 90, position.height);

            // Draw the guid string field
            string guidString = guid.ToString();
            guidString = EditorGUI.TextField(guidRect, guidString);

            // Draw the button
            if (GUI.Button(buttonRect, "New ID"))
            {
                guid = SerializableGuid.CreateNew;
                guidString = guid.ToString();
                GUIUtility.keyboardControl = 0;
            }

            // Apply the edited string back to the guid
            if (Guid.TryParse(guidString, out var newGuid))
            {
                guid = new SerializableGuid(newGuid);
            }

            return guid;
        }
    }
}
