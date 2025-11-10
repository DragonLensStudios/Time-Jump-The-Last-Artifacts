using System.Collections.Generic;
using System.Linq;
using PXE.Core.Dialogue.Interaction;
using PXE.Core.Enums;
using PXE.Core.Levels.Messaging.Messages;
using PXE.Core.Messaging;
using PXE.Core.ScriptableObjects;
using PXE.Core.UI.Messaging.Messages;
using UnityEngine;

namespace PXE.Core.Dialogue
{
    /// <summary>
    /// Manages the dialogues and their selection in the dialogue system.
    /// </summary>
    [CreateAssetMenu(fileName = "New Dialogue Manager", menuName = "PXE/Dialogue/Dialogue Manager", order = 25)]
    public class DialogueManagerObject : ScriptableObjectController
    {
        [field: Tooltip("The current reference state of the dialogue manager.")]
        [field: SerializeField] public string CurrentReferenceState { get; set; }
        
        [field: Tooltip("The current dialogue graph.")]
        [field: SerializeField] public DialogueGraph CurrentGraph { get; set; }
        
        [field: Tooltip("The current dialogue interaction.")]
        [field: SerializeField] public DialogueInteraction CurrentInteraction { get; set; }
        
        [field: Tooltip("The list of dialogue interactions.")]
        [field: SerializeField] public List<DialogueInteraction> Interactions { get; set; } = new();

        [field: Tooltip("The use random dialogue selection by weight toggle.")]
        [field: SerializeField] public bool UseRandomDialogueSelectionByWeight { get; set; } = false;
        
        /// <summary>
        ///  Executes the DialogueManagerObject method and returns the dialogue interactions.
        /// </summary>
        /// <param name="includeCompleted"></param>
        /// <returns></returns>
        public virtual List<DialogueInteraction> GetDialogueInteractions(bool includeCompleted)
        {
            var matchingInteractions = new List<DialogueInteraction>();
            if (includeCompleted)
            {
                matchingInteractions = Interactions
                    .FindAll(x => x.ReferenceState.Equals(CurrentReferenceState))
                    .OrderByDescending(x => x.InteractionWeight).ToList();

                if (CurrentGraph != null && matchingInteractions.Count <= 0)
                {
                    matchingInteractions = Interactions
                        .FindAll(x=>x.Graph == CurrentGraph)
                        .OrderByDescending(x => x.InteractionWeight).ToList();
                }

                if (CurrentInteraction != null && matchingInteractions.Count <= 0)
                {
                    matchingInteractions = Interactions
                        .FindAll(x=> x == CurrentInteraction)
                        .OrderByDescending(x => x.InteractionWeight).ToList();
                }
            }
            else
            {
                matchingInteractions = Interactions
                    .FindAll(x => x.ReferenceState.Equals(CurrentReferenceState) && !x.DialogueCompleted)
                    .OrderByDescending(x => x.InteractionWeight).ToList();

                if (CurrentGraph != null && matchingInteractions.Count <= 0)
                {
                    matchingInteractions = Interactions
                        .FindAll(x=>x.Graph == CurrentGraph && !x.DialogueCompleted)
                        .OrderByDescending(x => x.InteractionWeight).ToList();
                }

                if (CurrentInteraction != null && matchingInteractions.Count <= 0)
                {
                    matchingInteractions = Interactions
                        .FindAll(x=> x == CurrentInteraction && !x.DialogueCompleted)
                        .OrderByDescending(x => x.InteractionWeight).ToList();
                }
            }


            return matchingInteractions;
        }
        /// <summary>
        /// Sets the current dialogue interaction based on the current reference state and available interactions.
        /// </summary>
        public virtual bool SetCurrentDialogue()
        {
            var matchingInteractions = GetDialogueInteractions(false);

            if (matchingInteractions.Count > 1)
            {
                int totalWeight = 0;

                if (UseRandomDialogueSelectionByWeight)
                {
                    for (int i = 0; i < matchingInteractions.Count; i++)
                    {
                        totalWeight += matchingInteractions[i].InteractionWeight;
                    }

                    int randomValue = Random.Range(0, totalWeight);

                    for (int i = 0; i < matchingInteractions.Count; i++)
                    {
                        if (randomValue < matchingInteractions[i].InteractionWeight)
                        {
                            CurrentInteraction = matchingInteractions[i];
                            CurrentGraph = CurrentInteraction.Graph;
                            return true;
                        }

                        randomValue -= matchingInteractions[i].InteractionWeight;
                    }
                }
                else
                {
                    CurrentInteraction = matchingInteractions.FirstOrDefault();
                    if (CurrentInteraction != null) return true;
                }

                if (CurrentInteraction == null) return false;
            }
            else
            {
                for (var i = 0; i < Interactions.Count; i++)
                {
                    var interaction = Interactions[i];

                    if (interaction.ReferenceState.Equals(CurrentReferenceState) && !interaction.DialogueCompleted)
                    {
                        CurrentInteraction = interaction;
                        CurrentGraph = CurrentInteraction.Graph;
                        return true;
                    }

                    CurrentInteraction = Interactions.FirstOrDefault(x =>
                        x.ReferenceState.Equals(string.Empty) && x.RepeatableDialogue);
                }
        
                if (CurrentInteraction == null) return false;
            }

            return true;
        }


        /// <summary>
        /// Starts the dialogue by setting the current dialogue and marking it as completed.
        /// </summary>
        public virtual bool StartDialogue()
        {
            if (SetCurrentDialogue())
            {
                if (CurrentInteraction == null || CurrentInteraction.DialogueCompleted && !CurrentInteraction.RepeatableDialogue)
                {
                    return false;
                }
            }
            else
            {
                return false;
            }

            return true;
        }

        /// <summary>
        ///  Ends the dialogue by setting the current dialogue to null.
        /// </summary>
        /// <returns></returns>
        public virtual bool EndDialogue()
        {
            MessageSystem.MessageManager.SendImmediate(MessageChannels.UI, new HidePopupMessage(PopupType.Notification));
            if (CurrentInteraction != null)
            {
                CurrentInteraction.DialogueCompleted = true;
            }
            CurrentGraph = null;
            CurrentInteraction = null;
            return true;
        }
        
        /// <summary>
        /// Called when the scriptable object is enabled.
        /// </summary>
        private void OnEnable()
        {
            Application.quitting += ApplicationOnQuitting;
            MessageSystem.MessageManager.RegisterForChannel<LevelResetMessage>(MessageChannels.Level, LevelResetMessageHandler);
        }



        /// <summary>
        /// Called when the application is quitting.
        /// </summary>
        private void ApplicationOnQuitting()
        {
            CurrentGraph = null;
            CurrentInteraction = null;
            CurrentReferenceState = string.Empty;
        }

        /// <summary>
        /// Called when the scriptable object is disabled.
        /// </summary>
        private void OnDisable()
        {
            Application.quitting -= ApplicationOnQuitting;
            MessageSystem.MessageManager.UnregisterForChannel<LevelResetMessage>(MessageChannels.Level, LevelResetMessageHandler);

        }
        
        /// <summary>
        ///  Handles the level reset message.
        /// </summary>
        /// <param name="message"></param>
        private void LevelResetMessageHandler(MessageSystem.IMessageEnvelope message)
        {
            if(!message.Message<LevelResetMessage>().HasValue) return;
            ApplicationOnQuitting();
        }
    }
}