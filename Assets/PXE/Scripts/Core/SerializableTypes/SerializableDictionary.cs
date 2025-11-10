using System.Collections.Generic;
using UnityEngine;

namespace PXE.Core.SerializableTypes
{
    //TODO: Change the fields to be properties with backing fields.
    [System.Serializable]
    public class SerializableDictionary<TKey, TValue> : Dictionary<TKey, TValue>, ISerializationCallbackReceiver
    {

        [SerializeField] private List<TKey> keys = new List<TKey>();
        [SerializeField] private List<TValue> values = new List<TValue>();

        // save the dictionary to lists
/// <summary>
/// Executes the OnBeforeSerialize method.
/// Handles the OnBeforeSerialize functionality.
/// </summary>
        public void OnBeforeSerialize()
        {
            keys.Clear();
            values.Clear();
            foreach (KeyValuePair<TKey, TValue> pair in this) 
            {
                keys.Add(pair.Key);
                values.Add(pair.Value);
            }
        }

        // load the dictionary from lists
/// <summary>
/// Executes the OnAfterDeserialize method.
/// Handles the OnAfterDeserialize functionality.
/// </summary>
        public void OnAfterDeserialize()
        {
            SyncDictionaryFromLists();
        }

/// <summary>
/// Executes the SyncDictionaryFromLists method.
/// Handles the SyncDictionaryFromLists functionality.
/// </summary>
        public void SyncDictionaryFromLists()
        {
            Clear();

            if (keys.Count != values.Count) 
            {
                Debug.LogError("Tried to deserialize a SerializableDictionary, but the amount of keys ("
                               + keys.Count + ") does not match the number of values (" + values.Count 
                               + ") which indicates that something went wrong");
                return;
            }

            for (int i = 0; i < keys.Count; i++)
            {
                TryAdd(keys[i], values[i]);
            }
        }
    }
}