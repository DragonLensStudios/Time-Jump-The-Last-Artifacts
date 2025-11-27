using PXE.Core.Dialogue;
using PXE.Core.Dialogue.Messaging.Messages;
using PXE.Core.Enums;
using PXE.Core.Messaging;
using PXE.Core.ScriptableObjects;
using PXE.Core.Game.Managers;
using PXE.Core.State_System;
using PXE.Core.State_System.Messaging.Messages;
using UnityEngine;


namespace PXE.Example_Games.Oceans_Call.Scripts.End_Game
{
    [CreateAssetMenu(fileName = "EndOfGame", menuName = "PXE/Oceans Call/End Of Game")]
    public class EndOfGame : ScriptableObjectController
    {
        [field: SerializeField] public string EndOfGameDialogueId { get; set; }
        [field: SerializeField] public DialogueGraph EndOfGameDialogueGraph { get; set; }

        public void TriggerEndOfGame()
        {
            MessageSystem.MessageManager.SendImmediate(MessageChannels.Dialogue, new StartDialogueMessage(EndOfGameDialogueId, EndOfGameDialogueGraph));
            if (EndOfGameDialogueGraph != null)
            {
                MessageSystem.MessageManager.SendImmediate(MessageChannels.Level, new EndLevelMessage(EndOfGameDialogueGraph));
                // After signalling the level end, transition to the GameOver state via the GameFlow channel.
                MessageSystem.MessageManager.SendImmediate(MessageChannels.GameFlow, new PauseMessage(this, true));
                MessageSystem.MessageManager.SendImmediate(MessageChannels.GameFlow, new GameStateMessage(GameManager.Instance.GetStateByType<GameOverState>()));
            }
        }
    }
}
