using PXE.Core.Data_Persistence.Messaging.Messages;
using PXE.Core.Enums;
using PXE.Core.Levels.Messaging.Messages;
using PXE.Core.Messaging;
using PXE.Core.Objects;
using PXE.Core.Player.Managers;
using PXE.Core.Transition;
using PXE.Core.Transition.Messaging.Messages;
using UnityEngine;

namespace PXE.Core.Levels
{
/// <summary>
/// Represents the LevelTransition.
/// The LevelTransition class provides functionality related to leveltransition management.
/// This class contains methods and properties that assist in managing and processing leveltransition related tasks.
/// </summary>
    public class LevelTransition : ObjectController
    { 
        [field: Tooltip("The level to transition to.")]
        [field: SerializeField] public virtual LevelObject LevelObject { get; set; }
        
        [field: Tooltip("Force the spawn position?")]
        [field: SerializeField] public virtual bool UseForceSpawnPosition { get; set; }
        
        [field: Tooltip("The force spawn position.")]
        [field: SerializeField] public virtual Vector2 ForceSpawnPosition { get; set; }
        
        [field: Tooltip("The color of the gizmo.")]
        [field: SerializeField] public virtual Color GizmoColor { get; set; } = Color.green; // set a default color
        
        [field: Tooltip("The transition parameters.")]
        [field: SerializeField] public virtual TransitionParameters TransitionParameters { get; set; }
        
        [field: Tooltip("Save on level change?")]
        [field: SerializeField] public virtual bool SaveOnLevelChange { get; set; } = true;

        
        protected BoxCollider2D boxCollider;

        public override void Awake()
        {
            base.Awake();
            boxCollider = GetComponent<BoxCollider2D>();
        }
        public virtual void OnTriggerEnter2D(Collider2D col)
        {
            if (col.CompareTag("Player"))
            {
                MessageSystem.MessageManager.SendImmediate(MessageChannels.UI, new TransitionMessage(
                    TransitionParameters.transitionType, TransitionParameters.slideDirection, 
                    TransitionParameters.animationDurationInSeconds,TransitionParameters.endEvent
                    ));
                MessageSystem.MessageManager.SendImmediate(MessageChannels.Level,
                    UseForceSpawnPosition
                        ? new LevelMessage(LevelObject.ID, LevelObject.Name, LevelState.Loading, ForceSpawnPosition)
                        : new LevelMessage(LevelObject.ID, LevelObject.Name, LevelState.Loading, LevelObject.PlayerSpawnPosition));
                
                if (SaveOnLevelChange)
                {
                    MessageSystem.MessageManager.SendImmediate(MessageChannels.Saves, new SaveLoadMessage(PlayerManager.Instance.Player.ID, PlayerManager.Instance.Player.Name, SaveOperation.Save));
                }
            }
        }

        // Draw a colored box for the collider bounds
        public virtual void OnDrawGizmos()
        {
            if (boxCollider == null)
            {
                boxCollider = GetComponent<BoxCollider2D>();
            }

            if (boxCollider == null)
            {
                return;
            }

            Gizmos.color = GizmoColor;
            Gizmos.DrawCube(boxCollider.bounds.center, boxCollider.bounds.size);
        }
        
        // This method is called when transform of the GameObject changes
        public virtual void OnTransformChildrenChanged()
        {
            // If boxCollider is null, get it
            if (boxCollider == null)
            {
                boxCollider = GetComponent<BoxCollider2D>();
            }

            // If boxCollider is still null, return
            if (boxCollider == null)
            {
                return;
            }

            // Update the size and position of the BoxCollider2D based on the GameObject's transform
            boxCollider.size = transform.localScale;
            boxCollider.offset = transform.localPosition;
        }
    }
}