using PXE.Core.Enums;
using PXE.Core.Messaging;
using PXE.Core.State_System.Messaging.Messages;
using UnityEngine;

namespace PXE.Core.State_System
{
    /// <summary>
    ///  Represents the MainMenuState.
    /// </summary>
    [CreateAssetMenu(fileName = "MainMenuState", menuName = "PXE/GameStates/MainMenuState", order = 0)]
    public class MainMenuState : GameState
    {
        [field: Tooltip("The MainMenuCanvas prefab")]
        [field: SerializeField] public GameObject MainMenuCanvasPrefab { get; set; }
        
        [field: Tooltip("The MainMenuCanvas")]
        [field: SerializeField] public GameObject MainMenu { get; set; }
        
        /// <summary>
        ///  Handles the enter event and instantiates the MainMenuCanvas.
        /// </summary>
        public override void Enter()
        {
            base.Enter();
            MessageSystem.MessageManager.SendImmediate(MessageChannels.GameFlow, new PauseMessage(this, true));
            if (MainMenu == null)
            {
                MainMenu = Instantiate(MainMenuCanvasPrefab);
            }
        }
        
        /// <summary>
        ///  Handles the exit event and destroys the MainMenuCanvas.
        /// </summary>
        public override void Exit()
        {
            base.Exit();
            MessageSystem.MessageManager.SendImmediate(MessageChannels.GameFlow, new PauseMessage(this, false));
            if (MainMenu != null)
            {
                Destroy(MainMenu);
            }

        }
        
        /// <summary>
        ///  Handles the update event.
        /// </summary>
        public override void Update()
        {
            
        }
    }
}