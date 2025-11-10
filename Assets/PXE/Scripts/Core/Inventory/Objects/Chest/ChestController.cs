using PXE.Core.Enums;
using PXE.Core.Messaging;
using PXE.Core.Objects;
using PXE.Core.UI.Messaging.Messages;
using UnityEngine;

namespace PXE.Core.Inventory.Objects.Chest
{
    /// <summary>
    /// Represents the ChestController.
    /// The ChestController class provides functionality related to chestcontroller management.
    /// This class contains methods and properties that assist in managing and processing chestcontroller related tasks.
    /// </summary>
    [RequireComponent(typeof(Animator), typeof(BoxCollider2D))]
    public class ChestController : ObjectController
    {
        [field: Tooltip("The message to display when the chest is opened.")]
        [field: SerializeField] public virtual string OpenMessage { get; set; }
        
        [field: Tooltip("The message to display when the chest is closed.")]
        [field: SerializeField] public virtual string CloseMessage { get; set; }
        
        [field: Tooltip("The amount of time to display the message.")]
        [field: SerializeField] public virtual float MessageDisplayTime { get; set; }
        
        [field: Tooltip("The location to display the message.")]
        [field: SerializeField] public virtual PopupPosition MessageDisplayLocation { get; set; }
        
        [field: Tooltip("Is the chest open?")]
        [field: SerializeField] public virtual bool IsOpen { get; set; }

        protected Animator anim;

        public override void Awake()
        {
            base.Awake();
            anim = GetComponent<Animator>();
        }

        /// <summary>
        /// Executes the Open method.
        /// Handles the Open functionality.
        /// </summary>
        public virtual bool Open()
        {
            IsOpen = true;
            anim.SetBool("IsOpen", IsOpen);
            if (!string.IsNullOrWhiteSpace(OpenMessage))
            {
                MessageSystem.MessageManager.SendImmediate(MessageChannels.UI, new PopupMessage(OpenMessage, PopupType.Notification, MessageDisplayLocation,MessageDisplayTime));
            }
            return true;
        }

        /// <summary>
        /// Executes the Close method.
        /// Handles the Close functionality.
        /// </summary>
        public virtual bool Close()
        {
            IsOpen = false;
            anim.SetBool("IsOpen", IsOpen);
            if (!string.IsNullOrWhiteSpace(CloseMessage))
            {
                MessageSystem.MessageManager.SendImmediate(MessageChannels.UI, new PopupMessage(CloseMessage, PopupType.Notification, MessageDisplayLocation,MessageDisplayTime));
            }

            return true;
        }

        public virtual void OnTriggerEnter2D(Collider2D col)
        {
            if (col.CompareTag("Player"))
            {
                Open();
            }
        }

        public virtual void OnTriggerExit2D(Collider2D other)
        {
            if (other.CompareTag("Player"))
            {
                Close();
            }
        }
    }
}
