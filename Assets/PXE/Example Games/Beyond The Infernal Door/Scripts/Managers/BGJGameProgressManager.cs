using PXE.Core.Enums;
using PXE.Core.Game.Managers;
using PXE.Core.Inventory.Items;
using PXE.Core.Levels.Managers;
using PXE.Core.Levels.Messaging.Messages;
using PXE.Core.Messaging;
using PXE.Core.Objects;
using PXE.Core.State_System;
using PXE.Core.State_System.Messaging.Messages;
using PXE.Core.UI.Messaging.Messages;
using PXE.Core.Variables;
using PXE.Example_Games.Beyond_The_Infernal_Door.Scripts.Messaging.Messages;
using UnityEngine;

namespace PXE.Example_Games.Beyond_The_Infernal_Door.Scripts.Managers
{
    public class BGJGameProgressManager : ObjectController
    {
        public static BGJGameProgressManager Instance { get; private set; }
    
        [field: SerializeField] public InventoryObject PlayerInventory { get; set; }
    
        [field: SerializeField] public VariablesObject GameVariables { get; set; }

        [field: SerializeField] public int RequiredWrathProgress { get; set; } = 3;
        [field: SerializeField] public int RequiredSlothProgress { get; set; } = 3;
        [field: SerializeField] public int RequiredPrideProgress { get; set; } = 3;
    
        [field: SerializeField] public int WrathFailureToGameOver { get; set; } = 3;
        [field: SerializeField] public int SlothFailureToGameOver { get; set; } = 3;
        [field: SerializeField] public int PrideFailureToGameOver { get; set; } = 3;
    
        [field: SerializeField] public bool WrathProgressComplete { get; set; }
        [field: SerializeField] public bool WrathProgressFailed { get; set; }
        [field: SerializeField] public bool SlothProgressComplete { get; set; }
        [field: SerializeField] public bool SlothProgressFailed { get; set; }
        [field: SerializeField] public bool PrideProgressComplete { get; set; }
        [field: SerializeField] public bool PrideProgressFailed { get; set; }
        [field: SerializeField] public virtual bool IsGameOver { get; set; }
    
        [field: SerializeField] public ItemObject WrathKey { get; set; }
        [field: SerializeField] public ItemObject SlothKey { get; set; }
        [field: SerializeField] public ItemObject PrideKey { get; set; }
        [field: SerializeField] public ItemObject TrueKey { get; set; }

        public override void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
                PlayerInventory.Initialize();
            }
            else
            {
                Destroy(gameObject);
            }
            base.Awake();
        }
    
        public override void Start()
        {
            base.Start();
            if (GameVariables == null) return;
            IsGameOver = false;
            GameVariables.Reset();
            GameVariables.BoolVariables.AddVariable("IsGameOver", IsGameOver);
            GameVariables.IntVariables.AddVariable("WrathProgress", 0);
            GameVariables.IntVariables.AddVariable("WrathFailure", 0);
            GameVariables.IntVariables.AddVariable("SlothProgress", 0);
            GameVariables.IntVariables.AddVariable("SlothFailure", 0);
            GameVariables.IntVariables.AddVariable("PrideProgress", 0);
            GameVariables.IntVariables.AddVariable("PrideFailure", 0);
        }
    
        public override void OnActive()
        {
            base.OnActive();
            MessageSystem.MessageManager.RegisterForChannel<BGJProgressMessage>(MessageChannels.Gameplay, BGJProgressMessageHandler);
        }
        
        public override void OnInactive()
        {
            base.OnInactive();
            MessageSystem.MessageManager.UnregisterForChannel<BGJProgressMessage>(MessageChannels.Gameplay, BGJProgressMessageHandler);
        }
        
        private void BGJProgressMessageHandler(MessageSystem.IMessageEnvelope message)
        {
            if(!message.Message<BGJProgressMessage>().HasValue) return;
            var data = message.Message<BGJProgressMessage>().GetValueOrDefault();
            if (GameVariables == null) return;
            IsGameOver = data.IsGameOver;
            GameVariables.BoolVariables["IsGameOver"].Value = IsGameOver;
        
            if (IsGameOver)
            {
                MessageSystem.MessageManager.SendImmediate(MessageChannels.UI, new PopupMessage("You have completed the game!\nBut was the ending you got correct ending?", PopupType.Message, PopupPosition.Middle, 0f, 
                    () => 
                    {
                        IsGameOver = false;
                        GameVariables.Reset();
                        GameVariables.BoolVariables.AddVariable("IsGameOver", false);
                        GameVariables.IntVariables.AddVariable("WrathProgress", 0);
                        GameVariables.IntVariables.AddVariable("WrathFailure", 0);
                        GameVariables.IntVariables.AddVariable("SlothProgress", 0);
                        GameVariables.IntVariables.AddVariable("SlothFailure", 0);
                        GameVariables.IntVariables.AddVariable("PrideProgress", 0);
                        GameVariables.IntVariables.AddVariable("PrideFailure", 0);
                        PlayerInventory.Items.Clear();
                        MessageSystem.MessageManager.SendImmediate(MessageChannels.Level, new LevelMessage(LevelManager.Instance.CurrentLevelObject.ID, LevelManager.Instance.CurrentLevelObject.Name, LevelState.Unloading, Vector3.zero));
                        MessageSystem.MessageManager.SendImmediate(MessageChannels.GameFlow, new GameStateMessage(GameManager.Instance.GetStateByType<MainMenuState>()));
                    }));
                return;
            }
        
            WrathProgressComplete = GameVariables.IntVariables["WrathProgress"].Value >= RequiredWrathProgress;
            SlothProgressComplete = GameVariables.IntVariables["SlothProgress"].Value >= RequiredSlothProgress;
            PrideProgressComplete = GameVariables.IntVariables["PrideProgress"].Value >= RequiredPrideProgress;
        
            WrathProgressFailed = GameVariables.IntVariables["WrathFailure"].Value >= WrathFailureToGameOver;
            SlothProgressFailed = GameVariables.IntVariables["SlothFailure"].Value >= SlothFailureToGameOver;
            PrideProgressFailed = GameVariables.IntVariables["PrideFailure"].Value >= PrideFailureToGameOver;

            if (WrathProgressFailed)
            {
                IsGameOver = true;
                GameVariables.BoolVariables["IsGameOver"].Value = IsGameOver;
                MessageSystem.MessageManager.SendImmediate(MessageChannels.UI, new PopupMessage("You have failed the game!\nYou have too much wrath in your heart.", PopupType.Message, PopupPosition.Middle, 0f, 
                    () => 
                    {
                        IsGameOver = false;
                        GameVariables.Reset();
                        GameVariables.BoolVariables.AddVariable("IsGameOver", false);
                        GameVariables.IntVariables.AddVariable("WrathProgress", 0);
                        GameVariables.IntVariables.AddVariable("WrathFailure", 0);
                        GameVariables.IntVariables.AddVariable("SlothProgress", 0);
                        GameVariables.IntVariables.AddVariable("SlothFailure", 0);
                        GameVariables.IntVariables.AddVariable("PrideProgress", 0);
                        GameVariables.IntVariables.AddVariable("PrideFailure", 0);
                        PlayerInventory.Items.Clear();
                        MessageSystem.MessageManager.SendImmediate(MessageChannels.Level, new LevelMessage(LevelManager.Instance.CurrentLevelObject.ID, LevelManager.Instance.CurrentLevelObject.Name, LevelState.Unloading, Vector3.zero));
                        MessageSystem.MessageManager.SendImmediate(MessageChannels.GameFlow, new GameStateMessage(GameManager.Instance.GetStateByType<MainMenuState>()));
                    }));
                return;
            }

            if (WrathProgressComplete)
            {
                if(PlayerInventory != null)
                {
                    var wrathKey = PlayerInventory.ContainsItem(WrathKey);
                    if (!wrathKey)
                    {
                        PlayerInventory.AddItem(WrathKey, 1);
                    }
                }
            }
        
            if(SlothProgressFailed)
            {
                IsGameOver = true;
                GameVariables.BoolVariables["IsGameOver"].Value = IsGameOver;
                MessageSystem.MessageManager.SendImmediate(MessageChannels.UI, new PopupMessage("You have failed the game!\nYou have too much sloth in your heart.", PopupType.Message, PopupPosition.Middle, 0f, 
                    () => 
                    {
                        IsGameOver = false;
                        GameVariables.Reset();
                        GameVariables.BoolVariables.AddVariable("IsGameOver", false);
                        GameVariables.IntVariables.AddVariable("WrathProgress", 0);
                        GameVariables.IntVariables.AddVariable("WrathFailure", 0);
                        GameVariables.IntVariables.AddVariable("SlothProgress", 0);
                        GameVariables.IntVariables.AddVariable("SlothFailure", 0);
                        GameVariables.IntVariables.AddVariable("PrideProgress", 0);
                        GameVariables.IntVariables.AddVariable("PrideFailure", 0);
                        PlayerInventory.Items.Clear();
                        MessageSystem.MessageManager.SendImmediate(MessageChannels.Level, new LevelMessage(LevelManager.Instance.CurrentLevelObject.ID, LevelManager.Instance.CurrentLevelObject.Name, LevelState.Unloading, Vector3.zero));
                        MessageSystem.MessageManager.SendImmediate(MessageChannels.GameFlow, new GameStateMessage(GameManager.Instance.GetStateByType<MainMenuState>()));
                    }));
                return;
            }
        
            if (SlothProgressComplete)
            {
                if(PlayerInventory != null)
                {
                    var slothKey = PlayerInventory.ContainsItem(SlothKey);
                    if (!slothKey)
                    {
                        PlayerInventory.AddItem(SlothKey, 1);
                    }
                }
            }
        
            if(PrideProgressFailed)
            {
                IsGameOver = true;
                GameVariables.BoolVariables["IsGameOver"].Value = IsGameOver;
                MessageSystem.MessageManager.SendImmediate(MessageChannels.UI, new PopupMessage("You have failed the game!\nYou have too much pride in your heart.", PopupType.Message, PopupPosition.Middle, 0f, 
                    () => 
                    {
                        IsGameOver = false;
                        GameVariables.Reset();
                        GameVariables.BoolVariables.AddVariable("IsGameOver", false);
                        GameVariables.IntVariables.AddVariable("WrathProgress", 0);
                        GameVariables.IntVariables.AddVariable("WrathFailure", 0);
                        GameVariables.IntVariables.AddVariable("SlothProgress", 0);
                        GameVariables.IntVariables.AddVariable("SlothFailure", 0);
                        GameVariables.IntVariables.AddVariable("PrideProgress", 0);
                        GameVariables.IntVariables.AddVariable("PrideFailure", 0);
                        PlayerInventory.Items.Clear();
                        MessageSystem.MessageManager.SendImmediate(MessageChannels.Level, new LevelMessage(LevelManager.Instance.CurrentLevelObject.ID, LevelManager.Instance.CurrentLevelObject.Name, LevelState.Unloading, Vector3.zero));
                        MessageSystem.MessageManager.SendImmediate(MessageChannels.GameFlow, new GameStateMessage(GameManager.Instance.GetStateByType<MainMenuState>()));
                    }));
                return;
            }
        
            if (PrideProgressComplete)
            {
                if(PlayerInventory != null)
                {
                    var prideKey = PlayerInventory.ContainsItem(PrideKey);
                    if (!prideKey)
                    {
                        PlayerInventory.AddItem(PrideKey, 1);
                    }
                }
            }
        
            if (WrathProgressComplete && SlothProgressComplete && PrideProgressComplete)
            {
                if(PlayerInventory != null)
                {
                    var trueKey = PlayerInventory.ContainsItem(TrueKey);
                    if (!trueKey)
                    {
                        PlayerInventory.RemoveItem(WrathKey, 1);
                        PlayerInventory.RemoveItem(SlothKey, 1);
                        PlayerInventory.RemoveItem(PrideKey, 1);
                        PlayerInventory.AddItem(TrueKey, 1);
                    }
                }
            }
        }


    
        private void OnApplicationQuit()
        {
            PlayerInventory.Items.Clear();
        }
    }
}
