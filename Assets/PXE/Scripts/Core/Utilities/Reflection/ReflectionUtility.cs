using System;
using System.Collections.Generic;
using System.Linq;

namespace PXE.Core.Utilities.Reflection
{
    /// <summary>
    ///  Represents the ReflectionUtility.
    /// </summary>
    public static class ReflectionUtility
    {
        
        /// <summary>
        ///  Returns the derived types of the specified base type.
        /// </summary>
        /// <param name="baseType"></param>
        /// <returns></returns>
        public static Type[] GetDerivedTypes(Type baseType)
        {
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();
            var derivedTypes = new List<Type>();

            foreach (var assembly in assemblies)
            {
                var types = assembly.GetTypes().Where(t => t.IsSubclassOf(baseType));
                derivedTypes.AddRange(types);
            }

            return derivedTypes.ToArray();
        }
    }
}