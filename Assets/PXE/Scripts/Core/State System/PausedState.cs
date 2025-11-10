using UnityEngine;

namespace PXE.Core.State_System
{
    /// <summary>
    ///  Represents the PausedState.
    /// </summary>
    [CreateAssetMenu(fileName = "PausedState", menuName = "PXE/GameStates/Paused", order = 2)]
    public class PausedState : GameState
    {
        [field: Tooltip("The PauseMenuCanvas prefab")]
        [field: SerializeField] public GameObject PauseMenuCanvasPrefab { get; set; }
        
        [field: Tooltip("The PauseMenuCanvas")]
        [field: SerializeField] public GameObject PauseMenu { get; set; }
        
        /// <summary>
        ///  Handles the enter event and instantiates the PauseMenuCanvas.
        /// </summary>
        public override void Enter()
        {
            base.Enter();
            if (PauseMenu == null)
            {
                PauseMenu = Instantiate(PauseMenuCanvasPrefab);
            }
            
            // Debug.Log("Entered paused state");
        }
        
        /// <summary>
        ///  Handles the exit event and destroys the PauseMenuCanvas.
        /// </summary>
        public override void Exit()
        {
            base.Exit();
            if (PauseMenu != null)
            {
                Destroy(PauseMenu);
            }
            // Debug.Log("Exited paused state");
        }
        
        /// <summary>
        ///  Handles the update event.
        /// </summary>
        public override void Update()
        {
            // Debug.Log("Update paused state");
        }
    }
}