using System.Collections;
using System.Linq;
using PXE.Core.Audio;
using PXE.Core.Audio.Messaging.Messages;
using PXE.Core.Enums;
using PXE.Core.Interfaces;
using PXE.Core.Levels.Messaging.Messages;
using PXE.Core.Messaging;
using PXE.Core.ScriptableObjects;
using UnityEngine;

namespace PXE.Core.Levels
{
    /// <summary>
    ///  This class represents the level.
    /// </summary>
    [System.Serializable]
    [CreateAssetMenu(fileName = "New Level", menuName = "PXE/Game/Levels")]
    public class LevelObject : ScriptableObjectController
    {
        [field: Tooltip("The prefab of the level.")]
        [field: SerializeField] public virtual GameObject LevelPrefab { get; set; }
        
        [field: Tooltip("The position of the player spawn.")]
        [field: SerializeField] public virtual Vector2 PlayerSpawnPosition { get; set; }
        
        [field: Tooltip("The state of the level.")]
        [field: SerializeField] public virtual LevelState LevelState { get; set; }
        
        [field: Tooltip("The sfx that plays when the level is entered.")]
        [field: SerializeField] public virtual AudioObject EnterSfx { get; set; }
        
        [field: Tooltip("The sfx that plays when the level is exited.")]
        [field: SerializeField] public virtual AudioObject ExitSfx { get; set; }
        
        [field: Tooltip("The bgm that plays when the level is entered.")]
        [field: SerializeField] public virtual AudioObject EnterBgm { get; set; }
        
        [field: Tooltip("The bgm that plays when the level is exited.")]
        [field: SerializeField] public virtual AudioObject ExitBgm { get; set; }
        
        [field: Tooltip("The sfx that plays when the level is reset.")]
        [field: SerializeField] public virtual AudioObject ResetSfx { get; set; }
        
        [field: Tooltip("The bgm that plays when the level is reset.")]
        [field: SerializeField] public virtual AudioObject ResetBgm { get; set; }
        
        [field: Tooltip("The spawned level.")]
        public GameObject spawnedLevel;

        public IEnumerator EnterCoroutine(System.Action<bool> callback)
        {
            if (EnterSfx != null)
            {
                MessageSystem.MessageManager.SendImmediate(MessageChannels.Audio, new AudioMessage(EnterSfx, AudioOperation.Play, AudioChannel.SoundEffects));
            }
            if (EnterBgm != null)
            {
                MessageSystem.MessageManager.SendImmediate(MessageChannels.Audio, new AudioMessage(EnterBgm, AudioOperation.Play, AudioChannel.Music));
            }
            
            LevelState = LevelState.Loading;
            spawnedLevel = Instantiate(LevelPrefab);
            yield return new WaitForEndOfFrame();
            if (spawnedLevel == null)
            {
                callback(false);
                yield break;
            }

            spawnedLevel.name = Name;

            // Wait until all IInitializable components are initialized
            IInitializable[] initializables = spawnedLevel.GetComponentsInChildren<IInitializable>();
            foreach (var initializable in initializables)
            {
                initializable.Initialize();
            }

            // Wait for all components to be initialized
            while (!initializables.All(i => i.IsInitialized))
            {
                yield return null;
            }

            LevelState = LevelState.Loaded;
            MessageSystem.MessageManager.SendImmediate(MessageChannels.Level, new LevelMessage(ID, Name, LevelState, PlayerSpawnPosition));

            callback(true);
        }

        public IEnumerator ExitCoroutine(System.Action<bool> callback)
        {
            if (spawnedLevel == null)
            {
                callback(true);
                yield break;
            }

            if (ExitSfx != null)
            {
                MessageSystem.MessageManager.SendImmediate(MessageChannels.Audio, new AudioMessage(ExitSfx, AudioOperation.Play, AudioChannel.SoundEffects));
            }

            if (ExitBgm != null)
            {
                MessageSystem.MessageManager.SendImmediate(MessageChannels.Audio, new AudioMessage(ExitBgm, AudioOperation.Play, AudioChannel.Music));
            }

            Destroy(spawnedLevel);
            LevelState = LevelState.Unloaded;
            MessageSystem.MessageManager.SendImmediate(MessageChannels.Level, new LevelMessage(ID, Name, LevelState, PlayerSpawnPosition));
            callback(true);
        }

        public IEnumerator ResetCoroutine(System.Action<bool> callback)
        {
            if (spawnedLevel == null)
            {
                callback(false);
                yield break;
            }

            if (ResetSfx != null)
            {
                MessageSystem.MessageManager.SendImmediate(MessageChannels.Audio, new AudioMessage(ResetSfx, AudioOperation.Play, AudioChannel.SoundEffects));
            }

            if (ResetBgm != null)
            {
                MessageSystem.MessageManager.SendImmediate(MessageChannels.Audio, new AudioMessage(ResetBgm, AudioOperation.Play, AudioChannel.Music));
            }

            Destroy(spawnedLevel);
            LevelState = LevelState.Unloaded;
            MessageSystem.MessageManager.SendImmediate(MessageChannels.Level, new LevelMessage(ID, Name, LevelState, PlayerSpawnPosition));

            LevelState = LevelState.Loading;
            spawnedLevel = Instantiate(LevelPrefab);
            if (spawnedLevel == null)
            {
                callback(false);
                yield break;
            }

            spawnedLevel.name = Name;

            // Wait until all IInitializable components are initialized
            IInitializable[] initializables = spawnedLevel.GetComponentsInChildren<IInitializable>();
            foreach (var initializable in initializables)
            {
                initializable.Initialize();
            }

            // Wait for all components to be initialized
            while (!initializables.All(i => i.IsInitialized))
            {
                yield return null;
            }

            LevelState = LevelState.Loaded;
            MessageSystem.MessageManager.SendImmediate(MessageChannels.Level, new LevelMessage(ID, Name, LevelState, PlayerSpawnPosition));
            callback(true);
        }
    }
}