using System;
using PXE.Core.Variables;
using UnityEditor;

namespace PXE.Scripts.Core.Variables.Editor
{
    public static class VariableEditorExtensions
    {
        public static IVariable<T> EditorField<T>(this IVariable<T> variable, T defaultValue) where T : IComparable<T>, IEquatable<T>
        {
            variable.Name = EditorGUILayout.TextField(variable.Name);
        
            if (typeof(T) == typeof(int))
            {
                int intValue = (int)(object)variable.Value;
                intValue = EditorGUILayout.IntField(intValue);
                variable.Value = (T)(object)intValue;
            }
            else if (typeof(T) == typeof(long))
            {
                long longValue = (long)(object)variable.Value;
                longValue = EditorGUILayout.LongField(longValue);
                variable.Value = (T)(object)longValue;
            }
            else if (typeof(T) == typeof(short))
            {
                short shortValue = (short)(object)variable.Value;
                shortValue = (short)EditorGUILayout.IntField(shortValue);
                variable.Value = (T)(object)shortValue;
            }
            else if (typeof(T) == typeof(double))
            {
                double doubleValue = (double)(object)variable.Value;
                doubleValue = EditorGUILayout.DoubleField(doubleValue);
                variable.Value = (T)(object)doubleValue;
            }
            else if (typeof(T) == typeof(decimal))
            {
                decimal decimalValue = (decimal)(object)variable.Value;
                float floatValue = (float)(double)decimalValue;
                floatValue = EditorGUILayout.FloatField(floatValue);
                decimalValue = (decimal)floatValue;
                variable.Value = (T)(object)decimalValue;
            }
            else if (typeof(T) == typeof(float))
            {
                float floatValue = (float)(object)variable.Value;
                floatValue = EditorGUILayout.FloatField(floatValue);
                variable.Value = (T)(object)floatValue;
            }
            else if (typeof(T) == typeof(bool))
            {
                bool boolValue = (bool)(object)variable.Value;
                boolValue = EditorGUILayout.Toggle(boolValue);
                variable.Value = (T)(object)boolValue;
            }
            else if (typeof(T) == typeof(string))
            {
                string stringValue = (string)(object)variable.Value;
                stringValue = EditorGUILayout.TextField(stringValue);
                variable.Value = (T)(object)stringValue;
            }
            else if (typeof(T) == typeof(ComparableVector2))
            {
                ComparableVector2 vector2Value = (ComparableVector2)(object)variable.Value;
                vector2Value.Value = EditorGUILayout.Vector2Field("",vector2Value.Value);
                variable.Value = (T)(object)vector2Value;
            }
            else if (typeof(T) == typeof(ComparableVector3))
            {
                ComparableVector3 vector3Value = (ComparableVector3)(object)variable.Value;
                vector3Value.Value = EditorGUILayout.Vector3Field("",vector3Value.Value);
                variable.Value = (T)(object)vector3Value;
            }
            else if (typeof(T) == typeof(DateTime))
            {
                DateTime dateTimeValue = (DateTime)(object)variable.Value;
                dateTimeValue = DelayedDateTimeField(dateTimeValue);
                variable.Value = (T)(object)dateTimeValue;
            }
            else
            {
                EditorGUILayout.LabelField("Type not supported");
            }
        
            return variable;
        }


        private static DateTime DelayedDateTimeField(DateTime dateTimeValue)
        {
            EditorGUILayout.BeginHorizontal();
            DateTime result = dateTimeValue;
            string dateString = result.ToString("yyyy-MM-dd");
            string timeString = result.ToString("HH:mm:ss");
            string newDateString = EditorGUILayout.DelayedTextField(dateString);
            string newTimeString = EditorGUILayout.DelayedTextField(timeString);
            DateTime newDateTimeValue;
            if (DateTime.TryParseExact(newDateString, "yyyy-MM-dd", null, System.Globalization.DateTimeStyles.None, out var newDate) &&
                DateTime.TryParseExact(newTimeString, "HH:mm:ss", null, System.Globalization.DateTimeStyles.None, out DateTime newTime))
            {
                newDateTimeValue = new DateTime(newDate.Year, newDate.Month, newDate.Day, newTime.Hour, newTime.Minute, newTime.Second);
            }
            else
            {
                newDateTimeValue = result;
            }
            EditorGUILayout.EndHorizontal();
            return newDateTimeValue;
        }
    }
}