using System;
using System.Linq;
using System.Threading.Tasks;
using PXE.Core.Data_Persistence.Asset_Management;
using PXE.Core.Data_Persistence.Asset_Management.Asset_References;
using PXE.Core.Enums;
using PXE.Core.Interfaces;
using PXE.Core.Messaging;
using PXE.Core.Messaging.Messages;
using PXE.Core.Objects;
using PXE.Core.Spawning.Messaging.Messages;
using UnityEngine;

namespace PXE.Core.Data_Persistence.Managers
{
/// <summary>
/// Represents the PrefabAssetLoader.
/// The PrefabAssetLoader class provides functionality related to prefabassetloader management.
/// This class contains methods and properties that assist in managing and processing prefabassetloader related tasks.
/// </summary>
    public class PrefabAssetLoader : ObjectController
    {
        // Singleton instance
        public static PrefabAssetLoader Instance { get; private set; }
        
        [field: Tooltip("The amount of times to retry spawning a prefab.")]
        [field: SerializeField] public virtual int RetryAmount { get; set; } = 5;
        
        [field: Tooltip("The prefab references.")]
        [field: SerializeField] public virtual PrefabReferences PrefabReferences { get; set; }

        
        protected int retryCount = 0;


        /// <summary>
        ///  Singleton pattern for the prefabassetloader.
        /// </summary>
        public override void Awake()
        {
            // Singleton pattern
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
            base.Awake();
        }
        
        /// <summary>
        ///  This method registers the PrefabAssetLoader for the SpawnPrefabMessage message.
        /// </summary>
        public override void OnActive()
        {
            base.OnActive();
            MessageSystem.MessageManager.RegisterForChannel<SpawnPrefabMessage>(MessageChannels.Spawning, SpawnPrefabMessageHandler);

        }

        /// <summary>
        ///  This method unregisters the PrefabAssetLoader for the SpawnPrefabMessage message.
        /// </summary>
        public override void OnInactive()
        {
            base.OnInactive();
            MessageSystem.MessageManager.UnregisterForChannel<SpawnPrefabMessage>(MessageChannels.Spawning, SpawnPrefabMessageHandler);

        }
        
        
        /// <summary>
        ///  Spawns a prefab with the specified key, name, position, rotation, scale, and parent.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="name"></param>
        /// <param name="position"></param>
        /// <param name="rotation"></param>
        /// <param name="scale"></param>
        /// <param name="parent"></param>
        /// <returns></returns>
        public virtual async Task<GameObject> SpawnPrefab(string key, string name, Vector3 position, Quaternion rotation, Vector3 scale, Transform parent = null)
        {
            if (!PrefabReferences.PrefabAssetReferences.ContainsKey(key)) return null;
            
            AssetReferenceGameObject assetReference = PrefabReferences.PrefabAssetReferences[key];
            GameObject go = await assetReference.InstantiateAsync(position, rotation, parent).Task;
        
            if (go == null)
            {
                if (retryCount > RetryAmount) return go;
                Debug.LogWarning($"Something went wrong trying to spawn prefab trying again. Try: {retryCount + 1} / {RetryAmount + 1}");
                go = await SpawnPrefab(key, name, position, rotation, scale, parent);
                retryCount++;
            }
            else
            {
                go.name = name;
                go.transform.localScale = scale;
                await MessageSystem.MessageManager.SendImmediateAsync(MessageChannels.Spawning, new GameObjectMessage(go));

                retryCount = 0;
            }

            return go;

        }
        
        /// <summary>
        ///  Handles the spawn prefab message and spawns the prefab.
        /// </summary>
        /// <param name="message"></param>
        public virtual async void SpawnPrefabMessageHandler(MessageSystem.IMessageEnvelope message)
        {
            if (!message.Message<SpawnPrefabMessage>().HasValue) return;
            var data = message.Message<SpawnPrefabMessage>().GetValueOrDefault();
            var objects = FindObjectsByType<GameObject>(FindObjectsInactive.Include, FindObjectsSortMode.None).Select(x=> x.GetComponent<IGameObject>());
            Transform parentTransform = null;
            foreach (var obj in objects)
            {
                var id = obj.ID;
                if (id.Guid == Guid.Empty ) continue;
                if (id.Equals(data.Transform.ParentID))
                {
                    parentTransform = obj.gameObject.transform;
                }
            }
            var spawnedObj = await SpawnPrefab(data.PrefabKey, data.Name, data.Transform.LocalPosition, data.Transform.LocalRotation, data.Transform.LocalScale, parentTransform);
        }
    }
}