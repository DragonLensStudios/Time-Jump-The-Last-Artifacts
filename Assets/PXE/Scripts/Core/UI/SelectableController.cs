using System;
using PXE.Core.Audio;
using PXE.Core.Audio.Messaging.Messages;
using PXE.Core.Data_Persistence.Data;
using PXE.Core.Data_Persistence.Interfaces;
using PXE.Core.Data_Persistence.Managers;
using PXE.Core.Data_Persistence.Messaging.Messages;
using PXE.Core.Enums;
using PXE.Core.Game.Managers;
using PXE.Core.Levels;
using PXE.Core.Levels.Managers;
using PXE.Core.Levels.Messaging.Messages;
using PXE.Core.Messaging;
using PXE.Core.Objects;
using PXE.Core.Player.Managers;
using PXE.Core.State_System;
using PXE.Core.State_System.Messaging.Messages;
using PXE.Core.UI.Messaging.Messages;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace PXE.Core.UI
{
    //TODO: Refactor this class to be more generic and reusable and not tied to specific implementation
    public class SelectableController : ObjectController, IPointerEnterHandler
    {
        [field: SerializeField] public virtual Selectable Selectable { get; set; }

        public override void Start()
        {
            base.Start();
            if (Selectable == null)
            {
                Selectable = GetComponent<Selectable>();
            }
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (EventSystem.current == null) Debug.LogError("EventSystem.current is broken!");
            EventSystem.current.SetSelectedGameObject(null);
            EventSystem.current.SetSelectedGameObject(Selectable?.gameObject);
        }

        public virtual void PushPage(Page page)
        {
            MessageSystem.MessageManager.SendImmediate(MessageChannels.UI, new PageMessage(PageOperation.Push, page));
        }
       public virtual void PopPage()
        {
            MessageSystem.MessageManager.SendImmediate(MessageChannels.UI, new PageMessage(PageOperation.Pop));
        }
        public virtual void PopAllPages()
        {
            MessageSystem.MessageManager.SendImmediate(MessageChannels.UI, new PageMessage(PageOperation.PopAll));
        }

        public virtual void ChangeState(GameState state)
        {
            MessageSystem.MessageManager.SendImmediate(MessageChannels.GameFlow, new GameStateMessage(state));
        }

        public virtual void Exit()
        {
            try
            {
                #if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
                #else
                Application.Quit();
                #endif
            }
            catch (Exception)
            {
                LoadMainMenu();
            }
        }

        public virtual void LoadMainMenu()
        {
            MessageSystem.MessageManager.SendImmediate(MessageChannels.Level, new LevelMessage(LevelManager.Instance.CurrentLevelObject.ID, LevelManager.Instance.CurrentLevelObject.Name, LevelState.Unloading, Vector2.zero));
            MessageSystem.MessageManager.SendImmediate(MessageChannels.GameFlow, new GameStateMessage(GameManager.Instance.GetStateByType<MainMenuState>()));
        }
    
        public virtual void LoadLevel(LevelObject levelObject)
        {
            MessageSystem.MessageManager.SendImmediate(MessageChannels.Level, new LevelMessage(levelObject.ID, levelObject.Name, LevelState.Loading, levelObject.PlayerSpawnPosition));
        }
        public virtual void RetryGame(GameState state)
        {
            var player = PlayerManager.Instance.Player;
            var level = LevelManager.Instance.CurrentLevelObject;
            MessageSystem.MessageManager.SendImmediate(MessageChannels.Level, new LevelResetMessage());
            MessageSystem.MessageManager.SendImmediate(MessageChannels.Level, new LevelMessage(player.CurrentLevelID, player.CurrentLevelName, LevelState.Loading, level.PlayerSpawnPosition));
            MessageSystem.MessageManager.SendImmediate(MessageChannels.GameFlow, new GameStateMessage(state));
            MessageSystem.MessageManager.SendImmediate(MessageChannels.GameFlow, new PauseMessage(this, false));
        }

        public virtual void ContinueGame(GameState state)
        {
            var player = PlayerManager.Instance.Player;
            
            if(DataPersistenceManager.Instance.baseGameDataHandler is not IGameDataHandler handler) return;
            var mostRecentPlayer = handler.GetMostRecentlyUpdatedPlayer<BaseGameData>();
            player.ID = mostRecentPlayer.playerID;
            player.Name = mostRecentPlayer.gameData.Name;
            player.transform.position = mostRecentPlayer.gameData.Position;
            player.CurrentLevelID = mostRecentPlayer.gameData.CurrentLevelID;
            player.CurrentLevelName = mostRecentPlayer.gameData.CurrentLevelName;
            player.AchievementProgressList = mostRecentPlayer.gameData.Achievements;
            MessageSystem.MessageManager.SendImmediate(MessageChannels.Saves, new SaveLoadMessage(player.ID, player.Name, SaveOperation.Load));
            MessageSystem.MessageManager.SendImmediate(MessageChannels.Level, new LevelMessage(player.CurrentLevelID, player.CurrentLevelName, LevelState.Loading, player.transform.position));
            MessageSystem.MessageManager.SendImmediate(MessageChannels.GameFlow, new GameStateMessage(state));
            MessageSystem.MessageManager.SendImmediate(MessageChannels.GameFlow, new PauseMessage(this, false));
        }
        
        public virtual void PlaySFX(AudioObject audioObj)
        {
            MessageSystem.MessageManager.SendImmediate(MessageChannels.Audio, new AudioMessage(audioObj, AudioOperation.Play, AudioChannel.SoundEffects));
        }
        
        public virtual void UnpauseGame()
        {
            MessageSystem.MessageManager.SendImmediate(MessageChannels.GameFlow, new GameStateMessage(GameManager.Instance.GetStateByType<GamePlayingState>()));
            MessageSystem.MessageManager.SendImmediate(MessageChannels.GameFlow, new PauseMessage(this, false));
        }

    }
}