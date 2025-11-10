using PXE.Core.SerializableTypes;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace PXE.Core.Editor.Drawers
{
    /// <summary>
    /// Represents the SerializableDictionaryDrawer.
    /// The SerializableDictionaryDrawer class provides functionality related to serializabledictionarydrawer management.
    /// This class contains methods and properties that assist in managing and processing serializabledictionarydrawer related tasks.
    /// </summary>
    [CustomPropertyDrawer(typeof(SerializableDictionary<,>), true)]
    public class SerializableDictionaryDrawer : PropertyDrawer
    {
        private ReorderableList list;

/// <summary>
/// Executes the OnGUI method.
/// Handles the OnGUI functionality.
/// </summary>
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            var keys = property.FindPropertyRelative("keys");
            var values = property.FindPropertyRelative("values");

            if (list == null)
            {
                list = new ReorderableList(keys.serializedObject, keys, true, true, true, true);

                list.drawHeaderCallback += (Rect rect) =>
                {
                    EditorGUI.LabelField(rect, label);
                };

                list.drawElementCallback += (Rect rect, int index, bool isActive, bool isFocused) =>
                {
                    var key = keys.GetArrayElementAtIndex(index);
                    var value = values.GetArrayElementAtIndex(index);

                    rect.y += 2;
                    rect.height = EditorGUIUtility.singleLineHeight;

                    float half = rect.width * 0.5f;
                    var keyRect = new Rect(rect.x, rect.y, half, rect.height);
                    var valueRect = new Rect(rect.x + half, rect.y, half, rect.height);

                    EditorGUI.PropertyField(keyRect, key, GUIContent.none);
                    EditorGUI.PropertyField(valueRect, value, GUIContent.none);
                };

                list.onAddCallback += (ReorderableList l) =>
                {
                    int index = l.count;
                    keys.arraySize++;
                    values.arraySize++;
                    l.index = index;

                    var key = keys.GetArrayElementAtIndex(index);
                    var value = values.GetArrayElementAtIndex(index);

                    // Set default key value, ensure its uniqueness
                    key.stringValue = GenerateUniqueKey(keys);
                };

                list.onRemoveCallback += (ReorderableList l) =>
                {
                    keys.DeleteArrayElementAtIndex(l.index);
                    values.DeleteArrayElementAtIndex(l.index);
                    property.serializedObject.ApplyModifiedProperties();

                    // Sync the dictionary with the updated lists
                    var targetObject = property.serializedObject.targetObject;
                    var dictionary = fieldInfo.GetValue(targetObject);
                    var syncMethod = fieldInfo.FieldType.GetMethod("SyncDictionaryFromLists");
                    syncMethod.Invoke(dictionary, null);
                };
            }

            list.DoList(position);

            EditorGUI.EndProperty();
        }

/// <summary>
/// Executes the GetPropertyHeight method.
/// Handles the GetPropertyHeight functionality.
/// </summary>
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            if (list == null)
                return EditorGUIUtility.singleLineHeight * 2;

            return list.GetHeight();
        }

        private string GenerateUniqueKey(SerializedProperty keys)
        {
            int counter = 0;
            string baseKey = "Key";
            string proposedKey = baseKey;

            while (IsKeyPresent(proposedKey, keys))
            {
                counter++;
                proposedKey = baseKey + counter;
            }

            return proposedKey;
        }

        private bool IsKeyPresent(string proposedKey, SerializedProperty keys)
        {
            for (int i = 0; i < keys.arraySize; i++)
            {
                if (keys.GetArrayElementAtIndex(i).stringValue == proposedKey)
                {
                    return true;
                }
            }

            return false;
        }
    }
}
