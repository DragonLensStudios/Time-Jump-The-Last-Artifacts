using PXE.Core.Audio;
using PXE.Core.Audio.Messaging.Messages;
using PXE.Core.Enums;
using PXE.Core.Messaging;
using PXE.Core.Objects;
using PXE.Core.State_System.Messaging.Messages;
using PXE.Example_Games.Oceans_Call.Messages;
using UnityEngine;

namespace PXE.Example_Games.Oceans_Call.Objects
{
    /// <summary>
    ///  Represents the MessageInABottleController.
    /// </summary>
    public class MessageInABottleController : ObjectController
    {
        [field: Tooltip("The message to display when the player interacts with the bottle.")]
        [field: SerializeField,TextArea(1, 3), Header("Max characters to fit the letter is 286")] public string Message { get; set; }
        
        [field: Tooltip("The time in seconds to display the message.")]
        [field: SerializeField] public float TimeToDisplay { get; set; } = 10f;
        
        [field: Tooltip("The speed at which the bottle moves.")]
        [field: SerializeField] public float MoveSpeed { get; set; } = 1.5f;
        
        [field: Tooltip("The direction that the bottle moves.")]
        [field: SerializeField] public Vector2 MoveDirection { get; set; } = Vector2.up;
        
        [field: Tooltip("Indicates whether the bottle's movement is disabled.")]
        [field: SerializeField] public bool IsMovementDisabled { get; set; }
        
        [field: Tooltip("The bounds for the bottle.")]
        [field: SerializeField] public Vector2 Bounds { get; set; }
        
        [field: Tooltip("The AudioObject to play when the bottle is opened.")]
        [field: SerializeField] public AudioObject OpenSfx { get; set; }
        [field: SerializeField] public bool IsCollectable { get; set; } = true;
        

        protected Rigidbody2D rb;
        protected float originalGravityScale;

        /// <summary>
        ///  sets the Rigidbody2D component.
        /// </summary>
        public override void Awake()
        {
            base.Awake();
            rb = GetComponent<Rigidbody2D>();
        }

        /// <summary>
        ///  sets the originalGravityScale.
        /// </summary>
        public override void Start()
        {
            base.Start();
            originalGravityScale = rb.gravityScale;
        }

        /// <summary>
        ///  Registers for the Pause channel and handles PauseMessages.
        /// </summary>
        public override void OnActive()
        {
            base.OnActive();
            if (rb == null)
            {
                rb = GetComponent<Rigidbody2D>();
            }
            MessageSystem.MessageManager.RegisterForChannel<PauseMessage>(MessageChannels.GameFlow, PauseMessageHandler);
        }


        /// <summary>
        ///  Unregisters for the Pause channel and stops handling PauseMessages.
        /// </summary>
        public override void OnInactive()
        {
            base.OnInactive();
            if (rb == null)
            {
                rb = GetComponent<Rigidbody2D>();
            }
            MessageSystem.MessageManager.UnregisterForChannel<PauseMessage>(MessageChannels.GameFlow, PauseMessageHandler);
        }
        
        /// <summary>
        ///  Handles the OnTriggerEnter2D functionality and sends a LetterMessage.
        /// </summary>
        /// <param name="other"></param>
        public virtual void OnTriggerEnter2D(Collider2D other)
        {
            if (!other.gameObject.CompareTag("Player")) return;
            if(OpenSfx != null)
            {
                MessageSystem.MessageManager.SendImmediate(MessageChannels.Audio, new AudioMessage(OpenSfx, AudioOperation.Play, AudioChannel.SoundEffects));
            }
            MessageSystem.MessageManager.SendImmediate(MessageChannels.UI, new LetterMessage(Message, TimeToDisplay));
            SetObjectActive(false);
            if (IsCollectable)
            {
                MessageSystem.MessageManager.SendImmediate(MessageChannels.Player, new CollectibleMessage(ID, Name));
                Destroy(gameObject);
            }
        }

        /// <summary>
        ///  Handles the FixedUpdate functionality and applies a downward force if the bottle is above the bounds.
        /// </summary>
        public override void FixedUpdate()
        {
            base.FixedUpdate();
            if (IsMovementDisabled) return;
            
            if(rb.position.y > Bounds.y)
            {
                rb.AddForce(new Vector2(0, -MoveSpeed)); // Apply a downward force
            }
            else
            {
                rb.AddForce(MoveDirection * MoveSpeed);
            }
        }
        
        /// <summary>
        ///  Handles the PauseMessage functionality and sets the IsMovementDisabled property based on the PauseMessage state.
        /// </summary>
        /// <param name="message"></param>
        public virtual void PauseMessageHandler(MessageSystem.IMessageEnvelope message)
        {
            if(!message.Message<PauseMessage>().HasValue) return;
            var data = message.Message<PauseMessage>().GetValueOrDefault();
            if(rb == null) return;
            if (data.IsPaused)
            {
                IsMovementDisabled = true;
                rb.gravityScale = 0;
            }
            else
            {
                IsMovementDisabled = false;
                rb.gravityScale = originalGravityScale;
            }
        }
    }
}
