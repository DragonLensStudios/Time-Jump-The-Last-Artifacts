using System;
using PXE.Core.Dialogue;
using PXE.Core.Dialogue.Interaction;
using PXE.Core.Dialogue.Interfaces;
using PXE.Core.Dialogue.Messaging.Messages;
using PXE.Core.Enums;
using PXE.Core.Extensions.GameObjectExtensions;
using PXE.Core.Interfaces;
using PXE.Core.Inventory.Items;
using PXE.Core.Messaging;
using PXE.Core.Messaging.Messages;
using PXE.Core.Objects;
using PXE.Core.Projectiles;
using PXE.Core.SerializableTypes;
using UnityEngine;
//TODO: Try to make these dependencies optional so that the code can be used in other projects.

namespace PXE.Core.Actor
{
    /// <summary>
    /// Abstract base class for controlling an actor in the game.
    /// </summary>
    //TODO: Try to fix it so that IDialogue is only used when the dialogue module is included.
    public class ActorController : PatrolObjectController, IDialogueActor, IHitable
    {
        [field: SerializeField] public virtual ObjectController AfterDeathPrefab { get; set; }
        
        [Tooltip("Current Health of the actor.")]
        [SerializeField] protected int _currentHealth;

        [Tooltip("Max Health of the actor.")]
        [SerializeField] protected int _maxHealth;
        
        
        public virtual int CurrentHealth 
        {   
            get => _currentHealth;
            set => _currentHealth = Mathf.Clamp(value, 0, MaxHealth);
        }

        public virtual int MaxHealth
        {
            get => _maxHealth;
            set
            {
                _maxHealth = value;
                CurrentHealth = Mathf.Clamp(CurrentHealth, 0, MaxHealth);
            }
        }
        
        [field: Tooltip("Hits passthrough actor when false.")]
        [field: SerializeField] public virtual bool IsHitable { get; set; } = true;
       
        [field: Tooltip("Normal hits passthrough actor when true.")]
        [field: SerializeField] public virtual bool IsDodgeing { get; set; } = false;

        [field: Tooltip("Actor cannot be damaged when true.")]
        [field: SerializeField] public virtual bool IsInvincible { get; set; } = false;

        [field: Tooltip("Actor cannot be killed by falling when true.")]
        [field: SerializeField] public virtual bool IsFlying { get; set; } = false;

        [field: Tooltip("default duration actor is immune after taking damage.")]
        [field: SerializeField] public virtual float DodgeDuration { get; set; } = 0.2f;

        [field: Tooltip("team used to prevent friendly fire.")]
        [field: SerializeField] public virtual TeamType Team { get; set; }

        [field: Tooltip("The Reference State for the actor.")]
        [field: SerializeField] public virtual string ReferenceState { get; set; }
        
        [field: Tooltip("The current DialogueGraph for the actor.")]
        [field: SerializeField] public virtual DialogueGraph CurrentDialogueGraph { get; set; }
       
        [field: Tooltip("The current DialogueInteraction for the actor.")]
        [field: SerializeField] public virtual DialogueInteraction CurrentDialogueInteraction { get; set; }
        
        [field: Tooltip("Indicates whether the actor is currently interacting with a target.")]
        [field: SerializeField] public virtual bool IsInteracting { get; set; }
        
        [field: Tooltip("The Inventory for the actor.")]
        [field: SerializeField] public virtual InventoryObject Inventory { get; set; }
        
        [field: Tooltip("The current Portrait for the actor.")]
        [field: SerializeField] public virtual Sprite CurrentPortrait { get; set; }
        
        [field: Tooltip("The ID of the GameObject that the actor is currently targeting for interaction.")]
        [field: SerializeField] public virtual SerializableGuid TargetID { get; set; }
        
        [field: Tooltip("The GameObject that the actor is currently targeting for interaction.")]
        [field: SerializeField] public virtual GameObject TargetGameObject { get; set; }

        // [field: Tooltip("Graphical Rotation Type")]
        // [field: SerializeField] public virtual RotateType RotateType { get; set; }

        [field: Tooltip("The last movement direction.")]
        [field: SerializeField] public virtual Vector3 MovementDirection { get; set; } = Vector3.down;
        
        public virtual Vector3 LastPosition { get; set; }
        public virtual Vector3 MoveVelocity { get; set; } = Vector3.zero;
        protected virtual float DodgeCountdown { get; set; } = 0f;
        

        /// <summary>
        ///  Registers for the Dialogue channel and handles DialogueMessages.
        /// </summary>
        public override void OnActive()
        {
            base.OnActive();
            MessageSystem.MessageManager.RegisterForChannel<DialogueMessage>(MessageChannels.Dialogue, DialogueMessageHandler);
        }

        /// <summary>
        /// Unregisters for the Dialogue channel and stops handling DialogueMessages.
        /// </summary>
        public override void OnInactive()
        {
            base.OnInactive();
            MessageSystem.MessageManager.UnregisterForChannel<DialogueMessage>(MessageChannels.Dialogue, DialogueMessageHandler);
        }

        /// <summary>
        /// Interacts with the current target GameObject.
        /// </summary>
        public virtual void Interact()
        {
            if (IsInteracting) return;
            if (TargetGameObject == null) return;
            MessageSystem.MessageManager.SendImmediate(MessageChannels.Dialogue, new DialogueMessage(DialogueState.Start, ReferenceState, CurrentDialogueGraph, CurrentDialogueInteraction, ID, TargetID));
        }

        /// <summary>
        ///  Handles the OnTriggerEnter2D functionality and sends a GameObjectInteractionMessage.
        /// </summary>
        /// <param name="col"><see cref="Collider2D"/></param>
        public virtual void OnTriggerEnter2D(Collider2D col)
        {
            TargetGameObject = col.gameObject;
            MessageSystem.MessageManager.SendImmediate(MessageChannels.Gameplay, new GameObjectInteractionMessage(gameObject, TargetGameObject));
            TargetID = TargetGameObject.GetObjectID();
        }

        /// <summary>
        ///  Handles the OnTriggerExit2D functionality and sets the TargetGameObject to null.
        /// </summary>
        /// <param name="col"><see cref="Collider2D"/></param>
        public virtual void OnTriggerExit2D(Collider2D col)
        {
            TargetGameObject = null;
            TargetID = new SerializableGuid(Guid.Empty);
        }

        /// <summary>
        ///  Handles the DialogueMessage functionality and sets the ReferenceState and IsInteracting properties based on the DialogueMessage state.
        /// </summary>
        /// <param name="message">The Dialogue Message</param>
        public virtual void DialogueMessageHandler(MessageSystem.IMessageEnvelope message)
        {
            if (!message.Message<DialogueMessage>().HasValue) return;
            var data = message.Message<DialogueMessage>().GetValueOrDefault();
            if (!ID.Equals(data.SourceID) && !ID.Equals(data.TargetID)) return;
            switch (data.State)
            {
                case DialogueState.SetReferenceState:
                    ReferenceState = data.ReferenceState;
                    break;
                case DialogueState.Start:
                    ReferenceState = data.ReferenceState;
                    IsInteracting = true;
                    IsDisabled = true;
                    break;
                case DialogueState.End:
                    IsInteracting = false;
                    IsDisabled = false;
                    break;
            }

        }
        
        public virtual void TakeDamage(int damage)
        {
            CurrentHealth -= damage;
            if (CurrentHealth <= 0)
            {
                OnDie();
            }
        }
        public virtual bool OnHit(IGameObject source, HitType hitType, int damage = 0)
        {
            if (!IsHitable) return false;

            HitableProjectile sourceProjectile = source.gameObject.GetComponent<HitableProjectile>(); // = source as HitableProjectile; 
            if (sourceProjectile != null)
            {
                if (!hitType.HasFlag(HitType.HitOwner) && ID.Equals(sourceProjectile.Owner.ID)) return false;
            }

            var sourceHitable = source.gameObject.GetComponent<IHitable>();
            if (sourceHitable != null)
            {
                if (!hitType.HasFlag(HitType.HitTeam) && ((Team & sourceHitable.Team) != 0)) return false;
            }
            if (IsDodgeing && !hitType.HasFlag(HitType.HitDodging)) return false;
            if (IsFlying && hitType.HasFlag(HitType.MissFlying)) return false;
            if (IsInvincible) return true;
            //TODO: Status Effects Go Here
            if (MaxHealth <= 0) return true;
            if (damage <= 0) return true;
            if (hitType.HasFlag(HitType.Healing))
            {
                CurrentHealth += damage;
                if (CurrentHealth > MaxHealth)
                {
                    CurrentHealth = MaxHealth;
                }

                return true;
            }
            else
            {
                CurrentHealth -= damage;
                if (CurrentHealth <= 0)
                {
                    OnDie();
                }
                DodgeCountdown = Mathf.Max(DodgeCountdown, DodgeDuration);
                IsDodgeing = true;
                return true;
            }
        }

        public virtual bool OnHit(IGameObject source, int damage = 0)
        {
            return OnHit(source, HitType.None, damage);
        }

        public virtual bool OnHit(HitType hitType, int damage = 0)
        {
            return OnHit(null, hitType, damage);
        }

        public virtual bool OnHit(int damage = 0)
        {
            return OnHit(null, HitType.None, damage);
        }


        public virtual void OnDie()
        {
            if (anim != null)
            {
                anim?.SetTrigger("Death");
            }
            if (AfterDeathPrefab != null)
            {
                Instantiate(AfterDeathPrefab, transform.position, Quaternion.identity);
            }
#if UNITY_EDITOR
            DestroyImmediate(gameObject);
#else
            Destroy(gameObject);
#endif
        }

        public override void Update()
        {
            base.Update();
            if (IsDisabled) return;
            UpdateMovementDirection();
            // this.UpdateRotate(MovementDirection);

            if (!IsInvincible) {
                Color color = sr.color;
                color.a = IsDodgeing ? 0.5f : 1.0f;
                sr.color = color;
            }

            if (CurrentHealth <= 0)
            {
                OnDie();
            }
        }

        public override void LateUpdate() // Cleanup code, Overrides should run base.LateUpdate()
        {
            base.LateUpdate();
            if (IsDisabled) return;
            // base.LateUpdate();
            DodgeCountdown = Mathf.Max(DodgeCountdown - UnityEngine.Time.deltaTime, 0f);
            IsDodgeing = (DodgeCountdown > 0f) ? true : false ;

            LastPosition = transform.position;
        }

        public virtual void UpdateMovementDirection()
        {
            Vector3 difference = (transform.position - LastPosition).normalized;
            if (difference != Vector3.zero) MovementDirection = difference;
            // if (movement != Vector2.zero) MovementDirection = new Vector2(movement.x, movement.y);
            // var rb = source.GetComponent<rb>();
            // if (rb != null && transform.velocity != Vector2.zero)
            // {
                // MovementDirection = Vector3.Normalize(new Vector3(transform.velocity.x, transform.velocity.y));
            // }
            // var sr = source.GetComponent<sr>();
            // if (sr != null)
            // {
            // }
            // MovementDirection.Normalize();
            if (MovementDirection == Vector3.zero) MovementDirection = Vector3.down;
        }

    }
}
