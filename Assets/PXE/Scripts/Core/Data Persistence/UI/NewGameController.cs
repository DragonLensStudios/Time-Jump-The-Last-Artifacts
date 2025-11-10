using System.Collections.Generic;
using PXE.Core.Achievements.Data;
using PXE.Core.Data_Persistence.Messaging.Messages;
using PXE.Core.Enums;
using PXE.Core.Levels;
using PXE.Core.Levels.Messaging.Messages;
using PXE.Core.Messaging;
using PXE.Core.Objects;
using PXE.Core.Player;
using PXE.Core.Player.Managers;
using PXE.Core.SerializableTypes;
using PXE.Core.State_System;
using PXE.Core.State_System.Messaging.Messages;
using PXE.Core.Time.Managers;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace PXE.Core.Data_Persistence.UI
{
    public class NewGameController : ObjectController
    {
        [field: SerializeField] public LevelObject LevelObject { get; set; }
        
        [field: SerializeField] public bool OverridePlayerPosition { get; set; } = false;
        [field: SerializeField] public Vector3 PlayerPosition { get; set; } = Vector3.zero;
        [field: SerializeField] public float PlayerMoveSpeed { get; set; } = 3f;
        [field: SerializeField] public List<PlayerAchievementProgress> PlayerAchievementProgresses { get; set; } = new();
        [field: SerializeField] public GameState State { get; set; } 
        [field: SerializeField] public Button StartGameButton { get; set; } 
        [field: SerializeField] public TMP_InputField NameInputField { get; set; }

        protected PlayerController player;

        public override void OnActive()
        {
            base.OnActive();
            //TODO: Remove dependacy for PlayerManager Here
            player = PlayerManager.Instance.Player;
            if (player == null) return;
            player.ID = SerializableGuid.CreateNew;
            player.SetAchievementProgress();
            PlayerAchievementProgresses = player.AchievementProgressList;
            if (NameInputField != null)
            {
                StartGameButton.interactable = !string.IsNullOrEmpty(NameInputField.text);
            }
        }

        public virtual void SetPlayer(string inputPlayerName)
        {
            if (player == null) return;
            StartGameButton.interactable = !string.IsNullOrEmpty(inputPlayerName);
            MessageSystem.MessageManager.SendImmediate(MessageChannels.Player, new PlayerDataMessage(player.ID, inputPlayerName, PlayerPosition, PlayerMoveSpeed, LevelObject.ID, LevelObject.Name, PlayerAchievementProgresses));
        }
        
        public virtual void LoadLevel()
        {
            if(player == null) return;
            if (string.IsNullOrEmpty(NameInputField.text))
            {
                StartGameButton.interactable = false;
                return;
            }
            //TODO: Remove Dependency for TimeManager Here
            TimeManager.Instance.CurrentTimeObject.ResetFullDate();
            MessageSystem.MessageManager.SendImmediate(MessageChannels.Level, new LevelResetMessage());
            MessageSystem.MessageManager.SendImmediate(MessageChannels.Level,
                OverridePlayerPosition
                    ? new LevelMessage(LevelObject.ID, LevelObject.Name, LevelState.Loading, PlayerPosition)
                    : new LevelMessage(LevelObject.ID, LevelObject.Name, LevelState.Loading, LevelObject.PlayerSpawnPosition));
            MessageSystem.MessageManager.SendImmediate(MessageChannels.GameFlow, new GameStateMessage(State));
            MessageSystem.MessageManager.SendImmediate(MessageChannels.Saves, new SaveLoadMessage(player.ID, player.Name, SaveOperation.Save));
        }
    }
}