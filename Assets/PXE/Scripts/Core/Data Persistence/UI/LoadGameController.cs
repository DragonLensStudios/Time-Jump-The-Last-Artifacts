using System.Linq;
using PXE.Core.Data_Persistence.Data;
using PXE.Core.Data_Persistence.Managers;
using PXE.Core.Data_Persistence.Messaging.Messages;
using PXE.Core.Enums;
using PXE.Core.Messaging;
using PXE.Core.UI;
using PXE.Core.UI.Messaging.Messages;
using UnityEngine;
using UnityEngine.UI;

namespace PXE.Core.Data_Persistence.UI
{
/// <summary>
/// Represents the LoadGameController.
/// The LoadGameController class provides functionality related to loadgamecontroller management.
/// This class contains methods and properties that assist in managing and processing loadgamecontroller related tasks.
/// </summary>
    public class LoadGameController : Page
    {
        [SerializeField] private GameObject loadPanelPrefab;
        [SerializeField] private Transform loadPanelContent;
        [SerializeField] private Button deleteAllSavesButton;
        
        public override void OnActive()
        {
            base.OnActive();
            // var contentOc = loadPanelContent.GetComponent<ObjectController>();
            // if (contentOc != null)
            // {
            //     contentOc.ControlChildrenActiveState = true;
            //     contentOc.SetObjectActive(true);
            // }
            ClearAndSpawnLoadGamePanels();
            MessageSystem.MessageManager.RegisterForChannel<SaveLoadMessage>(MessageChannels.Saves, SaveLoadMessageHandler);
        }

        public override void OnInactive()
        {
            base.OnInactive();
            // var contentOc = loadPanelContent.GetComponent<ObjectController>();
            // if (contentOc != null)
            // {
            //     contentOc.ControlChildrenActiveState = true;
            //     contentOc.SetObjectActive(false);
            // }
            MessageSystem.MessageManager.UnregisterForChannel<SaveLoadMessage>(MessageChannels.Saves, SaveLoadMessageHandler);
        }


        /// <summary>
        /// Executes the ClearAndSpawnLoadGamePanels method.
        /// Handles the ClearAndSpawnLoadGamePanels functionality.
        /// </summary>
        public virtual void ClearAndSpawnLoadGamePanels()
        {
            for (int i = loadPanelContent.childCount - 1; i >= 0; i--)
            {
                Destroy(loadPanelContent.GetChild(i).gameObject);
            }

            //TODO: Remove dependacy for DataPersistenceManager Here
            var allProfilesData = DataPersistenceManager.Instance.GetAllProfilesGameData<BaseGameData>();

            // Flatten the dictionary and sort by the most recent LastUpdated date
            
            var orderedData = allProfilesData
                .SelectMany(pair => pair.Value, (pair, gameData) => new { pair.Key, GameData = gameData })
                .Where(entry => entry.Key.Guid == entry.GameData.ID.Guid)
                .Distinct()
                .OrderByDescending(entry => entry.GameData.LastUpdated).ToList();

            for (int i = 0; i < orderedData.Count; i++)
            {
                var entry = orderedData[i];
                var save = Instantiate(loadPanelPrefab, loadPanelContent);
                var saveSlot = save.GetComponent<SaveSlot>();
                saveSlot.PlayerID = entry.Key;
                saveSlot.PlayerName = entry.GameData.Name;
                saveSlot.CurrentLevelID = entry.GameData.CurrentLevelID;
                saveSlot.CurrentLevelName = entry.GameData.CurrentLevelName;
                saveSlot.PlayerNameText.text = entry.GameData.Name;
                saveSlot.PlayerPosition = entry.GameData.Position;
                saveSlot.TimestampText.text = entry.GameData.LastUpdated.ToString("M/d/yyyy h:mm tt");
                
            }

            if (deleteAllSavesButton != null)
            {
                deleteAllSavesButton.interactable = orderedData.Any();
            }
        }


        public virtual void DeleteAllSaves()
        {
            //TODO: Remove dependacy for DataPersistenceManager Here
            var allProfilesData = DataPersistenceManager.Instance.GetAllProfilesGameData<BaseGameData>();
            
            var matchingData = allProfilesData
                .SelectMany(pair => pair.Value, (pair, gameData) => new { PlayerID = pair.Key, GameData = gameData })
                .Where(entry => entry.GameData.ID == entry.PlayerID)
                .OrderByDescending(entry => entry.GameData.LastUpdated);

            
            // Get the most recent LastUpdated date for each player
            // var orderedData = allProfilesData
            //     .SelectMany(pair => pair.Value, (pair, gameData) => new { pair.Key, GameData = gameData })
            //     .OrderByDescending(entry => entry.GameData.LastUpdated);

            MessageSystem.MessageManager.SendImmediate(MessageChannels.UI, new PopupMessage($"Are you sure you want to delete all {allProfilesData.Count()} saves?", PopupType.Confirm, PopupPosition.Middle, 0, null, () => {}, () =>
            {
                foreach (var entry in matchingData)
                {
                    MessageSystem.MessageManager.SendImmediate(MessageChannels.Saves,
                        new SaveLoadMessage(entry.GameData.ID, entry.GameData.Name, SaveOperation.Delete));
                }
                deleteAllSavesButton.interactable = false;
            }));
        }

        
        public virtual void SaveLoadMessageHandler(MessageSystem.IMessageEnvelope message)
        {
            if(!message.Message<SaveLoadMessage>().HasValue) return;
            var data = message.Message<SaveLoadMessage>().GetValueOrDefault();
            if (data.SaveOperationType == SaveOperation.Delete)
            {
                ClearAndSpawnLoadGamePanels();
            }
        }
    }
}
