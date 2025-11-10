using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using PXE.Core.Utilities.Containers;

namespace PXE.Core.Extensions.StringExtensions
{
    public static class StringParsingExtensions
    {
        private static readonly Regex methodRegex = new Regex(@"\((.*?)\)", RegexOptions.Compiled);
        private static readonly Regex indexerRegex = new Regex(@"\[(.*?)\]", RegexOptions.Compiled);
        private static readonly Dictionary<object, IDictionary<string, object>> cache = new Dictionary<object, IDictionary<string, object>>();

        public static string ParseObject(this string text, object target)
        {
            var variableStore = ExtractVariables(target, text);
            return ParseVariableInternal(text, variableStore);
        }
        private static IDictionary<string, object> ExtractVariables(object target, string input)
        {
            if (target == null) return new Dictionary<string, object>();

            // Check if the object has already been parsed
            if (cache.TryGetValue(target, out var variableStore))
            {
                return variableStore;
            }

            // If not, parse the object and store the result in the cache
            variableStore = new Dictionary<string, object>();
            ExtractIndexer(target, "", input, variableStore);
            ExtractFields(target, "", variableStore, new HashSet<object>());
            ExtractProperties(target, "", variableStore, new HashSet<object>());
            ExtractMethods(target, variableStore);

            cache[target] = variableStore;

            return variableStore;
        }


        private static void ExtractFields(object target, string prefix, IDictionary<string, object> variableStore, HashSet<object> visitedObjects)
        {
            if (visitedObjects.Contains(target))
                return;

            visitedObjects.Add(target);

            var fields = target.GetType().GetFields();
            foreach (var field in fields)
            {
                object value = field.GetValue(target);
                string fieldName = $"{prefix}{field.Name}";
                if (!variableStore.ContainsKey(fieldName))
                {
                    variableStore[fieldName] = value;
                }

                if (IsNestedType(field.FieldType))
                {
                    if (value != null)
                    {
                        ExtractFields(value, $"{fieldName}.", variableStore, visitedObjects);
                        ExtractProperties(value, $"{fieldName}.", variableStore, visitedObjects);
                        ExtractMethods(value, variableStore);
                    }
                }
            }
        }

        private static void ExtractProperties(object target, string prefix, IDictionary<string, object> variableStore, HashSet<object> visitedObjects)
        {
            if (visitedObjects.Contains(target))
                return;

            visitedObjects.Add(target);

            var properties = target.GetType().GetProperties();
            foreach (var property in properties)
            {
                if (property.CanRead)
                {
                    object value = null;
                    string propertyName = string.Empty;

                    if (property.GetIndexParameters().Length > 0)
                    {
                        // Handle indexers by skipping them
                        continue;
                    }
                    else
                    {
                        value = property.GetValue(target);
                        propertyName = $"{prefix}{property.Name}";
                    }

                    if (value == null && string.IsNullOrEmpty(propertyName))
                    {
                        continue;
                    }

                    if (!variableStore.ContainsKey(propertyName))
                    {
                        variableStore[propertyName] = value;
                    }

                    if (IsNestedType(property.PropertyType))
                    {
                        if (value != null)
                        {
                            ExtractFields(value, $"{propertyName}.", variableStore, visitedObjects);
                            ExtractProperties(value, $"{propertyName}.", variableStore, visitedObjects);
                            ExtractMethods(value, variableStore);
                        }
                    }
                }
            }
        }

        private static void ExtractMethods(object target, IDictionary<string, object> variableStore)
        {
            var methods = target.GetType().GetMethods();
            foreach (var method in methods)
            {
                // We will store the MethodContainer itself as the value in the dictionary
                // So we can invoke it later in the ResolveNestedPropertyOrMethod function
                // If a method with the same name already exists in the store, append an index to it
                string methodName = method.Name;
                int index = 1;
                while (variableStore.ContainsKey(methodName))
                {
                    methodName = $"{method.Name}{index++}";
                }

                var parameters = method.GetParameters();
                if (parameters.Length > 0)
                {
                    // If the method has parameters, store the MethodContainer with the parameters and their types in the variable store
                    var methodContainer = new MethodContainer { Instance = target, Method = method };
                    methodContainer.Parameters = parameters.Select(p => new { Name = p.Name, Type = p.ParameterType }).ToArray();
                    variableStore[methodName] = methodContainer;
                }
                else
                {
                    if (!variableStore.ContainsKey(methodName))
                    {
                        // If the method has no parameters, store the MethodContainer without parameters in the variable store
                        variableStore[methodName] = new MethodContainer { Instance = target, Method = method };
                    }
                }
            }
        }


        private static void ExtractIndexer(object target, string prefix, string input, IDictionary<string, object> variableStore)
        {
            string pattern = @"\{([^{\[]+)";
        
            var properties = target.GetType().GetProperties();
            foreach (var property in properties)
            {
                Match variableNameMatch = Regex.Match(property.Name, pattern);
        
                if (variableNameMatch.Success)
                {
                    prefix = variableNameMatch.Groups[1].Value.Trim();
                }
                
                if (property.CanRead && property.GetIndexParameters().Length > 0)
                {
                    if (property.Name.Equals("Item") && string.IsNullOrEmpty(prefix))
                    {
                        Match match = indexerRegex.Match(input);
                        if (match.Success)
                        {
                            string indexValue = match.Groups[1].Value.RemoveQuotations();
                            string propertyName = $"{property.Name}[{indexValue}]";
                            if (variableStore.ContainsKey(propertyName))
                            {
                                continue;;
                            }
                            
                            // Only attempt to get the indexer value if a valid index was found in the input string
                            if (!string.IsNullOrEmpty(indexValue))
                            {
                                object value = GetIndexerValue(property, target, indexValue);
                                if(value == null){ continue; }
                                if (!variableStore.ContainsKey(propertyName))
                                {
                                    variableStore[propertyName] = value;
                                }
                            }
                        }
                    }
                }
            }
        }


        private static object GetIndexerValue(PropertyInfo property, object target, string indexValue)
        {
            var indexerParameters = property.GetIndexParameters();
            var indexType = indexerParameters[0].ParameterType;

            var convertedIndex = ConvertToIndexType(indexValue, indexType);
            if(convertedIndex == null){ return null; }

            if (convertedIndex.GetType() != indexType) 
            {
                return null;
            }
    
            try
            {
                var val = property.GetValue(target, new[] { convertedIndex });
                return val;
            }
            catch (TargetInvocationException ex)
            {
                // Check if the inner exception is a KeyNotFoundException.
                if (ex.InnerException is KeyNotFoundException)
                {
                    // Handle KeyNotFoundException
                    return null;
                }
                else
                {
                    // If not, rethrow the original exception
                    throw;
                }
            }
        }



        private static bool IsNestedType(Type type)
        {
            if (!type.IsPrimitive && type != typeof(string) && !type.IsValueType)
                return true;

            if (type.IsArray)
                return IsNestedType(type.GetElementType());

            if (type.IsGenericType)
            {
                var genericArguments = type.GetGenericArguments();
                foreach (var argument in genericArguments)
                {
                    if (IsNestedType(argument))
                        return true;
                }
            }

            return false;
        }

        private static string ParseVariableInternal(string text, IDictionary<string, object> variableStore)
        {
            try
            {
                var pattern = @"\{([^}]+)\}";
                var matches = Regex.Matches(text, pattern).Cast<Match>().ToList();
                matches.Reverse();

                foreach (Match match in matches)
                {
                    string variableName = match.Value.Substring(1, match.Value.Length - 2);
                    object resolvedValue = ResolveNestedPropertyOrMethod(variableName, variableStore);
                    string replacement = resolvedValue?.ToString() ?? string.Empty;
                    replacement = Regex.Replace(replacement, @"^""(.*)""$", "$1");
                    replacement = replacement.Replace("\"", string.Empty);
                    text = text.Remove(match.Index, match.Length).Insert(match.Index, replacement);
                }
            }
            catch (Exception e)
            {
                throw new Exception($"Failed to parse variables in text: {e.Message}", e);
            }

            return text;
        }

       private static object ResolveNestedPropertyOrMethod(string variableName, IDictionary<string, object> variableStore)
{
    string[] propertyChain = variableName.Split('.');
    string baseVariable = propertyChain[0].Split('[')[0].Split('(')[0];
    string nestedValue = propertyChain.Length > 1 ? propertyChain[1].Split('(')[0] : string.Empty;
    object currentObject = null;

    // Try to get the object for variableName from variableStore
    if (variableStore.TryGetValue(baseVariable, out currentObject))
    {
        if (currentObject is MethodContainer methodContainer)
        {
            // If it's a MethodContainer instance, update the currentObject and baseVariable
            currentObject = methodContainer;
            baseVariable = propertyChain.Length > 1 ? propertyChain[1] : null;
        }
    }
    else if (variableStore.TryGetValue(nestedValue, out currentObject))
    {
        if (currentObject is MethodContainer methodContainer)
        {
            currentObject = methodContainer;
            baseVariable = nestedValue;
        }
    }
    else if (variableStore.TryGetValue($"Item{variableName.RemoveQuotations()}", out currentObject))
    {
        return currentObject;
    }
    // If not a method with no parameters
    if (!string.IsNullOrEmpty(baseVariable) && !variableStore.TryGetValue(baseVariable, out currentObject))
    {
        throw new ArgumentException($"Failed to find variable '{baseVariable}' in the store");
    }

    // Handle initial indexer
    if (propertyChain[0].Contains("["))
    {
        var indexSplit = propertyChain[0].Split('[');
        var propertyName = indexSplit[0];
        var indexValue = string.Join("[", indexSplit.Skip(1)).TrimEnd(']').RemoveQuotations();
        currentObject = GetIndexerValue(currentObject, propertyName, indexValue);
    }

    // Handle initial method with parameters
    if (propertyChain[0].Contains("("))
    {
        var methodSplit = propertyChain[0].Split('(');
        var methodName = methodSplit[0];
        var parameters = methodSplit[1]?.TrimEnd(')')?.Split(',')
            .Select(p => p.Trim())
            .Where(p => !string.IsNullOrEmpty(p))
            .Select(p => ResolveParameterValue(p, variableStore))
            .ToArray();

        if (parameters.Length > 0 && variableStore.TryGetValue(methodName + parameters.Length, out currentObject))
        {
            if (currentObject is MethodContainer methodContainer)
            {
                methodContainer.Parameters =
                    parameters.Select(p => (object)p).ToArray(); // Convert parameters to object[]
                // If it's a MethodContainer instance, update the currentObject and baseVariable
                currentObject = methodContainer;
                currentObject = InvokeMethod(currentObject, methodName, methodContainer.Parameters);
            }
        }
        else if (parameters.Length > 0 && currentObject == null && variableStore.TryGetValue(methodName, out currentObject))
        {
            if (currentObject is MethodContainer methodContainer)
            {
                methodContainer.Parameters =
                    parameters.Select(p => (object)p).ToArray(); // Convert parameters to object[]
                // If it's a MethodContainer instance, update the currentObject and baseVariable
                currentObject = methodContainer;
                currentObject = InvokeMethod(currentObject, methodName, methodContainer.Parameters);
            }
        }
    }

    for (int i = 1; i < propertyChain.Length; i++)
    {
        string property = propertyChain[i];

        if (property.Contains("("))
        {
            var methodSplit = property.Split('(');
            var methodName = methodSplit[0];
            var parameters = methodSplit[1]?.TrimEnd(')')?.Split(',')
                .Select(p => p.Trim())
                .Where(p => !string.IsNullOrEmpty(p))
                .Select(p => ResolveParameterValue(p, variableStore))
                .ToArray();

            if (parameters.Length > 0 && variableStore.TryGetValue(methodName + parameters.Length, out currentObject))
            {
                if (currentObject is MethodContainer methodContainer)
                {
                    methodContainer.Parameters =
                        parameters.Select(p => (object)p).ToArray(); // Convert parameters to object[]
                    // If it's a MethodContainer instance, update the currentObject and baseVariable
                    currentObject = methodContainer;
                    currentObject = InvokeMethod(currentObject, methodName, methodContainer.Parameters);
                }
            }
            else if (parameters.Length > 0 && variableStore.TryGetValue(methodName, out currentObject))
            {
                if (currentObject is MethodContainer methodContainer)
                {
                    methodContainer.Parameters =
                        parameters.Select(p => (object)p).ToArray(); // Convert parameters to object[]
                    // If it's a MethodContainer instance, update the currentObject and baseVariable
                    currentObject = methodContainer;
                    currentObject = InvokeMethod(currentObject, methodName, methodContainer.Parameters);
                }
            }
            else if (parameters.Length <= 0 && variableStore.TryGetValue(methodName, out currentObject))
            {
                if (currentObject is MethodContainer methodContainer)
                {
                    methodContainer.Parameters = Array.Empty<object>();
                    // If it's a MethodContainer instance, update the currentObject and baseVariable
                    currentObject = methodContainer;
                    currentObject = InvokeMethod(currentObject, methodName, methodContainer.Parameters);
                }
            }
        }
        else if (property.Contains("["))
        {
            var indexSplit = property.Split('[');
            var propertyName = indexSplit[0];
            var indexValue = string.Join("[", indexSplit.Skip(1)).TrimEnd(']').RemoveQuotations();
            currentObject = GetIndexerValue(currentObject, propertyName, indexValue);
        }
        else
        {
            currentObject = GetIndexerOrMemberValue(currentObject, property, null);
        }
    }

    return currentObject;
}

       private static object ResolveParameterValue(string parameterValue, IDictionary<string, object> variableStore)
       {
           if (parameterValue.StartsWith("\"") && parameterValue.EndsWith("\"") && parameterValue.Length > 1)
           {
               // Remove quotes from string literals
               return parameterValue.Trim('"');
           }
           else if (int.TryParse(parameterValue, out int intValue))
           {
               return intValue;
           }
           else if (long.TryParse(parameterValue, out long longValue))
           {
               return longValue;
           }
           else if (short.TryParse(parameterValue, out short shortValue))
           {
               return shortValue;
           }
           else if (double.TryParse(parameterValue, out double doubleValue))
           {
               return doubleValue;
           }
           else if (decimal.TryParse(parameterValue, out decimal decimalValue))
           {
               return decimalValue;
           }
           else if (float.TryParse(parameterValue, out float floatValue))
           {
               return floatValue;
           }
           else if (DateTime.TryParse(parameterValue, out DateTime dateTimeValue))
           {
               return dateTimeValue;
           }
           else if (bool.TryParse(parameterValue, out bool boolValue))
           {
               return boolValue;
           }
           else
           {
               // Resolve nested properties or methods
               return ResolveNestedPropertyOrMethod(parameterValue, variableStore);
           }
       }


       private static object GetIndexerOrMemberValue(object obj, string propertyName, string indexValue)
       {
           Type objectType = obj.GetType();

           // First, try to get a property with the given name
           PropertyInfo propertyInfo = objectType.GetProperty(propertyName);
           if (propertyInfo != null)
           {
               return propertyInfo.GetValue(obj);
           }

           // If property not found, try to get a field with the given name
           FieldInfo fieldInfo = objectType.GetField(propertyName);
           if (fieldInfo != null)
           {
               return fieldInfo.GetValue(obj);
           }

           // If property or field not found, check if it is an indexer
           PropertyInfo indexerPropertyInfo = objectType.GetProperty("Item");
           if (indexerPropertyInfo != null)
           {
               if (indexValue != null)
               {
                   return GetIndexerValue(obj, indexerPropertyInfo.Name, indexValue);
               }
               else
               {
                   throw new ArgumentException($"Missing index value for indexer property '{propertyName}'");
               }
           }

           throw new ArgumentException($"Failed to find property, field, or indexer '{propertyName}' in type '{objectType.Name}'");
       }

       private static object InvokeMethod(object obj, string methodName, params object[] parameters)
       {
           if (obj is MethodContainer methodContainer)
           {
               var methodInfo = methodContainer.Method;
               var objInstance = methodContainer.Instance;

               try
               {
                   if (parameters.Length > 0)
                   {
                       var result = methodInfo.Invoke(objInstance, parameters);
                       return ConvertReturnValue(result, methodInfo.ReturnType);
                   }
                   else if (methodInfo.GetParameters().Length == 0)
                   {
                       var result = methodInfo.Invoke(objInstance, null);
                       return ConvertReturnValue(result, methodInfo.ReturnType);
                   }
                   else
                   {
                       throw new ArgumentException("Parameter count mismatch");
                   }
               }
               catch (Exception ex)
               {
                   throw new Exception($"Method invocation failed: {ex.Message}", ex);
               }
           }
           else
           {
               throw new ArgumentException($"Object '{obj}' is not a MethodContainer instance");
           }
       }

       private static object ConvertReturnValue(object value, Type targetType)
       {
           if (value == null || targetType == typeof(void))
           {
               return null;
           }
           else if (targetType.IsInstanceOfType(value))
           {
               return value;
           }
           else
           {
               try
               {
                   return Convert.ChangeType(value, targetType);
               }
               catch (Exception ex)
               {
                   throw new Exception($"Failed to convert return value to type '{targetType.FullName}': {ex.Message}", ex);
               }
           }
       }



private static object GetIndexerValue(object obj, string propertyName, string indexValue)
{
    if (obj is Array jaggedArray && jaggedArray.GetType().GetElementType().IsArray)
    {
        var indices = ParseArrayIndices(indexValue);
        return GetJaggedArrayValue(jaggedArray, indices);
    }
    else if (obj is Array array)
    {
        var indices = ParseArrayIndices(indexValue);
        return GetArrayValue(array, indices);
    }
    else if (obj is IList objList)
    {
        int index = int.Parse(indexValue);
        return objList[index];
    }
    else if (obj is IDictionary objDictionary)
    {
        var indexerPropertyInfo = objDictionary.GetType().GetProperty("Item");
        if (indexerPropertyInfo != null)
        {
            var indexerParameters = indexerPropertyInfo.GetIndexParameters();
            if (indexerParameters.Length == 1)
            {
                var keyType = indexerParameters[0].ParameterType;
                object typedIndexValue = ConvertToIndexType(indexValue, keyType);
                if (typedIndexValue is string stringValue)
                {
                    stringValue = stringValue.RemoveQuotations();
                    if (!objDictionary.Contains(stringValue))
                    {
                        throw new KeyNotFoundException($"Key '{typedIndexValue}' not found in dictionary property '{propertyName}'");
                    }
                    return indexerPropertyInfo.GetValue(objDictionary, new[] { stringValue });
                }
                else
                {
                    if (!objDictionary.Contains(typedIndexValue))
                    {
                        throw new KeyNotFoundException($"Key '{typedIndexValue}' not found in dictionary property '{propertyName}'");
                    }
                    return indexerPropertyInfo.GetValue(objDictionary, new[] { typedIndexValue });
                }
            }
        }
    }
    else if (obj is IList<List<object>> jaggedList)
    {
        var indices = ParseArrayIndices(indexValue);
        return GetJaggedArrayValue(jaggedList.ToArray(), indices);
    }
    else if (obj != null)
    {
        // Check if the property or field with the given name exists
        PropertyInfo propertyInfo = obj.GetType().GetProperty(propertyName);
        FieldInfo fieldInfo = obj.GetType().GetField(propertyName);

        if (propertyInfo != null)
        {
            var nestedObject = propertyInfo.GetValue(obj);
            return GetIndexerValue(nestedObject, propertyName, indexValue);
        }
        else if (fieldInfo != null)
        {
            var nestedObject = fieldInfo.GetValue(obj);
            return GetIndexerValue(nestedObject, propertyName, indexValue);
        }
        else
        {
            // If the property or field does not exist, attempt to get an indexer
            var indexerPropertyInfo = obj.GetType().GetProperty("Item");
            if (indexerPropertyInfo != null)
            {
                var indexerParameters = indexerPropertyInfo.GetIndexParameters();
                if (indexerParameters.Length == 1)
                {
                    var keyType = indexerParameters[0].ParameterType;
                    object typedIndexValue = ConvertToIndexType(indexValue, keyType);
                    if (typedIndexValue == null)
                    {
                        throw new ArgumentException($"Index value '{indexValue}' cannot be converted to type '{keyType.Name}'");
                    }

                    try
                    {
                        return indexerPropertyInfo.GetValue(obj, new[] { typedIndexValue });
                    }
                    catch (TargetInvocationException ex)
                    {
                        if (ex.InnerException is KeyNotFoundException)
                        {
                            throw new KeyNotFoundException($"Key '{typedIndexValue}' not found in indexer property '{propertyName}'", ex);
                        }
                        else
                        {
                            throw;
                        }
                    }
                }
            }
            else if (obj.GetType().IsValueType)
            {
                throw new ArgumentException($"Property '{propertyName}' does not support indexing");
            }
        }
    }

    throw new ArgumentException($"Property '{propertyName}' does not support indexing");
}


private static object GetJaggedArrayValue(Array array, int[] indices, int currentIndex = 0)
{
    if (currentIndex >= indices.Length)
    {
        throw new ArgumentException("Invalid jagged array structure");
    }

    int index = indices[currentIndex];
    if (array.Length <= index)
    {
        throw new IndexOutOfRangeException($"Index {index} is out of range for jagged array at level {currentIndex}");
    }

    object element = array.GetValue(index);
    if (element is Array nestedArray)
    {
        if (currentIndex == indices.Length - 1)
        {
            throw new ArgumentException($"Invalid jagged array structure at level {currentIndex + 1}");
        }

        int[] subIndices = new int[indices.Length - currentIndex - 1];
        Array.Copy(indices, currentIndex + 1, subIndices, 0, subIndices.Length);

        return GetJaggedArrayValue(nestedArray, subIndices, 0);
    }
    else if (currentIndex == indices.Length - 1)
    {
        return element;
    }
    else
    {
        throw new ArgumentException($"Invalid jagged array structure at level {currentIndex + 1}");
    }
}


private static object GetArrayValue(Array array, int[] indices)
{
    if (array.Rank != indices.Length)
        throw new ArgumentException("Array rank does not match provided indices");

    int[] indicesToUse = new int[indices.Length];
    for (int i = 0; i < indices.Length; i++)
    {
        indicesToUse[i] = indices[i];
    }

    object result = array.GetValue(indicesToUse);
    return result;
}

       
       private static object ConvertToIndexType(string indexValue, Type targetType)
       {
           if (targetType == typeof(int))
           {
               if (int.TryParse(indexValue, out int parsedValue))
                   return parsedValue;
           }
           else if (targetType == typeof(long))
           {
               if (long.TryParse(indexValue, out long parsedValue))
                   return parsedValue;
           }
           else if (targetType == typeof(short))
           {
               if (short.TryParse(indexValue, out short parsedValue))
                   return parsedValue;
           }
           else if (targetType == typeof(double))
           {
               if (double.TryParse(indexValue, out double parsedValue))
                   return parsedValue;
           }
           else if (targetType == typeof(decimal))
           {
               if (decimal.TryParse(indexValue, out decimal parsedValue))
                   return parsedValue;
           }
           else if (targetType == typeof(float))
           {
               if (float.TryParse(indexValue, out float parsedValue))
                   return parsedValue;
           }
           else if (targetType == typeof(string))
           {
               return indexValue;
           }
           else if (targetType == typeof(DateTime))
           {
               if (DateTime.TryParse(indexValue, out DateTime parsedValue))
                   return parsedValue;
           }
           else
           {
               throw new NotSupportedException($"Indexer type '{targetType.Name}' is not supported.");
           }

           return null; // Return null if parsing fails
       }
       
       private static int[] ParseArrayIndices(string indexValue)
       {
           var indices = indexValue.Split(new[] { "[", "]", "," }, StringSplitOptions.RemoveEmptyEntries)
               .Select(index => int.Parse(index.Trim()))
               .ToArray();

           return indices;
       }
    }
}
