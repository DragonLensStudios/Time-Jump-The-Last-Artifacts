using PXE.Core.Enemy;
using PXE.Core.Extensions.TransformExtensions;
using UnityEngine;

namespace PXE.Example_Games.Oceans_Call.Enemy
{
    /// <summary>
    ///  Represents the HorizontalEnemyController.
    /// </summary>
    public class HorizontalEnemyController : EnemyActorController
    {
        
        /// <summary>
        ///  Handles flipping the sprite renderer based on the direction of the target and the current patrol point.
        /// </summary>
        public override void FixedUpdate()
        {
            base.FixedUpdate();
            Vector2 targetPosition = Vector2.zero;
            if (PatrolPoints.Count > CurrentPatrolIndex)
            {

                if (Target == null)
                {
                    targetPosition = PatrolPoints[CurrentPatrolIndex];
                }
                else
                {
                    targetPosition = isChasing ? Target.position : PatrolPoints[CurrentPatrolIndex];
                }
            }
            else
            {
                if(Target != null)
                {
                    targetPosition = isChasing ? Target.position : Vector3.zero;
                }
                else
                {
                    targetPosition = Vector3.zero;
                }
            }

            Vector2 directionToTarget = targetPosition - (Vector2)transform.position;
            float angleToTarget = Mathf.Atan2(directionToTarget.y, directionToTarget.x) * Mathf.Rad2Deg;

            // Vertical flipping logic based on the angle
            sr.flipY = angleToTarget is > 90 or < -90; // Flip vertically if angle is greater than 90 degrees or less than -90 degrees

            if (isChasing)
            {
                if (Target != null)
                {
                    transform.Rotate2D(Target.position);
                }

            }
            else if (PatrolPoints.Count > CurrentPatrolIndex)
            {
                transform.Rotate2D(PatrolPoints[CurrentPatrolIndex]);
            }
        }
    }
}