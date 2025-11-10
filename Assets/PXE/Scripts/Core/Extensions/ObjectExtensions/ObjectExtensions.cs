using System;
using System.Linq.Expressions;
using System.Reflection;
using UnityEngine;

namespace PXE.Core.Extensions.ObjectExtensions
{
    public static class ObjectExtensions
    {
        public static bool TrySet<T>(this object obj, string memberName, T value)
        {
            if (obj == null) return false;
            
            var type = obj.GetType();
            var flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static;

            // Try to get the property
            var property = type.GetProperty(memberName,flags);
            if (property != null && property.PropertyType == typeof(T))
            {
                property.SetValue(obj, value);
                return true;
            }

            // Try to get the field
            var field = type.GetField(memberName, flags);
            if (field != null && field.FieldType == typeof(T))
            {
                field.SetValue(obj, value);
                return true;
            }

            return false;
        }
        
        public static bool TrySet<T, TValue>(this T obj, Expression<Func<T, TValue>> memberExpression, TValue value)
        {
            string memberName = obj.GetMemberName(memberExpression);
            return obj.TrySet(memberName, value);
        }
        
        public static T TryGet<T>(this object obj, string memberName)
        {
            if (obj == null) return default;
            
            var type = obj.GetType();
            var flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static;

            // Try to get the property
            var property = type.GetProperty(memberName,flags);
            if (property != null && property.PropertyType == typeof(T))
            {
                return (T) property.GetValue(obj);
            }

            // Try to get the field
            var field = type.GetField(memberName, flags);
            if (field != null && field.FieldType == typeof(T))
            {
                return (T) field.GetValue(obj);
            }

            return default;
        }
        
        public static TValue TryGet<T, TValue>(this T obj, Expression<Func<T, TValue>> memberExpression)
        {
            string memberName = obj.GetMemberName(memberExpression);
            return obj.TryGet<TValue>(memberName);
        }
        
        public static string GetMemberName<T, TValue>(this T obj, Expression<Func<T, TValue>> memberExpression)
        {
            if (memberExpression.Body is MemberExpression member)
            {
                return member.Member.Name;
            }

            Debug.LogError($"Expression is not a member access for {nameof(memberExpression)}");
            return string.Empty;
        }
    }
}