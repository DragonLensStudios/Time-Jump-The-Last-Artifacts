using System.Collections.Generic;
using System.Linq;
using PXE.Core.Dialogue.UI;
using PXE.Core.Enums;
using PXE.Core.Messaging;
using PXE.Core.Objects;
using PXE.Core.State_System;
using PXE.Core.State_System.Messaging.Messages;
using UnityEngine;
using UnityEngine.InputSystem;

namespace PXE.Core.Game.Managers
{
    public class GameManager : ObjectController
    {
        private static GameManager instance;
        public static GameManager Instance
        {
            get
            {
                if (instance != null) return instance;
                instance = FindFirstObjectByType<GameManager>(FindObjectsInactive.Include);

                if (instance != null) return instance;
                GameObject managerObject = new GameObject("Game Manager");
                instance = managerObject.AddComponent<GameManager>();

                return instance;
            }
        }

        [field: SerializeField, HideInInspector] public virtual GameState CurrentState { get; set; }
        [field: SerializeField, HideInInspector] public virtual GameState InitialState { get; set; }
        [field: SerializeField, HideInInspector] public virtual List<GameState> AllStates { get; set; } = new();

        protected PlayerInputActions playerInput;
        
        public override void OnActive()
        {
            base.OnActive();
            playerInput ??= new PlayerInputActions();
            playerInput.Enable();
            playerInput.Player.Pause.performed += PauseOnperformed;
            playerInput.Player.ToggleInventory.performed += ToggleInventoryOnperformed;
            MessageSystem.MessageManager.RegisterForChannel<GameStateMessage>(MessageChannels.GameFlow, ChangeStateHandler);
            MessageSystem.MessageManager.RegisterForChannel<PauseMessage>(MessageChannels.GameFlow, PausedMessageHandler);
        }

        public override void OnInactive()
        {
            base.OnInactive();
            playerInput ??= new PlayerInputActions();
            playerInput.Disable();
            playerInput.Player.Pause.performed -= PauseOnperformed;
            playerInput.Player.ToggleInventory.performed -= ToggleInventoryOnperformed;


            MessageSystem.MessageManager.UnregisterForChannel<GameStateMessage>(MessageChannels.GameFlow, ChangeStateHandler);
            MessageSystem.MessageManager.UnregisterForChannel<PauseMessage>(MessageChannels.GameFlow, PausedMessageHandler);
        }

        public virtual void ToggleInventoryOnperformed(InputAction.CallbackContext input)
        {
            if (!IsCurrentState<PausedState>() && !IsCurrentState<MainMenuState>() && !IsCurrentState<InventoryState>())
            {
                SetState(GetStateByType<InventoryState>());
            }
            else if (IsCurrentState<InventoryState>())
            {
                SetState(GetStateByType<GamePlayingState>());
            }
        }

        public override void Awake()
        {
            if (instance == null)
            {
                instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
            base.Awake();

            if (AllStates.Count <= 0)
            {
                AllStates = Resources.LoadAll<GameState>("States").ToList();
            }

            playerInput = new PlayerInputActions();

            
        }
        
        public override void Start()
        {
            base.Start();
            if (InitialState == null) return;
            SetState(InitialState, true);
        }

        public override void Update()
        {
            base.Update();
            if (CurrentState != null)
                CurrentState.Update();
        }
        
        public virtual void SetState(GameState newState, bool isStartingGame = false)
        {
            if (CurrentState != null && CurrentState != newState)
            {
                CurrentState.Exit();
            }
            
            CurrentState = newState;
            
            if (CurrentState != null && (!isStartingGame || (isStartingGame && CurrentState == InitialState)))
            {
                CurrentState.Enter();
            }
        }
        
        //TODO: Refactor or move this code so that it is not needed to be called with the singleton instance.
        public virtual bool IsCurrentState<T>() where T : GameState
        {
            return CurrentState is T;
        }
        
        //TODO: Refactor or move this code so that it is not needed to be called with the singleton instance.
        public virtual GameState GetStateByType<T>() where T : GameState
        {
            return AllStates.FirstOrDefault(x => x.GetType() == typeof(T));
        }    
        public virtual void ChangeStateHandler(MessageSystem.IMessageEnvelope message)
        {
            if(!message.Message<GameStateMessage>().HasValue) return;
            var data = message.Message<GameStateMessage>().GetValueOrDefault();
            SetState(data.State);
        }
        
        public virtual void PausedMessageHandler(MessageSystem.IMessageEnvelope message)
        {
            if(!message.Message<PauseMessage>().HasValue) return;
            var data = message.Message<PauseMessage>().GetValueOrDefault();
            var gameplayState = GetStateByType<GamePlayingState>();
            if (IsCurrentState<PausedState>())
            {
                if (!data.IsPaused)
                {
                    if (gameplayState != null)
                    {
                        SetState(gameplayState);
                    }
                }
            }
            if(!data.ShowPauseMenu) return;
            if(IsCurrentState<MainMenuState>()) return;
            if (!IsCurrentState<GamePlayingState>()) return;
            var pausedState = GetStateByType<PausedState>();
            if (pausedState != null)
            {
                SetState(pausedState);
            }

        }
        
        public virtual void PauseOnperformed(InputAction.CallbackContext input)
        {
            var dialogueOpen = DialogueUi.Instance.IsDialogueOpen;
            Debug.Log("Dialogue Open: " + dialogueOpen);
            if (!IsCurrentState<PausedState>() && !IsCurrentState<MainMenuState>() && !IsCurrentState<InventoryState>() && !dialogueOpen)
            {
                MessageSystem.MessageManager.SendImmediate(MessageChannels.GameFlow, new PauseMessage(this,true, true));
            }
            else if (IsCurrentState<PausedState>())
            {
                MessageSystem.MessageManager.SendImmediate(MessageChannels.GameFlow, new PauseMessage(this, false));
            }
        }
    }
}
