using PXE.Core.Enums;
using PXE.Core.Messaging;
using PXE.Core.Messaging.Messages;
using PXE.Core.Objects;
using PXE.Core.Player.Managers;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace PXE.DEBUG.UI
{
    public class PlayerPropertiesUiController : ObjectController
    {
        [field: Tooltip("The player position X input field." )]
        [field: SerializeField] public TMP_InputField PlayerPositionXInput { get; set; }
        
        [field: Tooltip("The player position Y input field." )]
        [field: SerializeField] public TMP_InputField PlayerPositionYInput { get; set; }
        
        [field: Tooltip("The player position Z input field." )]
        [field: SerializeField] public TMP_InputField PlayerPositionZInput { get; set; }
        
        [field: Tooltip("sets god mode on or off." )]
        [field: SerializeField] public Toggle GodModeToggle { get; set; }
        
        [field: Tooltip("The god mode toggle checkbox." )]
        [field: SerializeField] public ObjectController GodModeToggleCheckbox { get; set; }

        
        /// <summary>
        ///  Disable the god mode toggle checkbox.
        /// </summary>
        public override void OnInactive()
        {
            base.OnInactive();
            if (GodModeToggleCheckbox != null)
            {
                GodModeToggleCheckbox.SetObjectActive(false);
            }
        }

        /// <summary>
        ///  Enable the god mode toggle checkbox.
        /// </summary>
        public override void OnActive()
        {
            base.OnActive();
            if (GodModeToggleCheckbox == null) return;
            if (GodModeToggle != null)
            {
                GodModeToggleCheckbox.SetObjectActive(GodModeToggle.isOn);
            }
        }

        /// <summary>
        ///  Executes the PlayerPropertiesUiController method and sets the player position and calls the player position message.
        /// </summary>
        public virtual void SetPlayerPosition()
        {
            var player = PlayerManager.Instance.Player;
            float posX, posY, posZ;
            float.TryParse(PlayerPositionXInput.text, out posX);
            float.TryParse(PlayerPositionYInput.text, out posY);
            float.TryParse(PlayerPositionZInput.text, out posZ);
            Vector3 position = new Vector3(posX, posY, posZ);
            MessageSystem.MessageManager.SendImmediate(MessageChannels.Object, new TransformPositionMessage(player.ID, position, Quaternion.identity));
        }

        /// <summary>
        ///  Sets god mode on or off.
        /// </summary>
        /// <param name="godMode"></param>
        public virtual void SetGodMode(bool godMode)
        {
            MessageSystem.MessageManager.SendImmediate(MessageChannels.Player, new GodModeMessage(godMode));
            if (GodModeToggleCheckbox != null)
            {
                GodModeToggleCheckbox.SetObjectActive(godMode);
            }
        }
    }
}
