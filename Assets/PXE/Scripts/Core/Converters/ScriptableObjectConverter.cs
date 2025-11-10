using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine;

namespace PXE.Core.Converters
{
    public class ScriptableObjectConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return typeof(ScriptableObject).IsAssignableFrom(objectType);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            // Deserialize the JSON into a JObject
            JObject jObject = JObject.Load(reader);

            // Create a new instance of the target objectType
            ScriptableObject target = (ScriptableObject)ScriptableObject.CreateInstance(objectType);

            // Populate the properties of the new instance using the JSON
            serializer.Populate(jObject.CreateReader(), target);

            return target;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            // Serialize the ScriptableObject's public properties and fields into JSON
            JToken token = JToken.FromObject(value);
            token.WriteTo(writer);
        }
    }
}