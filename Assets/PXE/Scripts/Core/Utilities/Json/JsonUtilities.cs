#nullable enable
using System.Collections.Generic;
using JetBrains.Annotations;
using Newtonsoft.Json;
using PXE.Core.Converters;

namespace PXE.Core.Utilities.Json
{
    public static class JsonUtilities
    {
        public static JsonSerializerSettings JsonSettingsForUnityObjects()
        {
            JsonSerializerSettings settings = new JsonSerializerSettings
            {
                PreserveReferencesHandling = PreserveReferencesHandling.Objects,
                TypeNameHandling = TypeNameHandling.Auto,
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                Formatting = Formatting.Indented,
                Converters = new List<JsonConverter> { new UnityConverter(), new ScriptableObjectConverter() }
            };
            return settings;
        }
        
        public static string SerializedUnityObject([CanBeNull] object obj, Formatting formatting)
        {
            return JsonConvert.SerializeObject(obj, formatting, JsonSettingsForUnityObjects());
        }
        
        public static T? DeserializeUnityObject<T>(string json)
        {
            return JsonConvert.DeserializeObject<T>(json, JsonSettingsForUnityObjects());
        }
    }
}