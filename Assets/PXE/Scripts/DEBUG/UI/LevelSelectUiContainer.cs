using PXE.Core.Enums;
using PXE.Core.Game.Managers;
using PXE.Core.Levels;
using PXE.Core.Levels.Messaging.Messages;
using PXE.Core.Messaging;
using PXE.Core.Objects;
using PXE.Core.State_System;
using PXE.Core.State_System.Messaging.Messages;
using TMPro;
using UnityEngine;

namespace PXE.DEBUG.UI
{
    public class LevelSelectUiContainer : ObjectController
    {
        [field: Tooltip("The text object that displays the level name.")]
        [field: SerializeField] public TMP_Text LevelNameText { get; set; }
        
        [field: Tooltip("The selected level.")]
        [field: SerializeField] public LevelObject SelectedLevelObject { get; set; }

        /// <summary>
        ///  Executes the LevelSelectUiContainer method and sets the level.
        /// </summary>
        /// <param name="levelObject"></param>
        public void SetLevel(LevelObject levelObject)
        {
            SelectedLevelObject = levelObject;
            if (SelectedLevelObject != null && LevelNameText != null)
            {
                LevelNameText.text = $"Level: {levelObject.Name}";
            }
        }

        /// <summary>
        ///  Executes the LevelSelectUiContainer method and calls level messages to load the level.
        /// </summary>
        public void LoadLevel()
        {
            if (SelectedLevelObject == null) return;
            //TODO: Remove dependacy for DebugManager Here
            DebugManager.Instance.ShowDebugMenu(false);
            MessageSystem.MessageManager.SendImmediate(MessageChannels.Level, new LevelResetMessage());
            MessageSystem.MessageManager.SendImmediate(MessageChannels.Level, new LevelMessage(SelectedLevelObject.ID, SelectedLevelObject.Name, LevelState.Loading, SelectedLevelObject.PlayerSpawnPosition));
            MessageSystem.MessageManager.SendImmediate(MessageChannels.GameFlow, new GameStateMessage(GameManager.Instance.GetStateByType<GamePlayingState>()));
        }
    }
}