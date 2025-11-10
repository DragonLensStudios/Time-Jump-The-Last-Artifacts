using System;
using PXE.Core.Data_Persistence.Data;
using PXE.Core.Data_Persistence.Interfaces;
using PXE.Core.Dialogue.Messaging.Messages;
using PXE.Core.Enums;
using PXE.Core.Messaging;
using PXE.Core.Objects;
using PXE.Core.State_System.Messaging.Messages;
using PXE.Core.UI.Messaging.Messages;
using UnityEngine;

namespace PXE.Core.Dialogue.Managers
{
    public class DialogueManager : ObjectController, IDataPersistable
    {
        public static DialogueManager Instance { get; private set; }
        
        [field: Tooltip("The current dialogue manager object.")]
        [field: SerializeField] public virtual DialogueManagerObject CurrentDialogueManagerObject { get; set; }
        
        [field: Tooltip("Pause the game when dialogue is active.")]
        [field: SerializeField] public virtual bool PauseGameOnDialogue { get; set; } = true;
        
        /// <summary>
        ///  Singleton pattern for the dialogue manager.
        /// </summary>
        public override void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
            base.Awake();
        }
        
        /// <summary>
        ///  When the dialogue manager is enabled it registers for the dialogue channel.
        /// </summary>
        public override void OnActive()
        {
            base.OnActive();
            MessageSystem.MessageManager.RegisterForChannel<DialogueMessage>(MessageChannels.Dialogue, DialogueInteractMessageHandler);
        }

        /// <summary>
        ///  When the dialogue manager is disabled it unregisters for the dialogue channel.
        /// </summary>
        public override void OnInactive()
        {
            base.OnInactive();
            MessageSystem.MessageManager.UnregisterForChannel<DialogueMessage>(MessageChannels.Dialogue, DialogueInteractMessageHandler);
        }
        

        /// <summary>
        /// Handles the dialogue interact message and sets the current reference state, graph, and interaction and starts or ends the dialogue.
        /// </summary>
        /// <param name="message"></param>
        public virtual void DialogueInteractMessageHandler(MessageSystem.IMessageEnvelope message)
        {
            if(!message.Message<DialogueMessage>().HasValue) return;
            var data = message.Message<DialogueMessage>().GetValueOrDefault();
            MessageSystem.MessageManager.SendImmediate(MessageChannels.UI, new HidePopupMessage(PopupType.Notification));
            switch (data.State)
            {
                case DialogueState.SetReferenceState:
                    CurrentDialogueManagerObject.CurrentReferenceState = data.ReferenceState;
                    break;
                case DialogueState.SetGraph:
                    CurrentDialogueManagerObject.CurrentGraph = data.Graph;
                    break;
                case DialogueState.SetInteraction:
                    CurrentDialogueManagerObject.CurrentInteraction = data.Interaction;
                    break;
                case DialogueState.Start:
                    CurrentDialogueManagerObject.StartDialogue();
                    if (PauseGameOnDialogue)
                    {
                        MessageSystem.MessageManager.SendImmediate(MessageChannels.GameFlow, new PauseMessage(this, true, false));
                    }
                    break;
                case DialogueState.End:
                    CurrentDialogueManagerObject.EndDialogue();
                    if (PauseGameOnDialogue)
                    {
                        MessageSystem.MessageManager.SendImmediate(MessageChannels.GameFlow, new PauseMessage(this, false, false));
                    }
                    break;
            }
            
        }
        /// <summary>
        ///  When the application quits it sets the current reference state, interaction, and graph to null.
        /// </summary>
        public virtual void OnApplicationQuit()
        {
            if (CurrentDialogueManagerObject == null) return;
            CurrentDialogueManagerObject.CurrentReferenceState = string.Empty;
            CurrentDialogueManagerObject.CurrentInteraction = null;
            CurrentDialogueManagerObject.CurrentGraph = null;
        }
        
        public void LoadData<T>(T loadedGameData) where T : class, IGameDataContent, new()
        {
            if (loadedGameData is BaseGameData gameData)
            {
                if(!gameData.ID.Equals(ID)) return;
                CurrentDialogueManagerObject.CurrentReferenceState = gameData.ReferenceState;
            }
        }

        public void SaveData<T>(T savedGameData) where T : class, IGameDataContent, new()
        {
            if (savedGameData is BaseGameData gameData)
            {
                savedGameData.ID = ID;
                savedGameData.Name = Name;
                savedGameData.ReferenceState = CurrentDialogueManagerObject.CurrentReferenceState;
                savedGameData.LastUpdated = DateTime.Now;
            }
        }
    }
}