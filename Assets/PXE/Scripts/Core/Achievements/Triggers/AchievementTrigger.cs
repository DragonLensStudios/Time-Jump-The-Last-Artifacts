using PXE.Core.Achievements.Data;
using PXE.Core.Achievements.Messaging.Messages;
using PXE.Core.Enums;
using PXE.Core.Messaging;
using PXE.Core.Objects;
using UnityEngine;

namespace PXE.Core.Achievements.Triggers
{
    /// <summary>
    ///  Represents the AchievementTrigger.
    /// </summary>
    [RequireComponent(typeof(Collider2D))]
    public class AchievementTrigger : ObjectController
    {
        [field: Tooltip("The achievement to trigger")]
        [field: SerializeField] public virtual Achievement Achievement { get; set; }
        
        [field: Tooltip("The progress to add to the achievement")]
        [field: SerializeField] public virtual float Progress { get; set; }
        
        [field: Tooltip("Should the achievement be unlocked?")]
        
        [field: SerializeField] public virtual bool Unlock { get; set; }

        protected Collider2D achievementCollider;

        /// <summary>
        ///  Executes the Awake method for the AchievementTrigger sets the achievementCollider to a trigger.
        /// </summary>
        public override void Awake()
        {
            base.Awake();
            achievementCollider = GetComponent<Collider2D>();
            achievementCollider.isTrigger = true;
        }
        
        /// <summary>
        ///  Executes the OnTriggerEnter2D method for the AchievementTrigger sends a message to the achievement manager.
        /// </summary>
        /// <param name="col"></param>
        public virtual void OnTriggerEnter2D(Collider2D col)
        {
            if (col.gameObject.CompareTag("Player"))
            {
                MessageSystem.MessageManager.SendImmediate(MessageChannels.Achievement,
                    Achievement.Progression
                        ? new AchievementMessage(Achievement.Key, AchievementOperator.Add, Progress)
                        : new AchievementMessage(Achievement.Key, AchievementOperator.Unlock));
            }
        }
    }
}