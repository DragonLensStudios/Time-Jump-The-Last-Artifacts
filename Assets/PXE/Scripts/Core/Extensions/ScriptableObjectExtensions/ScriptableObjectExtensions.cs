using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Newtonsoft.Json;
using UnityEngine;

namespace PXE.Core.Extensions.ScriptableObjectExtensions
{
    public static class ScriptableObjectExtensions
    {
        public static string SerializeToJson(this ScriptableObject obj)
        {
            var fields = GetSerializableFields(obj.GetType())
                .ToDictionary(field => field.Name, field => SerializeFieldOrObject(field.GetValue(obj)));

            return JsonConvert.SerializeObject(fields, Formatting.Indented);
        }
        
        
        public static void DeserializeFromJson<T>(this T obj, string json) where T : ScriptableObject
        {
            var fields = JsonConvert.DeserializeObject<Dictionary<string, object>>(json);
            var allFields = GetSerializableFields(obj.GetType());

            foreach (var field in allFields)
            {
                if (fields.TryGetValue(field.Name, out var fieldValue))
                {
                    field.SetValue(obj, DeserializeField(field, fieldValue));
                }
            }
        }

        private static object DeserializeField(FieldInfo field, object value)
        {
            if (typeof(ScriptableObject).IsAssignableFrom(field.FieldType))
            {
                return DeserializeScriptableObject(field.FieldType, value.ToString());
            }

            if (IsEnumerableOfScriptableObject(field.FieldType))
            {
                var itemType = field.FieldType.GetGenericArguments()[0];
                var itemList = ((IEnumerable)value).Cast<string>().Select(itemJson => DeserializeScriptableObject(itemType, itemJson)).ToList();
                return ConvertList(itemList, field.FieldType);
            }

            if (field.FieldType.IsPrimitive || field.FieldType == typeof(string))
            {
                return Convert.ChangeType(value, field.FieldType);
            }

            // Deserialize complex object fields
            return JsonConvert.DeserializeObject(value.ToString(), field.FieldType);
        }

        private static ScriptableObject DeserializeScriptableObject(Type type, string json)
        {
            var method = typeof(ScriptableObjectExtensions).GetMethod(nameof(DeserializeFromJson), BindingFlags.Public | BindingFlags.Static);
            var generic = method.MakeGenericMethod(type);
            return (ScriptableObject)generic.Invoke(null, new object[] { ScriptableObject.CreateInstance(type), json });
        }

        private static IEnumerable<FieldInfo> GetSerializableFields(Type type)
        {
            var allFields = type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

            return allFields.Where(field =>
                !field.IsStatic &&
                (field.IsPublic || field.GetCustomAttribute<SerializeField>() != null) &&
                field.GetCustomAttribute<NonSerializedAttribute>() == null &&
                field.GetCustomAttribute<JsonIgnoreAttribute>() == null);
        }
        
        private static bool IsEnumerableOfScriptableObject(Type type)
        {
            // Check if the type implements IEnumerable
            if (!typeof(IEnumerable).IsAssignableFrom(type))
            {
                return false;
            }

            // Get the type of the elements in the IEnumerable
            // For generic types (like List<T>), get the generic argument; otherwise, use the element type
            Type elementType = type.IsGenericType
                ? type.GetGenericArguments()[0]
                : type.HasElementType ? type.GetElementType() : null;

            // Check if the element type is assignable from ScriptableObject
            return elementType != null && typeof(ScriptableObject).IsAssignableFrom(elementType);
        }

        
        private static object SerializeFieldOrObject(object value)
        {
            if (value == null) return null;

            // Handle ScriptableObject instances directly
            if (value is ScriptableObject so)
            {
                return so.SerializeToJson();
            }

            // Handle lists of objects, including ScriptableObjects
            if (IsEnumerableType(value.GetType()))
            {
                var list = new List<object>();
                foreach (var item in (IEnumerable)value)
                {
                    list.Add(SerializeComplexObject(item));
                }
                return JsonConvert.SerializeObject(list, Formatting.Indented);
            }

            // Serialize other complex objects
            return SerializeComplexObject(value);
        }

        private static object SerializeComplexObject(object value)
        {
            if (value.GetType().IsPrimitive || value is string)
            {
                return value; // Primitive types and strings are serialized normally
            }

            var fields = GetSerializableFields(value.GetType())
                .ToDictionary(field => field.Name, field => {
                    var fieldValue = field.GetValue(value);
                    return fieldValue is ScriptableObject fieldSO ? fieldSO.SerializeToJson() : fieldValue;
                });

            return JsonConvert.SerializeObject(fields, Formatting.Indented);
        }

        private static bool IsEnumerableType(Type type)
        {
            return typeof(IEnumerable).IsAssignableFrom(type) && type != typeof(string);
        }

        private static object DeserializeField(Type fieldType, object value)
        {
            if (typeof(ScriptableObject).IsAssignableFrom(fieldType))
            {
                return DeserializeScriptableObject(fieldType, value.ToString());
            }
            if (IsListOfScriptableObjects(fieldType))
            {
                var itemType = fieldType.GetGenericArguments()[0];
                var itemList = ((IEnumerable)value).Cast<string>().Select(itemJson => DeserializeScriptableObject(itemType, itemJson)).ToList();
                return ConvertList(itemList, fieldType);
            }
            return Convert.ChangeType(value, fieldType); // Non-ScriptableObject values are handled normally
        }

        private static bool IsListOfScriptableObjects(Type type)
        {
            return type.IsGenericType &&
                   type.GetGenericTypeDefinition() == typeof(List<>) &&
                   typeof(ScriptableObject).IsAssignableFrom(type.GetGenericArguments()[0]);
        }

        private static IList ConvertList(IList sourceList, Type listType)
        {
            var list = (IList)Activator.CreateInstance(listType);
            foreach (var item in sourceList)
            {
                list.Add(item);
            }
            return list;
        }
    }
}
