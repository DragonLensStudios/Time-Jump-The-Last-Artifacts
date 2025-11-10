using System;
using System.Collections;
using System.Collections.Generic;
using PXE.Core.Data_Persistence.Data;
using PXE.Core.Data_Persistence.Interfaces;
using PXE.Core.Data_Persistence.Messaging.Messages;
using PXE.Core.Enums;
using PXE.Core.Game.Managers;
using PXE.Core.Inventory.Data;
using PXE.Core.Messaging;
using PXE.Core.Objects;
using PXE.Core.Player;
using PXE.Core.Player.Managers;
using PXE.Core.SerializableTypes;
using PXE.Core.State_System;
using PXE.Core.UI.Messaging.Messages;
using UnityEngine;

namespace PXE.Core.Data_Persistence.Managers
{
    //TODO: Refactor this class to use the backing properties and add more handling with multiple gamedata types.
    public class DataPersistenceManager : ObjectController
    {
        public static DataPersistenceManager Instance { get; private set; }

        public BaseGameDataHandlerObject baseGameDataHandler;
        
        [field:SerializeField] [field:Header("Debugging")] public virtual bool DisableDataPersistence { get; set; } = false;
        [field:SerializeField] public virtual bool InitializeDataIfNull { get; set; } = false;
        [field:SerializeField] public virtual bool OverrideSelectedProfileId { get; set; } = false;
        [field:SerializeField] public virtual SerializableGuid TestSelectedPlayerID { get; set; } = new(Guid.Empty);
        [field:SerializeField] [field:Header("Auto Saving Configuration")] public virtual bool UseAutoSave { get; set; } = false;
        [field:SerializeField] public virtual bool DisplayAutoSaveNotification { get; set; } = false;
        [field:SerializeField] public virtual float DisplayAutoSaveNotificationTime { get; set; } = 2.5f;
        [field:SerializeField] public virtual bool UseSaveOnExit { get; set; } = false;
        [field:SerializeField] public virtual float AutoSaveTimeSeconds { get; set; } = 60f;
        [field:SerializeField] public virtual string SelectedPlayerName { get; set; } = string.Empty;
        [field:SerializeField] public virtual SerializableGuid SelectedPlayerID { get; set; } = new(Guid.Empty);
        
        [field:SerializeField] public virtual Vector3 SelectedPlayerPosition { get; set; } = Vector3.zero;

        [field:SerializeField] public virtual SerializableGuid SelectedPlayerLevelID { get; set; } = new(Guid.Empty);

        [field:SerializeField] public virtual string SelectedPlayerLevelName { get; set; } = string.Empty;

        [field:SerializeField] public virtual List<SavedGameObject> SelectedPlayerSavedGameObjects { get; set; } = new();
        
        protected Coroutine autoSaveCoroutine;

        protected PlayerController player;

        public override void Awake()
        {
            if (Instance != null) 
            {
                Debug.Log("Found more than one Data Persistence Manager in the scene. Destroying the newest one.");
                Destroy(gameObject);
                return;
            }
            Instance = this;
            DontDestroyOnLoad(gameObject);
            
            base.Awake();

            if (DisableDataPersistence) 
            {
                Debug.LogWarning("Data Persistence is currently disabled!");
            }
            
            InitializeSelectedPlayerId();
        }

        public override void OnActive()
        {
            base.OnActive();
            MessageSystem.MessageManager.RegisterForChannel<SaveLoadMessage>(MessageChannels.Saves, SaveOrLoadMessageHandler);
        }

        public override void OnInactive()
        {
            base.OnInactive();
            MessageSystem.MessageManager.UnregisterForChannel<SaveLoadMessage>(MessageChannels.Saves, SaveOrLoadMessageHandler);
        }

        public override void Start()
        {
            base.Start();
            if (UseAutoSave)
            {
                StartCoroutine(AutoSave());
            }
            
            player = PlayerManager.Instance.Player;

            if (baseGameDataHandler.DataHandler != null) return;
            baseGameDataHandler.DataHandler = new FileDataHandler();
            baseGameDataHandler.DataHandler?.Initialize();
        }
        
        public virtual void DeleteProfileData(SerializableGuid playerID, string playerName)
        {
            if(baseGameDataHandler is not IGameDataHandler handler) return;
            // delete the data for this profile id
            handler.DeleteGameData(playerID, playerName);

        }

        public virtual void InitializeSelectedPlayerId() 
        {
            if(player == null) return;;
            if(baseGameDataHandler is not IGameDataHandler handler) return;
            var mostRecentPlayer = handler.GetMostRecentlyUpdatedPlayer<BaseGameData>();
            if (mostRecentPlayer.gameData == null) return;
            SelectedPlayerName = mostRecentPlayer.gameData.Name;
            SelectedPlayerID = mostRecentPlayer.gameData.ID;
            SelectedPlayerPosition = mostRecentPlayer.gameData.Position;
            SelectedPlayerLevelID = mostRecentPlayer.gameData.CurrentLevelID;
            SelectedPlayerLevelName = mostRecentPlayer.gameData.CurrentLevelName;
            
            MessageSystem.MessageManager.SendImmediate(MessageChannels.Player, new PlayerDataMessage(SelectedPlayerID, player.Name, player.transform.position, player.MoveSpeed, player.CurrentLevelID, player.CurrentLevelName, player.AchievementProgressList));
            if (OverrideSelectedProfileId) 
            {
                SelectedPlayerID = TestSelectedPlayerID;
                Debug.LogWarning("Overrode selected profile id with test id: " + TestSelectedPlayerID);
            }

            if (SelectedPlayerID.Guid == Guid.Empty)
            {
                SelectedPlayerID = player.ID;
            }
        }
        
        public virtual void NewGame() 
        {
            if(baseGameDataHandler is not IGameDataHandler handler) return;
    
            var newGameData = new BaseGameData
            {
                
                ID = SelectedPlayerID,
                Name = SelectedPlayerName,
                Position = SelectedPlayerPosition,
                CurrentLevelID = SelectedPlayerLevelID,
                CurrentLevelName = SelectedPlayerLevelName,
                Achievements = player.AchievementProgressList,
                MoveSpeed = player.MoveSpeed,
                Inventory = new InventoryData(player.Inventory),
                ReferenceState = player.ReferenceState,
                MovementDirection = player.MovementDirection
            };

            var savedGameDatas = new List<BaseGameData>{newGameData};
            // Save the list
            handler.DataHandler.Save<BaseGameData>(savedGameDatas, SelectedPlayerID, SelectedPlayerName);
        }

        
        public virtual void LoadGame(SerializableGuid playerID, string playerName, bool onlyLoadObjects = false)
        {
            // return right away if data persistence is disabled
            if (DisableDataPersistence) 
            {
                return;
            }
            
            if(baseGameDataHandler is not IGameDataHandler handler) return;
            handler.LoadGameData(playerID, playerName);
        }
        
        public virtual void SaveGame(SerializableGuid playerID, string playerName)
        {
            // return right away if data persistence is disabled
            if (DisableDataPersistence) 
            {
                return;
            }
            
            if(baseGameDataHandler is not IGameDataHandler handler) return;
            handler.SaveGameData(playerID, playerName);
        }

        public virtual void OnApplicationQuit() 
        {
            if (UseSaveOnExit)
            {
                SaveGame(SelectedPlayerID, SelectedPlayerName);
            }
        }

        public virtual bool HasGameData() 
        {
            return GetAllProfilesGameData<BaseGameData>().Count > 0;
        }
        
        public virtual Dictionary<SerializableGuid, List<T>> GetAllProfilesGameData<T>() where T : class, IGameDataContent, new()
        {
            if (baseGameDataHandler is IGameDataHandler handler)
            {
                return handler.LoadAllProfiles<T>();
            }
            else
            {
                return new Dictionary<SerializableGuid, List<T>>();
            }
        }


        public virtual IEnumerator AutoSave() 
        {
            while (UseAutoSave)
            {
                if (GameManager.Instance.IsCurrentState<GamePlayingState>())
                {
                    yield return new WaitForSeconds(AutoSaveTimeSeconds);
                    SaveGame(player.ID, player.Name);
                    if (DisplayAutoSaveNotification)
                    {
                        MessageSystem.MessageManager.SendImmediate(MessageChannels.UI, new PopupMessage("Saving...", PopupType.Notification, PopupPosition.Bottom, DisplayAutoSaveNotificationTime));
                    }
                }
                yield return new WaitForSeconds(1f); // Wait for 1 second before the next iteration
            }
        }


        public virtual void SaveOrLoadMessageHandler(MessageSystem.IMessageEnvelope message)
        {
            if(!message.Message<SaveLoadMessage>().HasValue) return;
            var data = message.Message<SaveLoadMessage>().GetValueOrDefault();

            switch (data.SaveOperationType)
            {
                case SaveOperation.Save:
                    SaveGame(data.PlayerID, data.PlayerName);
                    break;
                case SaveOperation.Load:
                    LoadGame(data.PlayerID, data.PlayerName);
                    break;
                case SaveOperation.Delete:
                    DeleteProfileData(data.PlayerID, data.PlayerName);
                    break;
            }
        }
    }
}
