using System;
using System.Linq;
using System.Reflection;
using UnityEditor;

namespace PXE.Core.Editor.Extensions.SerializedPropertyExtensions
{
    public static class SerializedPropertyExtensions
    {
        
        // This extension will now work for both auto-properties and explicit backing fields.
        public static SerializedProperty FindBackingProperty(this SerializedObject serializedObject, string propertyName)
        {
            // First, try to find the property directly by its name.
            var property = serializedObject.FindProperty(propertyName);
            if (property != null)
                return property;

            // Next, try to find the backing field using the default auto-property naming convention.
            string backingFieldName = $"<{propertyName}>k__BackingField";
            SerializedProperty backingField = serializedObject.FindProperty(backingFieldName);
        
            if (backingField != null)
                return backingField;

            // If the backing field is still not found, use reflection to find the actual field.
            Type targetType = serializedObject.targetObject.GetType();
            FieldInfo fieldInfo = targetType.GetField(propertyName, BindingFlags.NonPublic | BindingFlags.Instance);
            if (fieldInfo != null)
            {
                backingField = serializedObject.FindProperty(fieldInfo.Name);
                if (backingField != null)
                    return backingField;
            }

            // If the backing field is still not found, look for fields that are private and contain the property name.
            FieldInfo[] privateFields = targetType.GetFields(BindingFlags.NonPublic | BindingFlags.Instance);
            fieldInfo = privateFields.FirstOrDefault(f => f.Name.Contains(propertyName, StringComparison.OrdinalIgnoreCase));
            if (fieldInfo != null)
            {
                backingField = serializedObject.FindProperty(fieldInfo.Name);
                if (backingField != null)
                    return backingField;
            }

            // Finally, try to find explicitly named backing fields if a convention is used (like prefixing with an underscore).
            backingFieldName = $"_{propertyName}";
            backingField = serializedObject.FindProperty(backingFieldName);
            if (backingField != null)
                return backingField;

            return null;
        }
        
        public static string GetBackingFieldName(this SerializedObject serializedObject, string propertyName)
        {
            // Try to find the explicit backing field first
            var explicitBackingField = serializedObject.FindProperty(propertyName.ToLower());
            if (explicitBackingField != null)
                return explicitBackingField.propertyPath;

            // If not found, try the auto-implemented naming convention
            return GetBackingFieldName(serializedObject.GetType(), propertyName);
        }
        
        public static SerializedProperty FindBackingPropertyRelative(this SerializedProperty serializedProperty, string propertyName)
        {
            // Try to find the explicit backing field first
            var explicitBackingField = serializedProperty.FindPropertyRelative(propertyName.ToLower());
            if (explicitBackingField != null)
                return explicitBackingField;

            // If not found, try the auto-implemented naming convention
            var backingFieldName = $"<{propertyName}>k__BackingField"; 
            return serializedProperty.FindPropertyRelative(backingFieldName);
        }

        public static SerializedProperty FindBackingField(this SerializedObject serializedObject, string propertyName)
        {
            var backingFieldName = GetBackingFieldName(serializedObject.GetType(), propertyName);
            return backingFieldName != null ? serializedObject.FindProperty(backingFieldName) : null;
        }
        
        private static string GetBackingFieldName(System.Type type, string propertyName)
        {
            var propertyInfo = type.GetProperty(propertyName);
            if (propertyInfo == null) return null;

            var getterMethod = propertyInfo.GetGetMethod(nonPublic: true);
            if (getterMethod == null) return null;

            var body = getterMethod.GetMethodBody();
            if (body == null) return null;

            var il = body.GetILAsByteArray();
            if (il == null) return null;

            const byte ldFieldOpCode = 0x7B;  // OpCode for Ldfld

            for (int i = 0; i < il.Length; i++)
            {
                if (il[i] == ldFieldOpCode)
                {
                    var fieldToken = BitConverter.ToInt32(il, i + 1);
                    var field = type.Module.ResolveField(fieldToken);
                    return field?.Name;
                }
            }

            return null;
        }

    }
}