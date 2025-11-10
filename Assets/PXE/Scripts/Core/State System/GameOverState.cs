using UnityEngine;

namespace PXE.Core.State_System
{
    /// <summary>
    ///  Represents the GameOverState.
    /// </summary>
    [CreateAssetMenu(fileName = "GameOverState", menuName = "PXE/GameStates/GameOverState", order = 3)]
    public class GameOverState : GameState
    {
        [field: Tooltip("The GameOverCanvas prefab")]
        [field: SerializeField] public GameObject GameOverCanvasPrefab { get; set; }
        
        [field: Tooltip("The GameOverCanvas")]
        [field: SerializeField] public GameObject GameOverCanvas { get; set; }

        /// <summary>
        ///  Handles the enter event and instantiates the GameOverCanvas.
        /// </summary>
        public override void Enter()
        {
            base.Enter();
            if (GameOverCanvas == null)
            {
                GameOverCanvas = Instantiate(GameOverCanvasPrefab);
            }
            // Debug.Log("Entered game over state");
        }

        /// <summary>
        ///  Handles the exit event and destroys the GameOverCanvas.
        /// </summary>
        public override void Exit()
        {
            base.Exit();
            if (GameOverCanvas != null)
            {
                Destroy(GameOverCanvas);
            }
            // Debug.Log("Exited game over state");
        }

        /// <summary>
        ///  Handles the update event.
        /// </summary>
        public override void Update()
        {
            // Debug.Log("Update game over state");
        }
    }
}