using PXE.Core.Enums;
using PXE.Core.Messaging;
using PXE.Core.State_System.Messaging.Messages;
using UnityEngine;

namespace PXE.Core.State_System
{
    [CreateAssetMenu(fileName = "InventoryState", menuName = "PXE/GameStates/InventoryState", order = 1)]
    public class InventoryState : GameState
    {
        [Tooltip("The InventoryCanvas prefab")]
        [field: SerializeField] public GameObject InventoryCanvasPrefab { get; set; }
        
        [Tooltip("The InventoryCanvas")]
        [field: SerializeField] public GameObject Inventory { get; set; }
        
        /// <summary>
        ///  Handles the enter event and instantiates the InventoryCanvas.
        /// </summary>
        public override void Enter()
        {
            base.Enter();
            MessageSystem.MessageManager.SendImmediate(MessageChannels.GameFlow, new PauseMessage(this, true));
            if (Inventory == null)
            {
                Inventory = Instantiate(InventoryCanvasPrefab);
            }
        }
        
        /// <summary>
        ///  Handles the exit event and destroys the InventoryCanvas.
        /// </summary>
        public override void Exit()
        {
            base.Exit();
            MessageSystem.MessageManager.SendImmediate(MessageChannels.GameFlow, new PauseMessage(this, false));
            if (Inventory != null)
            {
                Destroy(Inventory);
            }
        }
        
        /// <summary>
        ///  Handles the update event.
        /// </summary>
        public override void Update()
        {
            // Debug.Log("Update inventory state");
        }
    }
}