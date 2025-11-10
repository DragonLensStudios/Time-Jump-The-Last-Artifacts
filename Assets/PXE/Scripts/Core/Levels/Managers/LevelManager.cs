using System.Collections;
using System.Collections.Generic;
using System.Linq;
using PXE.Core.Enums;
using PXE.Core.Levels.Messaging.Messages;
using PXE.Core.Messaging;
using PXE.Core.Objects;
using UnityEngine;

namespace PXE.Core.Levels.Managers
{
    /// <summary>
    /// Represents the LevelManager.
    /// The LevelManager class provides functionality related to levelmanager management.
    /// This class contains methods and properties that assist in managing and processing levelmanager related tasks.
    /// </summary>
    public class LevelManager : ObjectController
    {
        public static LevelManager Instance { get; private set; }

        [field: Tooltip("The levels.")]
        [field: SerializeField] public List<LevelObject> Levels { get; set; }
        
        [field: Tooltip("The current level.")]
        [field: SerializeField] public LevelObject CurrentLevelObject { get; set; }
        
        public virtual bool IsCurrentLevelLoaded => CurrentLevelObject != null && CurrentLevelObject.LevelState == LevelState.Loaded;


        /// <summary>
        ///  This method registers the LevelManager for the LevelMessage and LevelResetMessage messages.
        /// </summary>
        public override void OnActive()
        {
            base.OnActive();
            MessageSystem.MessageManager.RegisterForChannel<LevelMessage>(MessageChannels.Level, ChangeLevelHandler);
            MessageSystem.MessageManager.RegisterForChannel<LevelResetMessage>(MessageChannels.Level, LeverResetMessageHandler );
        }

        /// <summary>
        ///  This method unregisters the LevelManager for the LevelMessage and LevelResetMessage messages.
        /// </summary>
        public override void OnInactive()
        {
            base.OnInactive();
            MessageSystem.MessageManager.UnregisterForChannel<LevelMessage>(MessageChannels.Level, ChangeLevelHandler);
            MessageSystem.MessageManager.UnregisterForChannel<LevelResetMessage>(MessageChannels.Level, LeverResetMessageHandler);
        }

        /// <summary>
        ///  Singleton pattern for the level manager.
        /// </summary>
        public override void Awake()
        {
            base.Awake();
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
            if (Levels.Count <= 0) 
            {
                Levels = Resources.LoadAll<LevelObject>("Levels").ToList();
            }
        }
        

        public virtual void OnApplicationQuit()
        {
            foreach (var level in Levels)
            {
                level.LevelState = LevelState.Unloaded;
            }
        }

        public virtual void ChangeLevel(LevelObject levelObject)
        {
            StartCoroutine(ChangeLevelCoroutine(levelObject));
        }

        private IEnumerator ChangeLevelCoroutine(LevelObject levelObject)
        {
            if (CurrentLevelObject != null && CurrentLevelObject.LevelState == LevelState.Loaded)
            {
                bool exitSuccess = false;
                yield return CurrentLevelObject.ExitCoroutine(success => exitSuccess = success);
                if (!exitSuccess)
                {
                    Debug.LogError("Failed to exit current level.");
                    yield break;
                }
            }

            if (CurrentLevelObject != levelObject && levelObject.LevelState == LevelState.Unloaded)
            {
                CurrentLevelObject = levelObject;
                bool enterSuccess = false;
                yield return CurrentLevelObject.EnterCoroutine(success => enterSuccess = success);
                if (!enterSuccess)
                {
                    Debug.LogError("Failed to enter new level.");
                    yield break;
                }
            }
        }

        public virtual void ResetLevel()
        {
            StartCoroutine(ResetLevelCoroutine());
        }

        private IEnumerator ResetLevelCoroutine()
        {
            if (CurrentLevelObject != null && CurrentLevelObject.LevelState == LevelState.Loaded)
            {
                bool resetSuccess = false;
                yield return CurrentLevelObject.ResetCoroutine(success => resetSuccess = success);
                if (!resetSuccess)
                {
                    Debug.LogError("Failed to reset level.");
                }
            }
        }

        public virtual void ChangeLevelHandler(MessageSystem.IMessageEnvelope message)
        {
            if (!message.Message<LevelMessage>().HasValue) return;
            var data = message.Message<LevelMessage>().GetValueOrDefault();
            var level = Levels.FirstOrDefault(x => x.ID.Equals(data.LevelID));
            if (level == null) level = Levels.FirstOrDefault(x => x.name.Equals(data.LevelName));
            if (level == null) return;

            if (data.LevelState == LevelState.Unloading)
            {
                if (level != null)
                {
                    StartCoroutine(level.ExitCoroutine(success => { }));
                }
                CurrentLevelObject = null;
                return;
            }

            if (level != null && level.LevelState == LevelState.Unloaded)
            {
                ChangeLevel(level);
            }
        }

        public virtual void LeverResetMessageHandler(MessageSystem.IMessageEnvelope message)
        {
            if (!message.Message<LevelResetMessage>().HasValue) return;
            ResetLevel();
        }
    }
}