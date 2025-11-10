using PXE.Core.Data_Persistence.Messaging.Messages;
using PXE.Core.Enums;
using PXE.Core.Levels.Messaging.Messages;
using PXE.Core.Messaging;
using PXE.Core.Objects;
using PXE.Core.SerializableTypes;
using PXE.Core.State_System;
using PXE.Core.State_System.Messaging.Messages;
using PXE.Core.Transition;
using PXE.Core.Transition.Messaging.Messages;
using PXE.Core.UI.Messaging.Messages;
using TMPro;
using UnityEngine;

namespace PXE.Core.Data_Persistence.UI
{
    /// <summary>
    /// Represents the SaveSlot.
    /// The SaveSlot class provides functionality related to saveslot management.
    /// This class contains methods and properties that assist in managing and processing saveslot related tasks.
    /// </summary>
    public class SaveSlot : ObjectController
    {
        [field: SerializeField] public SerializableGuid PlayerID { get; set; }
        [field: SerializeField] public string PlayerName { get; set; }
        [field: SerializeField] public Vector3 PlayerPosition { get; set; }
        [field: SerializeField] public SerializableGuid CurrentLevelID { get; set; }
        [field: SerializeField] public string CurrentLevelName { get; set; }
        [field: SerializeField] public TMP_Text PlayerNameText { get; set; }
        [field: SerializeField] public TMP_Text TimestampText { get; set; }
        [field: SerializeField] public GameState State { get; set; }
        [field: SerializeField] public TransitionParameters TransitionParameters { get; set; }

        /// <summary>
        /// Executes the LoadSave method.
        /// Handles the LoadSave functionality.
        /// </summary>
        public void LoadSave()
        {
            MessageSystem.MessageManager.SendImmediate(MessageChannels.Level, new LevelMessage(CurrentLevelID, CurrentLevelName, LevelState.Loading, PlayerPosition));
            MessageSystem.MessageManager.SendImmediate(MessageChannels.GameFlow, new GameStateMessage(State));
            MessageSystem.MessageManager.SendImmediate(MessageChannels.Saves, new SaveLoadMessage(PlayerID, PlayerName, SaveOperation.Load));
        }

        /// <summary>
        /// Executes the DeleteSave method.
        /// Handles the DeleteSave functionality.
        /// </summary>
        public void DeleteSave()
        {
            MessageSystem.MessageManager.SendImmediate(MessageChannels.UI, 
                new PopupMessage("Are you sure you want to delete this save?", PopupType.Confirm, PopupPosition.Middle, 0f, null, () => { }, () =>
                {
                    MessageSystem.MessageManager.SendImmediate(MessageChannels.Saves, new SaveLoadMessage(PlayerID, PlayerName, SaveOperation.Delete));
                }));
        }

        /// <summary>
        /// Executes the Transition method.
        /// Handles the Transition functionality.
        /// </summary>
        public void Transition()
        {
            // Create a new transition message
            var transitionMessage = new TransitionMessage(TransitionParameters.transitionType, TransitionParameters.slideDirection,
                TransitionParameters.animationDurationInSeconds, TransitionParameters.endEvent);

            // Send the message
            MessageSystem.MessageManager.SendImmediate(MessageChannels.UI, transitionMessage);
        }
    }
}
