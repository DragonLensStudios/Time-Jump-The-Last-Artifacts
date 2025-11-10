using System;
using System.Collections.Generic;
using PXE.Core.Achievements.Messaging.Messages;
using PXE.Core.Audio;
using PXE.Core.Audio.Messaging.Messages;
using PXE.Core.Data_Persistence.Data;
using PXE.Core.Data_Persistence.Interfaces;
using PXE.Core.Data_Persistence.Managers;
using PXE.Core.Data_Persistence.Messaging.Messages;
using PXE.Core.Enums;
using PXE.Core.Extensions.GameObjectExtensions;
using PXE.Core.Game.Managers;
using PXE.Core.Levels;
using PXE.Core.Levels.Managers;
using PXE.Core.Levels.Messaging.Messages;
using PXE.Core.Messaging;
using PXE.Core.Objects;
using PXE.Core.Player.Managers;
using PXE.Core.State_System;
using PXE.Core.State_System.Messaging.Messages;
using PXE.Core.Transition;
using PXE.Core.Transition.Messaging.Messages;
using PXE.Core.UI.Messaging.Messages;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace PXE.Core.UI
{
    [DisallowMultipleComponent]
    public class MenuController : ObjectController
    {
        [SerializeField] private Page InitialPage;
        [SerializeField] private GameObject FirstFocusItem;
        [SerializeField] private Button continueButton, loadButton;
        [SerializeField] private TransitionParameters transitionParameters;

        private Canvas RootCanvas;

        private Stack<Page> PageStack = new ();

        // private EventSystem _eventSystem;

        public override void Awake()
        {
            base.Awake();
            RootCanvas = GetComponent<Canvas>();
            // _eventSystem = FindObjectOfType<EventSystem>();
        }

        public override void Start()
        {
            base.Start();
            
            if (FirstFocusItem != null)
            {
                // _eventSystem.SetSelectedGameObject(null);
                // _eventSystem.SetSelectedGameObject(FirstFocusItem);
                EventSystem.current.SetSelectedGameObject(null);
                EventSystem.current.SetSelectedGameObject(FirstFocusItem);
            }

            if (InitialPage != null)
            {
                PushPage(InitialPage);
            }

            SetLoadContinueButtonInteractable();
        }

        public override void OnActive()
        {
            MessageSystem.MessageManager.RegisterForChannel<PageMessage>(MessageChannels.UI, PageMessageHandler);
        }
        public override void OnInactive()
        {
            MessageSystem.MessageManager.UnregisterForChannel<PageMessage>(MessageChannels.UI, PageMessageHandler);
        }

        public virtual void SetLoadContinueButtonInteractable()
        {
            //TODO: Remove dependacy for GameManager Here
            if (GameManager.Instance.IsCurrentState<PausedState>()) return;
            if(continueButton != null)
            {
                //TODO: Remove dependacy for DataPersistenceManager Here
                continueButton.interactable = DataPersistenceManager.Instance.HasGameData();
            }

            if (loadButton != null)
            {
                //TODO: Remove dependacy for DataPersistenceManager Here
                loadButton.interactable = DataPersistenceManager.Instance.HasGameData();
            }
        }

        public virtual void OnCancel()
        {
            if (!RootCanvas.enabled || !RootCanvas.gameObject.IsObjectActive()) return;
            if (PageStack.Count != 0)
            {
                PopPage();
            }
        }
        public virtual bool IsPageInStack(Page page)
        {
            return PageStack.Contains(page);
        }
        
        public virtual bool IsPageOnTopOfStack(Page page)
        {
            return PageStack.Count > 0 && page == PageStack.Peek();
        }
        
        // public virtual Page LinkPage(Page pageOrPrefab)
        // {
        //     if (IsPageInStack(pageOrPrefab)) return pageOrPrefab;
        //     var parent = pageOrPrefab.transform.parent;
        //     
        //     foreach(Page page in PageStack)
        //     {
        //         // Debug.Log(page.PrefabID + " ==? " + prefabID);
        //         // TODO make sure going from top most window down to root
        //         if (page.transform == parent) return page;
        //     }
        //
        //     for (int i = 0; i < RootCanvas.transform.childCount; i++)
        //     {
        //         Page sibling;
        //         if (RootCanvas.transform.GetChild(i).TryGetComponent<Page>(out sibling))
        //         {
        //             if (sibling != null && sibling.transform == parent) return sibling;
        //         }
        //     }
        //
        //     if (PageStack.Count > 0) // this is to fix a bug with main menu panel
        //     {
        //         Page pageNew = Instantiate(pageOrPrefab, Vector3.zero, Quaternion.identity);
        //         pageNew.transform.SetParent(RootCanvas.transform, false);
        //         pageNew.transform.localScale = new Vector3(1, 1, 1);
        //         return pageNew;
        //     }
        //     return pageOrPrefab;
        // }
        public virtual void PushPage(Page page)
        {
            // page = LinkPage(page);
            page.Enter(true);

            if (PageStack.Count > 0)
            {
                Page currentPage = PageStack.Peek();

                if (currentPage.ExitOnNewPagePush)
                {
                    currentPage.Exit(false);
                }
            }

            PageStack.Push(page);
            // var canvas = page.GetComponent<Canvas>();
            // canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            // canvas.overrideSorting = true;
            // canvas.sortingOrder = PageStack.Count;
        }
        
        public virtual void PopPage()
        {
            if (PageStack.Count > 1)
            {
                Page page = PageStack.Pop();
                page.Exit(true);

                Page newCurrentPage = PageStack.Peek();
                if (newCurrentPage.ExitOnNewPagePush)
                {
                    newCurrentPage.Enter(false);
                }
            }
            else
            {
                Debug.LogWarning("Trying to pop a page but only 1 page remains in the stack!");
            }
        }
        
        public virtual void PopAllPages()
        {
            for (int i = 1; i < PageStack.Count; i++)
            {
                PopPage();
            }
        }
        
        public virtual void OpenURL(string url)
        {
            Application.OpenURL(url);
        }
        
        public virtual void LoadScene(string sceneName)
        {
            SceneManager.LoadSceneAsync(sceneName);
        }
        
        public virtual void ReloadScene()
        {
            SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().buildIndex);
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
        
        public virtual void Unpause()
        {
            MessageSystem.MessageManager.SendImmediate(MessageChannels.GameFlow, new PauseMessage(this, false));
        }
        
        public virtual void LoadMainMenu()
        {
            MessageSystem.MessageManager.SendImmediate(MessageChannels.Level, new LevelMessage(LevelManager.Instance.CurrentLevelObject.ID, LevelManager.Instance.CurrentLevelObject.Name, LevelState.Unloading, Vector2.zero));
            MessageSystem.MessageManager.SendImmediate(MessageChannels.GameFlow, new GameStateMessage(GameManager.Instance.GetStateByType<MainMenuState>()));
        }
        
        public virtual void ContinueGame(GameState state)
        {
            var player = PlayerManager.Instance.Player;
            
            //TODO: Remove dependacy for DataPersistenceManager Here
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

        public virtual void RetryGame(GameState state)
        {
            //TODO: Remove dependacy for PlayerManager Here
            var player = PlayerManager.Instance.Player;
            //TODO: Remove dependacy for LevelManager Here
            var level = LevelManager.Instance.CurrentLevelObject;
            MessageSystem.MessageManager.SendImmediate(MessageChannels.Level, new LevelResetMessage());
            MessageSystem.MessageManager.SendImmediate(MessageChannels.Level, new LevelMessage(player.CurrentLevelID, player.CurrentLevelName, LevelState.Loading, level.PlayerSpawnPosition));
            MessageSystem.MessageManager.SendImmediate(MessageChannels.GameFlow, new GameStateMessage(state));
            MessageSystem.MessageManager.SendImmediate(MessageChannels.GameFlow, new PauseMessage(this, false));
        }
        
        public virtual void Transition()
        {
            // Create a new transition message
            var transitionMessage = new TransitionMessage(transitionParameters.transitionType, transitionParameters.slideDirection,
                transitionParameters.animationDurationInSeconds, transitionParameters.endEvent);

            // Send the message
            MessageSystem.MessageManager.SendImmediate(MessageChannels.UI, transitionMessage);
        }
        
        public virtual void OpenAchievementsMenu()
        {
            MessageSystem.MessageManager.SendImmediate(MessageChannels.UI, new AchievementMenuMessage(true));
        }

        public virtual void PlaySFX(AudioObject audioObj)
        {
            MessageSystem.MessageManager.SendImmediate(MessageChannels.Audio, new AudioMessage(audioObj, AudioOperation.Play, AudioChannel.SoundEffects));
        }

        public virtual void SaveGame()
        {
            MessageSystem.MessageManager.SendImmediate(MessageChannels.Saves, new SaveLoadMessage(PlayerManager.Instance.Player.ID, PlayerManager.Instance.Player.Name, SaveOperation.Save));
            MessageSystem.MessageManager.SendImmediate(MessageChannels.UI, new PopupMessage("Game Saved!", PopupType.Notification, PopupPosition.Middle, 1, null, null, null));
        }
        
        public virtual void LoadLevel(LevelObject levelObject)
        {
            MessageSystem.MessageManager.SendImmediate(MessageChannels.Level, new LevelMessage(levelObject.ID, levelObject.Name, LevelState.Loading, levelObject.PlayerSpawnPosition));
        }
        
        public virtual void ChangeState(GameState state)
        {
            MessageSystem.MessageManager.SendImmediate(MessageChannels.GameFlow, new GameStateMessage(state));
        }

        public virtual void PageMessageHandler(MessageSystem.IMessageEnvelope message)
        {
            if(!message.Message<PageMessage>().HasValue) return;
            var data = message.Message<PageMessage>().GetValueOrDefault();

            switch(data.PageOperation)
            {
                case PageOperation.Push:
                    if (data.Page == null) return;
                    PushPage(data.Page);
                break;
                case PageOperation.Pop:
                    PopPage();
                break;
                case PageOperation.PopAll:
                    Debug.Log(data);
                    PopAllPages();
                break;
            }
        }
    }
}
