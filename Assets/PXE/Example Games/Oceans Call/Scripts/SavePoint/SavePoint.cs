using PXE.Core.Audio;
using PXE.Core.Audio.Messaging.Messages;
using PXE.Core.Data_Persistence.Messaging.Messages;
using PXE.Core.Enums;
using PXE.Core.Messaging;
using PXE.Core.Objects;
using PXE.Core.UI.Messaging.Messages;
using PXE.Core.Utilities.Input;
using PXE.Example_Games.Oceans_Call.Player;
using UnityEngine;
using UnityEngine.InputSystem;

namespace PXE.Example_Games.Oceans_Call.SavePoint
{
    /// <summary>
    /// Represents the SavePoint.
    /// The SavePoint class provides functionality related to savepoint management.
    /// This class contains methods and properties that assist in managing and processing savepoint related tasks.
    /// </summary>
    [RequireComponent(typeof(BoxCollider2D))]
    public class SavePoint : ObjectController
    {
        [SerializeField] private bool saveOnTrigger = true;
        [SerializeField] private AudioObject sfx;
        [SerializeField] private InputActionReference actionReference;
        private PlayerInputActions playerInput;
        private OceansCallPlayerController player;
        private Animator anim;
        private bool isSaved = false;

        public override void Awake()
        {
            base.Awake();
            playerInput = new PlayerInputActions();
            anim = GetComponent<Animator>();
        }

        public override void OnActive()
        {
            base.OnActive();
            playerInput ??= new PlayerInputActions();
            playerInput.Enable();
            playerInput.Player.Interact.performed += InteractOnperformed;
        }

        public override void OnInactive()
        {
            base.OnInactive();
            playerInput ??= new PlayerInputActions();
            playerInput.Disable();
            playerInput.Player.Interact.performed -= InteractOnperformed;
        }
        
        private void OnTriggerEnter2D(Collider2D col)
        {
            if (!col.CompareTag("Player")) return;
            player = col.GetComponent<OceansCallPlayerController>();
            
            if (actionReference == null)
            {
                Debug.LogError("Action reference or button text is not assigned.");
                return;
            }
            string buttonName = InputHelper.GetButtonNameForAction(actionReference);
            if (saveOnTrigger)
            {
                if(isSaved) return;
                if (sfx != null)
                {
                    MessageSystem.MessageManager.SendImmediate(MessageChannels.Audio, new AudioMessage(sfx, AudioOperation.Play, AudioChannel.SoundEffects));
                }

                player.LastCheckpointPosition = transform.position;
                MessageSystem.MessageManager.SendImmediate(MessageChannels.Saves,  new SaveLoadMessage(player.ID, player.Name, SaveOperation.Save));
                MessageSystem.MessageManager.SendImmediate(MessageChannels.UI, new HidePopupMessage(PopupType.Notification));
                MessageSystem.MessageManager.SendImmediate(MessageChannels.UI, new PopupMessage("Checkpoint!", PopupType.Notification, PopupPosition.Bottom, 2));
                isSaved = true;
                if (anim != null)
                {
                    anim.SetBool("isOpen", isSaved);
                }
            }
            else
            {
                MessageSystem.MessageManager.SendImmediate(MessageChannels.UI, new PopupMessage($"Press {buttonName} To Save", PopupType.Notification));
            }
        }

        private void OnTriggerExit2D(Collider2D col)
        {
            if (!col.CompareTag("Player")) return;
            player = null;
            MessageSystem.MessageManager.SendImmediate(MessageChannels.UI, new HidePopupMessage(PopupType.Notification));
        }

        private void InteractOnperformed(InputAction.CallbackContext input)
        {
            if (player == null) return;
            if(saveOnTrigger) return;
            MessageSystem.MessageManager.SendImmediate(MessageChannels.UI, new HidePopupMessage(PopupType.Notification));
            MessageSystem.MessageManager.SendImmediate(MessageChannels.UI, new PopupMessage("Would you like to save?", PopupType.Confirm, PopupPosition.Middle, 0, null, () => {},
                () =>
                {
                    if (sfx != null)
                    {
                        MessageSystem.MessageManager.SendImmediate(MessageChannels.Audio, new AudioMessage(sfx, AudioOperation.Play, AudioChannel.SoundEffects));
                    }
                    player.LastCheckpointPosition = transform.position;
                    MessageSystem.MessageManager.SendImmediate(MessageChannels.Saves,  new SaveLoadMessage(player.ID, player.Name, SaveOperation.Save));
                    
                    isSaved = true;
                    if (anim != null)
                    {
                        anim.SetBool("isOpen", isSaved);
                    }

                }));
        }
    }
}
