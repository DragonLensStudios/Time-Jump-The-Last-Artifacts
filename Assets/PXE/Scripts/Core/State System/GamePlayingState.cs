using PXE.Core.Enums;
using PXE.Core.Messaging;
using PXE.Core.Messaging.Messages;
using UnityEngine;

namespace PXE.Core.State_System
{
    /// <summary>
    ///  Represents the GamePlayingState.
    /// </summary>
    [CreateAssetMenu(fileName = "GamePlayingState", menuName = "PXE/GameStates/GamePlayingState", order = 1)]
    public class GamePlayingState : GameState
    {
        [field: Tooltip("The InGameUICanvas prefab")]
        [field: SerializeField] public GameObject InGameUICanvasPrefab { get; set; }
        
        [field: Tooltip("The InGameUICanvas")]
        [field: SerializeField] public GameObject InGameUICanvas { get; set; }

        /// <summary>
        ///  Instantiates the InGameUICanvas.
        /// </summary>
        public override void Enter()
        {
            base.Enter();
            MessageSystem.MessageManager.SendImmediate(MessageChannels.UI, new TouchControlMessage(true));
            if (InGameUICanvas == null)
            {
                InGameUICanvas = Instantiate(InGameUICanvasPrefab);
            }
            

            // Debug.Log("Entered game playing state");
        }
        
        /// <summary>
        ///  Destroys the InGameUICanvas.
        ///  Destroys the TouchControlManager if the game is using touch controls.
        /// </summary>
        public override void Exit()
        {
            base.Exit();
            MessageSystem.MessageManager.SendImmediate(MessageChannels.UI, new TouchControlMessage(false));
            if (InGameUICanvas != null)
            {
                Destroy(InGameUICanvas);
            }

            // Debug.Log("Exited game playing state");
        }
        
        /// <summary>
        ///  Handles the update event.
        /// </summary>
        public override void Update()
        {
            // Debug.Log("Update game playing state");
        }
    }
}